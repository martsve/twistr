using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwistrSolver.Model;

namespace TwistrSolver
{
    class CandyBag : List<CandyType>
    { 
        public CandyBag(List<CandyType> list) 
        {
            AddRange(list);
        }
        
        public static List<CandyType> Random(int amount, int seed = 1) {
            var rand = new Random(seed);
            var enums = CandyTypeParser.GetAllTypes();
            var list = Enumerable.Range(0, amount).Select(x => enums[rand.Next(0, enums.Count() - 1)]).ToList();
            return new CandyBag(list);
        }

        public static List<CandyType> FromFile(string filename) 
        {
            var contents = File.ReadAllText(filename);
            return FromString(contents);
        }

        public static List<CandyType> FromString(string contents) 
        {
            var lines = contents.Replace("\r","").Split("\n");
            var bag = new List<CandyType>();
            foreach (var line in lines.Where(x => x.Trim().Length > 0)) 
            {
                var w = line.Replace('\t',' ').Trim().Split(' ');
                var type = CandyTypeParser.Parse(w[0]);
                var value = w.Length > 1 ? int.Parse(w[1].Trim()) : 1;
                for (int i = 0; i < value; i++)
                {
                    bag.Add(type);
                }
            }
            return bag;
        }
    }
}