using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TwistrSolver.Model;
using TwistrSolver.Extensions;

namespace TwistrSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = Person.FromFile("pref.inp");
            var candyBag = CandyBag.FromFile("twist.inp");

            var simple = new TwistDrafter(people, candyBag).Invoke();
            var swapped = new TwistSwapper(simple, 10, 2, 5).Invoke();
            var stolen = new TwistStealer(simple, 10, 2, 5).Invoke();

            var combined = new TwistSwapper(stolen, 10, 2, 5).Invoke();
            for (int i = 0; i < 5; i++) 
            {
                combined = new TwistStealer(combined, 10, 2, 5).Invoke();
                combined = new TwistSwapper(combined, 10, 2, 5).Invoke();
                combined = new TwistChainSwapper(combined, 1).Invoke();
            }

            var chainswap = new TwistChainSwapper(simple, 2).Invoke();

            var results = new Dictionary<string, Outcome>();
            results.Add("swapped", swapped);
            results.Add("stolen", stolen);
            results.Add("combined", combined);
            
            var allSimples = PermutationsByPeopleOrder(people, candyBag);
            var j = 1;
            foreach (var division in allSimples) 
            {
                results.Add("simple" + j++, division);
            }

            results.Add("chained", chainswap);

            using (StreamWriter file = new StreamWriter(@"D:\summary.txt"))
            {
                file.WriteLine($"Name\tMinimum\tAverage");
                foreach (var division in results) 
                {
                    var sums = division.Value.Sums;
                    file.WriteLine($"{division.Key}\t{division.Value.Min}\t{division.Value.Average}");
                }
            }

            PrintOutcome(chainswap, @"D:\chained.txt");
            PrintOutcome(combined, @"D:\combined.txt");
        }

        private static void PrintOutcome(Outcome outcome, string filename) 
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (var person in outcome.Persons)
                {
                    file.WriteLine($"{person.Name}");
                    file.WriteLine($"-----------------------");
                    foreach (var candy in outcome.Bags[person].OrderByDescending(x => person.Preferences[x]))
                    {
                        file.WriteLine($"  {candy}");
                    }
                    file.WriteLine("");
                }
            }
        }

        private static List<Outcome> PermutationsByPeopleOrder(List<Person> people, List<CandyType> candyBag) 
        {
            var combinations = people.Permutations();
            var list = new List<Outcome>();
            foreach (var order in combinations)
            {
                var division = new TwistDrafter(order, candyBag).Invoke();
                list.Add(division);
            }
            return list;
        }
    }
}
