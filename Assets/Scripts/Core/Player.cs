namespace AF.Core
{
    [System.Serializable]
    public class Player
    {
        // IDs y vínculos
        public string Id;
        public string ClubId;

        // Campos base
        public string FullName;
        public Position Position;   // enum (GK/DF/MF/FW)
        public int Overall;         // 1..100
        public int Potential;       // 1..100
        public int Age;

        // Campos que usa WorldGenerator / vistas
        public int ContractYears = 0;
        public Personality Personality = Personality.Professional;
        public double MarketValueM = 0.0; // valor en millones
        public float AgentAffinity = 0f;

        // ===== Aliases (compatibilidad con vistas/servicios existentes) =====
        public string Name
        {
            get => FullName;
            set => FullName = value;
        }

        public Position Pos
        {
            get => Position;
            set => Position = value;
        }

        public int Ovr
        {
            get => Overall;
            set => Overall = value;
        }

        public int Pot
        {
            get => Potential;
            set => Potential = value;
        }
    }
}
