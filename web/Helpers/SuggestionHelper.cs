using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Delver.FromFile;
using Twistr_web.Models;
using TwistrSolver;
using TwistrSolver.Model;

namespace Twistr_web.Helper
{
    public class SuggestionHelper 
    {
        public bool IsValidPreference(List<Preference> Preferences, Preference preference, out string error) 
        {
            if (preference == null)
            {
                error = $"No valid prefernece object attached.";
                return false;
            }
            
            if (preference.BestCount == 0 || preference.WorstCount == 0)
            {
                error = $"Preferences can't have counts of 0.";
                return false;
            }

            if (preference.BestCount == preference.WorstCount && preference.BestType == preference.WorstType)
            {
                error = $"Preferences can't have same count and type.";
                return false;
            }

            error = string.Empty;
            return true;
        }
        
        public List<Suggestion> GetSuggestions(List<Preference> preferences)
        {
            var suggestions = new List<Suggestion>();

            var types = CandyTypeParser.GetAllTypes();

            for (var i = 0; i < types.Count(); i++)
            {
                for (var j = i + 1; j < types.Count(); j++)
                {
                    if (preferences.Any(x => x.BestCount == 1 && x.WorstCount == 1 && (x.BestType == types[i] || x.WorstType == types[i]) && (x.BestType == types[j] || x.WorstType == types[j]) ))
                    {

                    }
                    else {
                        suggestions.Add(new Suggestion(){
                            TypeA = types[i],
                            CountA = 1,  
                            TypeB = types[j],
                            CountB = 1,
                        });
                    }
                }
            }
          
            var usage = preferences.Select(x => x.BestType).Concat(preferences.Select(x => x.WorstType)).ToList();
            var usageCount = types.ToDictionary(x => x, x => usage.Count(y => y == x));

            return suggestions.OrderBy(x => usageCount[x.TypeA] + usageCount[x.TypeB]).Take(1).ToList();

            /*
            var least = usageCount.OrderBy(x => x.Value).First().Key;
            var compareUsage = preferences.Where(x => x.WorstType == least).Select(x => x.BestType).Concat(preferences.Where(x => x.BestType == least).Select(x =>  x.WorstType)).ToList();
            var compoareUsageCount = types.ToDictionary(x => x, x => compareUsage.Count(y => y == x) + usageCount[x]);
            var compareLeast = compoareUsageCount.OrderBy(x => x.Value).First(x => x.Key != least).Key;

            suggestions.Add(new Suggestion() {
                TypeA = least,
                CountA = 1,  
                TypeB = compareLeast,
                CountB = 1,
            });

            return suggestions;
             */
        }        

        public List<Rank> GetRank(List<Preference> preferences)
        {
            if (preferences == null)
            {
                preferences = new List<Preference>();
            }

            var types = CandyTypeParser.GetAllTypes();
            var singles = preferences.Where(x => x.BestCount == 1 && x.WorstCount == 1).OrderBy(x => Math.Min((int)x.BestType, (int)x.WorstType)).ToList();
            var missingTypes = types.Except(singles.Select(x => x.BestType).Concat(singles.Select(x => x.WorstType))).ToList();

            var rank = new List<Rank>();

            for (var i = 0; i < 1000; i++)
            {
                bool changed = false;                
                foreach (var pref in singles.ToList())
                {
                    var best = rank.FirstOrDefault(x => x.Type == pref.BestType);
                    var worst = rank.FirstOrDefault(x => x.Type == pref.WorstType);

                    if (best != null && worst != null)
                    {
                        var posBest = rank.IndexOf(best);
                        var posWorst = rank.IndexOf(worst);
                        if (posWorst > posBest)
                        {
                            rank.Remove(worst);
                            rank.Insert(posBest, worst);
                        }
                    }

                    else if (best != null)
                    {
                        var pos = rank.IndexOf(best); // insert worst before best
                        rank.Insert(pos , new Rank(){
                            Type = pref.WorstType,
                        });
                        changed = true;
                        singles.Remove(pref);
                    }
                    else if (worst != null)
                    {
                        var pos = rank.IndexOf(worst); // insert best before worst
                        if (pos == rank.Count() - 1)
                        {
                            rank.Add(new Rank(){
                                Type = pref.BestType,
                            });
                        }
                        else {
                            rank.Insert(pos + 1 , new Rank(){
                                Type = pref.BestType,
                            });
                        }
                        changed = true;
                        singles.Remove(pref);
                    }
                    else if (!rank.Any())
                    {
                        rank.Add(new Rank(){
                            Type = pref.WorstType,
                        });
                        rank.Add(new Rank(){
                            Type = pref.BestType,
                        });
                        changed = true;
                        singles.Remove(pref);
                    }
                    else {
                        // happy
                    }

                    if (changed)
                        break;
                }

                if (!changed)
                    break;
            }

            return rank; 
        }
    }
}