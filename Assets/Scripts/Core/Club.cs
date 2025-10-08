namespace AF.Core
{
    [System.Serializable]
    public class ClubFacilities
    {
        public int stadiumLevel = 1;
        public int trainingLevel = 1;   // el nombre del campo puede ser 'training' aunque el enum tenga 'Academy'
        public int marketingLevel = 1;
        public int medicalLevel = 1;
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
