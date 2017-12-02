using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Delver.FromFile;
using Twistr_web.Models;

namespace Twistr_web.Helper
{
    public static class DatabaseHelper 
    {
        private static string Preferences_db = "twistr_preferences.db";

        private static FromFile<Dictionary<string, List<Preference>>> Preferences_file = new FromFile<Dictionary<string, List<Preference>>>(Preferences_db);

        private static Dictionary<string, List<Preference>> Preferences;

        public static void Add(string key, Preference preference) 
        {
            using (Preferences_file.Lock(out Preferences))
            {
                if (!Preferences.ContainsKey(key))
                {
                    Preferences.Add(key, new List<Preference>());
                }

                var duplicate = Preferences[key].FirstOrDefault(x => x.Samy(preference));
                if (duplicate != null)
                {
                    Preferences[key].Remove(preference);
                }

                Preferences[key].Add(preference);
            }
        }

        public static void Remove(string key, int id) 
        {
            if (Preferences == null)
            {
                Preferences = Preferences_file.Read();
            }

            if (!Preferences.ContainsKey(key))
            {
                RequestHelper.Respons("InvalidInput", $"No item with id {id} found", 400);
            }

            using (Preferences_file.Lock(out Preferences))
            {
                var item = Preferences[key].FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    Preferences[key].Remove(item);
                }
            }
        }

        public static List<Preference> Get(string key) 
        {
            if (Preferences == null)
            {
                Preferences = Preferences_file.Read();
            }
            return Preferences.ContainsKey(key) ? Preferences[key] : new List<Preference>();
        }

        public static bool Exists(string key) 
        {
            if (Preferences == null)
            {
                Preferences = Preferences_file.Read();
            }
            return Preferences.ContainsKey(key);
        }
        public static int NextId(string key) 
        {
            var list = Get(key);
            return list.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
        }
    }
}