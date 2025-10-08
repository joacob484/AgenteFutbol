using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class League
    {
        public string Id;
        public string Name;
        public AF.Core.Region Region;
        public int Tier; // 1 = máxima
        public List<string> Clubs = new();
    }
}
