using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class SaveData
    {
        public Agent Agent = new Agent();
        public WorldData World = new WorldData();
        public TimeState Time = new TimeState();

        // ⬇️ NUEVO: ofertas de mercado pendientes (persistentes)
        public List<MarketOfferDTO> PendingOffers = new();
    }

    [System.Serializable]
    public class WorldData
    {
        public Dictionary<string, League> Leagues = new();
        public Dictionary<string, Club>   Clubs   = new();
        public Dictionary<string, Player> Players = new();
    }

    /// <summary> Estado del calendario del save. </summary>
    [System.Serializable]
    public class TimeState
    {
        public int Week = 1;     // 1..52
        public int Day = 1;      // 1..7 (si lo usás)
        public int Season = 1;
        public int Tick = 0;

        public int TransferWindow = 0;

        public void NextDay()
        {
            Day++;
            if (Day > 7) { Day = 1; NextWeek(); }
            Tick++;
        }

        public void NextWeek()
        {
            Week++;
            if (Week > 52) { Week = 1; Season++; }
            Tick++;
        }
    }

    /// <summary> DTO serializable para ofertas de mercado. </summary>
    [System.Serializable]
    public class MarketOfferDTO
    {
        public string PlayerId;
        public string FromClubId;
        public string ToClubId;
        public long Fee;         // en euros
        public long YearlyWage;  // en euros
        public int Years;
        public float CommissionRate;
    }
}
