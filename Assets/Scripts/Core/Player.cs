namespace AF.Core
{
    [System.Serializable]
    public class Player
    {
        public string Id;        // requerido por generadores/listas
        public string FullName;
        public int Age;
        public int Overall;      // 1..100
        public int Potential;    // ← lo pide TalentsView
        public string Position;  // "GK","DF","MF","FW"
        public string ClubId;    // vínculo simple
    }
}
