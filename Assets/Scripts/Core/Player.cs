using System;
using AF.Core;

namespace AF.Core
{
    [System.Serializable]
    public class Player
    {
        public string Id;
        public string Name;
        public int Age;
        public Position Pos;
        public int Ovr;     // overall
        public int Pot;     // potential
        public string ClubId; // null si libre
        public int ContractYears;
        public Personality Personality;
        public double MarketValueM; // millones €

        // Relación con el agente (si lo representa)
        public float AgentAffinity; // -1..+1

        public Player Clone() => (Player)MemberwiseClone();
    }
}
