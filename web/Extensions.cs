using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwistrSolver
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Distinct<T, K>(this IEnumerable<T> list, Func<T, K> func)
        {
            return list.GroupBy(func).Select(x => x.First());
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }

        public static List<List<T>> Permutations<T>(this List<T> list)
        {
            var result = new List<List<T>>();

            if (list.Count == 1)
            {
                result.Add(list);
                return result;
            }
                
            for (int i = 0; i < list.Count; i++) {
                var sublist = new List<T>();
                for (int j = 0; j < list.Count; j++)
                    if (i != j)
                        sublist.Add(list[j]);
                foreach (var permutation in sublist.Permutations()) 
                {
                    permutation.Insert(0, list[i]);
                    result.Add(permutation);
                }
            }

            return result;
        }

        public static double StDev(this IEnumerable<double> list) 
        {
            double average = list.Average();
            double sumOfSquaresOfDifferences = list.Select(val => (val - average) * (val - average)).Sum();
            return Math.Sqrt(sumOfSquaresOfDifferences / list.Count()); 
        }
    }
}