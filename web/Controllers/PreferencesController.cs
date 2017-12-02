using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twistr_web.Helper;
using Twistr_web.Models;
using TwistrSolver.Model;

namespace Twistr_web.Controllers
{
    [Route("api/[controller]")]
    public class PreferencesController : Controller
    {
        private string Key => RequestHelper.ApiUser(Request);
        private SuggestionHelper SuggestionHelper = new SuggestionHelper();

        [HttpGet("")]
        public List<Preference> Get()
        {
            return DatabaseHelper.Get(Key);
        }
        
        [HttpGet("rank")]
        public List<Rank> Rank()
        {
            var preferences = DatabaseHelper.Get(Key);
            return SuggestionHelper.GetRank(preferences);
        }
        
        [HttpGet("info")]
        public List<CandyInfo> Info()
        {
            return CandyTypeParser.GetAllTypes().Select(x => new CandyInfo(x)).ToList();
        }

        [HttpGet("suggest")]
        public List<Suggestion> Suggest()
        {
            return SuggestionHelper.GetSuggestions(DatabaseHelper.Get(Key));
        }

        [HttpPost("")]
        public List<Suggestion> Post([FromBody]Preference preference)
        {
            var preferences = DatabaseHelper.Get(Key);

            string error;
            if (!SuggestionHelper.IsValidPreference(preferences, preference, out error))
            {
                RequestHelper.Respons("InvalidInput", error, 400);
            }

            preference.Id = DatabaseHelper.NextId(Key);

            DatabaseHelper.Add(Key, preference);

            return SuggestionHelper.GetSuggestions(DatabaseHelper.Get(Key));
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DatabaseHelper.Remove(Key, id);
        }
    }
}
