using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class League
    {
        public string Id;
        public string Name;
        public Region Region;
        public int Tier;
        public List<string> Clubs = new(); // ← WorldGenerator agrega ids acá
    }
}
