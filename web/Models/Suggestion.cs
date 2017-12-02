using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwistrSolver.Model;

namespace Twistr_web.Models
{
    public class Suggestion 
    {
        public CandyType TypeA { get; set; }
        public int CountA { get; set; }

        public CandyType TypeB { get; set; }
        public int CountB { get; set; }
    }
}