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
        public string Id;
        public string Name;
        public int Reputation;
        public long Budget;                  // ← long (WorldGenerator pasa long)
        public string LeagueId;              // vínculo con liga
        public List<string> Squad = new();   // ids de jugadores
        public ClubFacilities Facilities = new ClubFacilities();
    }
}
