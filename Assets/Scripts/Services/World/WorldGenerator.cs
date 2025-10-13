using AF.Services.Game; // para RandomService.Range(...)
using System.Collections.Generic;
using UnityEngine;
using AF.Core;

namespace AF.Services.World
{
    public static class WorldGenerator
    {
        public static void Generate(SaveData save)
        {
            // Ligas base (2)
            var lig1 = new League { Id = "fra1", Name = "France League", Region = Region.Europe, Tier = 1 };
            var lig2 = new League { Id = "spa1", Name = "Spain League", Region = Region.Europe, Tier = 1 };
            save.World.Leagues[lig1.Id] = lig1;
            save.World.Leagues[lig2.Id] = lig2;

            // Clubes (4) de ejemplo
            AddClub(save, lig1, "lyon",  "Lyon", 4, 120_000_000);
            AddClub(save, lig1, "reims", "Reims",3, 60_000_000);
            AddClub(save, lig2, "rmad",  "R. Madrid",5, 300_000_000);
            AddClub(save, lig2, "sev",   "Sevilla",4, 150_000_000);

            // Generar talentos libres (30)
            var names = JsonUtility.FromJson<NameList>(AF.Core.GameConfig.NamesES.text);
            for (int i = 0; i < 30; i++)
            {
                var pl = MakeRandomPlayer(names);
                save.World.Players[pl.Id] = pl;
            }

            // Completar planteles de clubes (8 jugadores cada uno)
            foreach (var club in save.World.Clubs.Values)
            {
                for (int i = 0; i < 8; i++)
                {
                    var pl = MakeRandomPlayer(names);
                    pl.ClubId = club.Id;
                    save.World.Players[pl.Id] = pl;
                    club.Squad.Add(pl.Id);
                }
            }
        }

        static void AddClub(SaveData save, League league, string id, string name, int rep, long budget)
        {
            var c = new Club { Id = id, Name = name, LeagueId = league.Id, Reputation = rep, Budget = budget };
            save.World.Clubs[id] = c;
            league.Clubs.Add(id);
        }

        static Player MakeRandomPlayer(NameList names)
        {
            string first = names.first[RandomService.Range(0, names.first.Length - 1)];
            string last  = names.last[RandomService.Range(0, names.last.Length - 1)];
            var pos = (Position)RandomService.Range(0, 3);
            int ovr = RandomService.Range(60, 92);
            int pot = Mathf.Clamp(ovr + RandomService.Range(0, 10), 60, 99);

            return new Player
            {
                Id = System.Guid.NewGuid().ToString("N"),
                Name = $"{first} {last}",
                Age = RandomService.Range(17, 34),
                Pos = pos,
                Ovr = ovr,
                Pot = pot,
                ClubId = null,
                ContractYears = 0,
                Personality = Personality.Professional,
                MarketValueM = System.Math.Round((ovr - 50) * 0.6 + (pot - ovr) * 0.8, 1),
                AgentAffinity = 0f
            };
        }

        [System.Serializable]
        public class NameList { public string[] first; public string[] last; }
    }
}
