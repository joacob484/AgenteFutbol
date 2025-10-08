using System.Collections.Generic;

namespace AF.Core
{
    [System.Serializable]
    public class Agent
    {
        public string Name = "Agente";
        public int Reputation = 0;     // afecta slots y respuesta de clubes
        public long Money = 500_000;   // en euros
        public int SlotLimit = 5;
        public List<string> RepresentedIds = new();

        public bool HasFreeSlot => RepresentedIds.Count < SlotLimit;
    }
}
