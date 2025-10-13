using UnityEngine;

namespace AF.Services.Game
{
    /// <summary>
    /// Servicio de aleatorios unificado para todo el juego.
    /// </summary>
    public static class RandomService
    {
        // RNG compartido (lo que pide RecruitmentService)
        public static readonly System.Random Rng = new System.Random();

        // Helpers útiles en otros lugares
        // Entero inclusivo en ambos extremos (0..n) → igual a Unity pero inclusivo arriba
        public static int Next(int minInclusive, int maxInclusive)
            => Rng.Next(minInclusive, maxInclusive + 1);

        // 0..1 (double → float)
        public static float Value01()
            => (float)Rng.NextDouble();

        // Proxy a UnityEngine.Random cuando necesites floats distribuidos por frame
        public static int Range(int minInclusive, int maxInclusive)
            => Random.Range(minInclusive, maxInclusive + 1);
    }
}
