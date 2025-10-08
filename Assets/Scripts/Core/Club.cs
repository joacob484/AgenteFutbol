using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class ClubFacilities
    {
        public int Stadium  = 1;
        public int Academy  = 1;
        public int Marketing = 1;
        public int Medical   = 1;
    }

    [System.Serializable]
    public class Club
    {
        public string Id;                       // ← usado por WorldGenerator
        public string Name;
        public int Reputation;
        public int Budget;
        public string LeagueId;                 // ← usado por WorldGenerator
        public List<string> Squad = new();      // ← ids de Players
        public ClubFacilities Facilities = new ClubFacilities();
    }
}
