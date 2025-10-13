using System.Collections.Generic;
using AF.Core;
using AF.Services.Game;

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
            var rng = new System.Random(); // usamos el System.Random compartido

            var p = new Player
            {
                Id   = System.Guid.NewGuid().ToString("N"),
                Name = NameGenerator.RandomName(region, rng), // tu generador pide un Random
                Age  = rng.Next(16, 25),

                // enum Position (0..3) según tu Enums.cs
                Pos  = (Position)rng.Next(0, System.Enum.GetValues(typeof(Position)).Length),

                // compat con tu modelo (Potential, Ovr/Overall, MarketValueM es double)
                Potential    = rng.Next(60, 95),
                Ovr          = rng.Next(55, 90),       // si no usás Ovr, podés quitar esta línea
                MarketValueM = rng.Next(1, 20),        // valor en millones
                ContractYears = 0,
                Personality   = Personality.Professional,
                AgentAffinity = 0f
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
