using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;

namespace TwistrSolver
{
    class Outcome 
    {
        public Dictionary<Person, List<CandyType>> Bags { get; private set; }
        public Dictionary<Person, double> Sum { get; private set; }

        public List<Person> Persons => Bags.Keys.ToList();

        public double[] Sums => Sum.Select(x => x.Value).ToArray();
        public double Min { private set; get; }
        public double StDev { private set; get; }
        public double Average { private set; get; }

        public Outcome(Dictionary<Person, List<CandyType>> bags)
        {
            Bags = bags;
            Sum = bags.ToDictionary(x => x.Key, x => x.Value.Select(y => x.Key.Preferences[y]).Sum());
            Calculate();
        }

        private Outcome() 
        {
        }

        public Outcome Copy() 
        {
            var outcome = new Outcome();
            outcome.Bags = Bags.ToDictionary(x => x.Key, x => x.Value.ToList());
            outcome.Sum = Sum.ToDictionary(x => x.Key, x => x.Value);
            outcome.Min = Min;
            outcome.Average = Average;
            outcome.StDev = StDev;
            return outcome;
        }

        public void Add(Person person, CandyType type)
        {
            Bags[person].Add(type);
            Sum[person] += person.Preferences[type];
            Calculate();
        }

        public void Remove(Person person, CandyType type)
        {
            Bags[person].Remove(type);
            Sum[person] -= person.Preferences[type];
            Calculate();
        }

        public bool IsBetterThan(Outcome outcome)
        {
            if (Min < outcome.Min)
                return false;
            if (Min > outcome.Min)
                return true;
                
            if (Average < outcome.Average)
                return false;
            if (Average > outcome.Average)
                return true;

            if (StDev < outcome.StDev)
                return false;
            if (StDev > outcome.StDev)
                return true;

            return false;
        }

        private void Calculate() 
        {
            Min = Sums.Min();
            StDev = Sums.StDev();
            Average = Sums.Average();
        }
    }
}