using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;
using TwistrSolver.Interface;

namespace TwistrSolver
{
    class TwistStealer : ITwistDivider
    {
        Outcome Bag;

        int Iterations;
        int Depth;
        int MaxSwaps;

        private bool hasChanged;

        public bool HasChanged() {
            return hasChanged;
        }

        public TwistStealer(Outcome bags, int iterations = 5, int depth = 3, int maxSwaps = 10)
        {
            Bag = bags;
            Iterations = iterations;
            Depth = depth;
            MaxSwaps = maxSwaps;
        }

        public Outcome Invoke() 
        {
            return DoTheCandySteal(Bag, out hasChanged);    
        }      

        public Outcome DoTheCandySteal(Outcome bags, out bool didSteel, int depth = 0)
        {
            if (depth == 0)
                Console.WriteLine("---------- STEAL STEAL STEAL ---------");
            else
                Console.WriteLine($"      Stealing at depth {depth}");

            var swaps = FindGoodSteals(bags);

            if (swaps.Count == 0 || depth == Depth)
            {
                didSteel = false;
                return bags;
            }

            didSteel = true;

            var bestBag = bags.Copy();
            Console.WriteLine($"Found {swaps.Count()} steals");
            foreach (var swap in swaps)
            {
                var newBag = bags.Copy();

                newBag.Add(swap.From, swap.Take);
                newBag.Remove(swap.To, swap.Take);

                bool tmp;
                newBag = DoTheCandySteal(newBag, out tmp, depth + 1);

                if (newBag.IsBetterThan(bestBag))
                {
                    Console.WriteLine($"Bag upgrade!");
                    bestBag = newBag;
                }
            }

            if (depth == 0)
            {
                for (int i = 0; i < Iterations; i++)
                {
                    Console.WriteLine(" -- Here we go stealing again!! -- ");
                    bool didSwap;
                    bestBag = DoTheCandySteal(bestBag, out didSwap, 1);
                    if (!didSwap)
                        break;
                }
            }

            return bestBag;
        }

        private List<Swap> FindGoodSteals(Outcome outcome)
        {
            var averages = outcome.Sum;
            var lower = outcome.Min;
            var swaps = new List<Swap>();
            foreach (var player in outcome.Persons)
            {
                foreach (var other in outcome.Persons.Where(x => x != player))
                {
                    foreach (var otherCandy in outcome.Bags[other].Distinct())
                    {
                        var add = player.Preferences[otherCandy];
                        var remOther = other.Preferences[otherCandy];

                        var newAvg = averages[player] + add;
                        var newAvgOther = averages[other] - remOther;

                        if (add > remOther && lower <= newAvg && lower <= newAvgOther)
                        {
                            swaps.Add(new Swap()
                            {
                                From = player,
                                To = other,
                                Take = otherCandy,
                                Gain = add - remOther,
                            });
                        }
                    }
                }
            }

            return swaps.OrderByDescending(x => x.Gain).Take(MaxSwaps).ToList();
        }
        
        private struct Swap {
            public Person From;
            public Person To;
            public CandyType Take;
            public double Gain;
        }
        
    }
}
