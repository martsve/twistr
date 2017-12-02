using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;
using TwistrSolver.Interface;

namespace TwistrSolver
{
    class TwistDrafter : ITwistDivider
    {
        List<Person> People;
        List<CandyType> Bag;

        public bool HasChanged() {
            return false;
        }

        public TwistDrafter(List<Person> people, List<CandyType> bag)
        {
            Bag = bag;
            People = people;
        }

        public Outcome Invoke() 
        {
            var N = People.Count;
            var division = new Dictionary<Person, List<CandyType>>();
            var pickList = new Dictionary<Person, List<CandyType>>();

            foreach (var person in People) 
            {
                division.Add(person, new List<CandyType>());
                pickList.Add(person, Bag.OrderByDescending(x => person.Preferences[x]).ToList());
            }

            for (var i = 0; i < Bag.Count; i++) {
                var person = People[i % N];
                var candy = pickList[person].First();
                division[person].Add(candy);
                foreach (var item in pickList) 
                {
                    item.Value.Remove(candy);
                }
            }        
            
            return new Outcome(division);
        }      
    }
}
