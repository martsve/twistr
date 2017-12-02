using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;
using TwistrSolver.Interface;

namespace TwistrSolver
{
    class TwistSwapper : ITwistDivider
    {
        Outcome _outcome;

        int Iterations;
        int Depth;
        int MaxSwaps;

        private bool hasChanged;

        public bool HasChanged() {
            return hasChanged;
        }

        public TwistSwapper(Outcome outcome, int iterations = 5, int depth = 3, int maxSwaps = 10)
        {
            _outcome = outcome;
            Iterations = iterations;
            Depth = depth;
            MaxSwaps = maxSwaps;
        }

        public Outcome Invoke() 
        {
            return DoTheCandySwap(_outcome, out hasChanged);    
        }      

        public Outcome DoTheCandySwap(Outcome outcome, out bool hasSwapped, int depth = 0) 
        {
            if (depth == 0)
            Console.WriteLine("---------- SWIP SWAP SWAP ---------");
            else
            Console.WriteLine($"      Swapping at depth {depth}");

            var swaps = FindGoodSwaps(outcome);

            if (swaps.Count == 0 || depth == Depth) 
            {
                hasSwapped = false;
                return outcome;
            }

            hasSwapped = true;

            var bestBag = outcome.Copy();
            Console.WriteLine($"Found {swaps.Count()} swaps");
            foreach (var swap in swaps)
            {
                var newBag = outcome.Copy();

                newBag.Remove(swap.From, swap.Give);
                newBag.Add(swap.From, swap.Take);

                newBag.Remove(swap.To, swap.Take);
                newBag.Add(swap.To, swap.Give);

                bool tmp;
                newBag = DoTheCandySwap(newBag, out tmp, depth + 1);

                if (newBag.IsBetterThan(bestBag)) {
                    Console.WriteLine($"Bag upgrade!");
                    bestBag = newBag;
                }
            }

            if (depth == 0)
            {
                for (int i = 0; i < Iterations; i++)
                 {
                    Console.WriteLine(" -- Here we go swapping again!! -- ");
                    bool didSwap;
                    bestBag = DoTheCandySwap(bestBag, out didSwap, 1);
                    if (!didSwap)
                        break;
                }
            }
            
            return bestBag;    
        }

        
        List<Swap> FindGoodSwaps(Outcome outcome) 
        {
            var averages = outcome.Sum;
            var lower = outcome.Min;
            var swaps = new List<Swap>();
            foreach (var player in outcome.Persons) 
            {
                foreach (var candy in outcome.Bags[player].Distinct()) 
                {                    
                    foreach (var other in outcome.Persons.Where(x => x != player))
                    {
                        foreach (var otherCandy in outcome.Bags[other].Distinct().Where(x => x != candy))
                        {
                            var rem = player.Preferences[candy];
                            var add = player.Preferences[otherCandy];

                            var remOther = other.Preferences[otherCandy];
                            var addOther = other.Preferences[candy];

                            var newAvg = averages[player] - rem + add;
                            var newAvgOther = averages[other] - remOther + addOther;

                            if ( add - rem > addOther - remOther  && lower <= newAvg && lower <= newAvgOther) {
                                swaps.Add(new Swap() {
                                    From = player,
                                    To = other,
                                    Give = candy,
                                    Take = otherCandy,
                                    Gain = (add - rem) - (addOther - remOther)
                                });
                            }
                        }
                    }
                }
            }

            return swaps.OrderByDescending(x => x.Gain).Take(MaxSwaps).ToList();
        }

        private struct Swap {
            public Person From;
            public CandyType Give;
            public Person To;
            public CandyType Take;
            public double Gain;
        }
        
    }
}
