using System.Collections.Generic;

namespace AF.Services.World
{
    public static class NewsService
    {
        static readonly List<string> _news = new();
        public static void Clear() => _news.Clear();
        public static void Add(string msg) { if (!string.IsNullOrEmpty(msg)) _news.Insert(0, msg); }
        public static IReadOnlyList<string> All() => _news.AsReadOnly();
    }
}
