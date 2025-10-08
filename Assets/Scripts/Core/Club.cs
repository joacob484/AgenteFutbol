namespace AF.Core
{
    [System.Serializable]
    public class ClubFacilities
    {
        // Nombres EXACTOS que usa FacilitiesService:
        public int Stadium  = 1;
        public int Academy  = 1;  // (antes “Training”)
        public int Marketing = 1;
        public int Medical   = 1;
    }

    [System.Serializable]
    public class Club
    {
        public string Name;
        public int Reputation;
        public int Budget;
        public ClubFacilities Facilities = new ClubFacilities();
    }
}
