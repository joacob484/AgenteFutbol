using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class SaveData
    {
        public Agent Agent = new Agent();
        public WorldData World = new WorldData();
        public TimeState Time = new TimeState();   // ← usado por TimeService
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
        public int Week = 1;           // 1..52
        public int Day = 1;            // 1..7 (si lo usás)
        public int Season = 1;
        public int Tick = 0;

        // ⬇️ NUEVO: requerido por TimeService
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
}
