using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class SaveData
    {
        public int Version = 1;
        public WorldState World = new();
        public Agent Agent = new();
        public TimeState Time = new();

        [System.Serializable]
        public class WorldState
        {
            public Dictionary<string, Player> Players = new();
            public Dictionary<string, Club> Clubs = new();
            public Dictionary<string, League> Leagues = new();
        }

        [System.Serializable]
        public class TimeState
        {
            public int Week = 1;
            public int Season = 1;
            public int TransferWindow = 1;
        }
    }
}
