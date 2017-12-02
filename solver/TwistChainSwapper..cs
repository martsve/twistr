using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;
using TwistrSolver.Interface;

namespace TwistrSolver
{
    class TwistChainSwapper : ITwistDivider
    {
        Outcome _outcome;

        int Depth;

        private bool hasChanged = false;

        public bool HasChanged() {
            return hasChanged;
        }

        public TwistChainSwapper(Outcome outcome, int depth = 3)
        {
            _outcome = outcome;
            Depth = depth;
        }

        public Outcome Invoke() 
        {
            bool changed = true;
            var outcome = _outcome;
            Console.WriteLine("---------- CHAIN CHAIN CHAIN ---------");
            for (int i = 0; i < 100; i++) 
            {
                outcome = DoChainSwap(outcome, out changed);    
                if (!changed)
                    break;
                Console.WriteLine("Let's chainswap again!");
            }
            return outcome;
        }      

        enum GiveType 
        {
            A_B,
            A_B_C,
            A_B_C_A,
            A_B_C_D,
            A_B_C_D_A
        }

        public Outcome DoChainSwap(Outcome outcome, out bool changed, int depth = 0) 
        {
            changed = false;

            var outcomes = GetChainSwaps(outcome);

            if (outcomes.Any()) 
            {
                if (depth < Depth) {
                    var deeper = new List<Outcome>();
                    foreach (var item in outcomes)
                    {
                        var result = DoChainSwap(item, out changed, depth + 1);
                        if (changed || depth < Depth - 1)
                            deeper.Add(result);
                    }
                }

                changed = false;

                var bestOutcome = outcome.Copy();
                foreach (var newOutcome in outcomes.OrderByDescending(x => x.Min))
                {
                    if (newOutcome.IsBetterThan(bestOutcome))
                    {
                        Console.WriteLine($"- New best chainswap! Outcomes: {outcomes.Count()} - Depth: {depth} -");
                        bestOutcome = newOutcome;
                        changed = true;
                    }
                }

                return bestOutcome;
            }

            return outcome;
        }

        
        public List<Outcome> GetChainSwaps(Outcome outcome) 
        {
            var bestOutcome = outcome.Copy();
            var list = new List<Outcome>() { bestOutcome };
            
            var swapList = FindAllSwaps(outcome, GiveType.A_B_C);
            swapList.AddRange(FindAllSwaps(outcome, GiveType.A_B));
            swapList.AddRange(FindAllSwaps(outcome, GiveType.A_B_C_A));
            swapList.AddRange(FindAllSwaps(outcome, GiveType.A_B_C_D));
            swapList.AddRange(FindAllSwaps(outcome, GiveType.A_B_C_D_A));

            foreach (var swaps in swapList.OrderByDescending(x => x.Sum(y => y.Value)).ToList())
            {
                var newOutcome = outcome.Copy();

                foreach (var swap in swaps) 
                {
                    newOutcome.Remove(swap.From, swap.Give);
                    newOutcome.Add(swap.To, swap.Give);
                }
                /*
                if (newOutcome.IsBetterThan(bestOutcome))
                {
                    Console.WriteLine(" - New best chainswap! -");
                    bestOutcome = newOutcome;
                }*/
                list.Add(newOutcome);
            }

            return list.OrderByDescending(x => x.Min).ThenByDescending(x => x.Average).Take(3).ToList();
        }
        
        List<List<CandyType>> GetDeepChains(List<Person> order, Outcome outcome) 
        {
            var first = order.First();

            if (order.Count == 1) 
            {
                var list = new List<List<CandyType>> ();
                foreach (var item in outcome.Bags[first].Distinct())
                    list.Add(new List<CandyType>(){ item });
                return list;
            }

            var existing = GetDeepChains(order.Skip(1).ToList(), outcome);
            
            var all = new List<List<CandyType>> ();
            foreach (var item in outcome.Bags[first].Distinct())
            {
                foreach (var swaps in existing)
                {
                    var newList = new List<CandyType>() { item };
                    newList.AddRange(swaps);
                    all.Add(newList);
                }
            }

            return all;
        }   

        List<List<Swap>> FindAllSwaps(Outcome outcome, GiveType type) 
        {
            var swaps = new List<List<Swap>>();
            var orders = outcome.Persons.Permutations();

            foreach (var order in orders) 
            {
                var candySwapChain = GetDeepChains(order, outcome);
                var first = order[0];
                var second = order[1];
                var third = order[2];
                var fourth = order[3];
                foreach (var chain in candySwapChain) 
                {
                    var firstCandy = chain[0];
                    var secondCandy = chain[1];
                    var thirdCandy = chain[2];
                    var fourthCandy = chain[3];

                    var swapList = new List<Swap>();

                    swapList.Add(new Swap() {
                        From = first,
                        To = second,
                        Give = firstCandy,
                    });

                    if (type != GiveType.A_B) {                                    
                        swapList.Add(new Swap() {
                            From = second,
                            To = third,
                            Give = secondCandy,
                        });                                    
                    }


                    if (type == GiveType.A_B_C) {
                        ;
                    }

                    else if (type == GiveType.A_B_C_A) {
                        swapList.Add(new Swap() {
                            From = third,
                            To = first,
                            Give = thirdCandy,
                        });
                    }

                    else if (type == GiveType.A_B_C_D) {
                        swapList.Add(new Swap() {
                            From = third,
                            To = fourth,
                            Give = thirdCandy,
                        });
                    }

                    else if (type == GiveType.A_B_C_D_A) {
                        swapList.Add(new Swap() {
                            From = third,
                            To = fourth,
                            Give = thirdCandy,
                        });

                        swapList.Add(new Swap() {
                            From = fourth,
                            To = first,
                            Give = fourthCandy,
                        });
                    }

                    //if (swapList.Sum(x => x.Value) > 0)
                        swaps.Add(swapList);
                }          
            }

            return swaps;
        }

        private struct Swap {
            public Person From;
            public CandyType Give;
            public Person To;
            public double Value => To.Preferences[Give] - From.Preferences[Give];
        }
    }
}
