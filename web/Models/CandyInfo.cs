using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TwistrSolver.Model;

namespace Twistr_web.Models
{
    public class CandyInfo 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public CandyInfo(CandyType type)
        {
            Id = (int)type;
            Name = type.ToString();
            Image = $"img/{type.ToString().ToLower()}.png";
        }
    }
}