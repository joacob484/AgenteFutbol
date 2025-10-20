using System.Collections.Generic;

namespace AF.Services.World
{
    public static class NewsService
    {
        private static readonly List<string> _feed = new();

        public static void Add(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            _feed.Add(msg);
            if (_feed.Count > 50) _feed.RemoveAt(0);
        }

        public static List<string> GetFeed()
        {
            return new List<string>(_feed);
        }

        public static void Clear()
        {
            _feed.Clear();
        }
    }
}
