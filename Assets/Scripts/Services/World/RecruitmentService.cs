using System.Collections.Generic;
using AF.Core;

namespace AF.Services.World
{
    public static class RecruitmentService
    {
        private static readonly string[] Regions = { "Europa", "Sudamérica", "África", "Asia", "Norteamérica" };

        /// <summary>
        /// Genera un nuevo jugador para reclutar, según región.
        /// </summary>
        public static Player Recruit(string region)
        {
            var rnd = RandomService.Rng;
            var p = new Player
            {
                Id = System.Guid.NewGuid().ToString(),
                Name = NameGenerator.RandomName(region, rnd),
                Age = rnd.Next(16, 25),
                Pos = (Position)rnd.Next(0, System.Enum.GetValues(typeof(Position)).Length),
                Potential = rnd.Next(60, 95),
                MarketValueM = rnd.Next(1, 20)
            };
            return p;
        }

        public static List<Player> RecruitBatch(string region, int count = 3)
        {
            var list = new List<Player>();
            for (int i = 0; i < count; i++)
                list.Add(Recruit(region));
            return list;
        }

        public static string[] GetRegions() => Regions;
    }
}
