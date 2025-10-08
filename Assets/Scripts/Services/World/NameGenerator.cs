using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF.Services.World
{
    [Serializable]
    public class NamesJson
    {
        public string region;
        public List<string> firstNames;
        public List<string> lastNames;
    }

    public static class NameGenerator
    {
        static readonly Dictionary<string, (List<string> first, List<string> last)> _data =
            new(StringComparer.OrdinalIgnoreCase);
        static bool _loaded;

        static void EnsureLoaded()
        {
            if (_loaded) return;
            _loaded = true;

            var files = new[] {
                "Data/names_europe",
                "Data/names_southamerica",
                "Data/names_africa",
                "Data/names_asia",
                "Data/names_northamerica"
            };

            foreach (var path in files)
            {
                var ta = Resources.Load<TextAsset>(path);
                if (ta == null) continue;
                var d = JsonUtility.FromJson<NamesJson>(ta.text);
                if (d == null || string.IsNullOrWhiteSpace(d.region)) continue;

                var first = (d.firstNames ?? new List<string>()).Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var last  = (d.lastNames  ?? new List<string>()).Where(x=>!string.IsNullOrWhiteSpace(x)).Select(x=>x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                _data[d.region.Trim()] = (first, last);
            }

            if (_data.Count == 0)
                _data["Europa"] = (new List<string>{"Luka","Andrei","Carlos","Marco","Jonas"}, new List<string>{"García","Müller"});
        }

        public static string RandomName(string region, System.Random rng)
        {
            EnsureLoaded();
            if (!_data.ContainsKey(region)) region = "Europa";
            var bank = _data[region];

            var first = bank.first.Count>0 ? bank.first[rng.Next(bank.first.Count)] : "Jugador";
            string last;
            if (bank.last.Count>0)
                last = bank.last[rng.Next(bank.last.Count)];
            else
                last = ((bank.first.Count>0 ? bank.first[rng.Next(bank.first.Count)] : "X") + " " + (char)('A' + rng.Next(26)) + ".");

            return $"{first} {last}";
        }
    }
}
