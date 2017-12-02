using System;
using System.Collections.Generic;
using System.Linq;

namespace TwistrSolver.Model
{
    public class CandyTypeParser 
    {
        public static CandyType Parse(string candy) 
        {
            return Enum.Parse<CandyType>(candy.Trim());   
        }

        public static List<CandyType> GetAllTypes()
        {
            return Enum.GetValues(typeof(CandyType)).Cast<CandyType>().ToList();
        }
    }

    public enum CandyType
    {
        Banan,
        Caramel,
        ChocolateToffee,
        Cocos,
        Daim,
        Eclairs,
        FranskNougat,
        GoldenToffee,
        Japp,
        Lakris,
        Marsipan,
        Nougatcrisp,
        Nøtti    
    }
}