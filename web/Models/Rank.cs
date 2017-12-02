using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwistrSolver;
using TwistrSolver.Model;

namespace Twistr_web.Models
{
    public class Rank 
    {
        public CandyType Type { get; set; }
        public int Strength { get; set; }
        public double Value { get; set; }
    }
}