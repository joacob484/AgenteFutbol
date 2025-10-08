using System;

namespace AF.Services.World
{
    public static class RandomService
    {
        static readonly System.Random _rng = new System.Random();
        public static int Range(int a, int b) => _rng.Next(a, b + 1);
        public static double Next01() => _rng.NextDouble();
        public static T Pick<T>(T[] arr) => arr[_rng.Next(arr.Length)];
    }
}
