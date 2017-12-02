using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwistrSolver.Model;

namespace Twistr_web.Models
{
    public class Preference 
    {
        public int Id { get; set; }

        public CandyType BestType { get; set; }
        public int BestCount { get; set; }

        public CandyType WorstType { get; set; }
        public int WorstCount { get; set; }

        public bool Equals(Preference obj)
        {
            if (this.Id > 0 && this.Id == obj.Id)
                return true;
                
            return this.BestType == obj.BestType && this.BestCount == obj.BestCount
                    && this.WorstType == obj.WorstType && this.WorstCount == obj.WorstCount;
        }
        
        public bool Samy(Preference obj)
        {
            if (this.Id > 0 && this.Id == obj.Id)
                return true;
                
            return this.BestCount == obj.BestCount && this.WorstCount == obj.WorstCount
                    && ((this.WorstType == obj.WorstType && this.BestType == obj.BestType) || (this.WorstType == obj.BestType && this.BestType == obj.WorstType) );
        }
    }
}