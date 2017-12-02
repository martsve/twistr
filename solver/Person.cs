using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TwistrSolver.Model;
using TwistrSolver.Extensions;

namespace TwistrSolver
{
    class Person {
        public Person(string name) 
        {
            Name = name;
        }

        public string Name { get; set; }
        
        public Dictionary<CandyType, double> Preferences { get; set; } = new Dictionary<CandyType, double>();

        public static Person RandomPerson(string name, int seed = 0) 
        {
            var rand = new Random(seed);
            var person = new Person(name);

            var enums = CandyTypeParser.GetAllTypes();
            var scale = Enumerable.Range(1, 100).OrderBy(x => rand.Next()).ToList();
            var sum = (double)scale.Take(enums.Count()).Sum();

            foreach (CandyType type in enums)
            {
                var value = scale.First();
                scale.Remove(value);
                person.Preferences.Add(type, value / sum);
            }

            return person;
        }

        public static List<Person> RandomPeople(int persons, int seed = 0) 
        {
            var rand = new Random(seed);
            var people = new List<Person>();
            for (int i = 0; i < persons; i++)
            {
                people.Add(Person.RandomPerson($"P{i}", rand.Next()));
            }
            return people;
        }
        
        public static List<Person> FromFile(string filename) 
        {
            var contents = File.ReadAllText(filename);
            return FromString(contents);
        }

        public static List<Person> FromString(string contents) 
        {
            var lines = contents.Replace("\r","").Split("\n");
            var names = lines.First().Split('\t').Skip(1);
            var N = names.Count();

            var preferences = lines.Skip(1);

            var people = new List<Person>();
            foreach (var name in names) {
                people.Add(new Person(name.Trim()));
            }

            foreach (var line in preferences.Where(x => x.Trim().Length > 0))
            {
                var w = line.Split('\t');
                var type = CandyTypeParser.Parse(w[0]);
                for (int i = 0; i < N; i++)
                {
                    var value = double.Parse(w[i + 1].Trim(), CultureInfo.InvariantCulture);
                    people[i].Preferences.Add(type, value);
                }
            }

            return people;
        }

    }
}