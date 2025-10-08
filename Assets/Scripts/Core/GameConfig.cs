using UnityEngine;

namespace AF.Core
{
    [System.Serializable]
    public class BalanceConfig
    {
        public int BaseSlots = 5;
        public int[] ReputationThresholds = new int[] { 10, 25, 50, 80, 120 };
        public int[] SlotsPerThreshold    = new int[] { 3, 4, 5, 6, 8 };

        public float CommissionTransfer = 0.12f; // 12%
        public float CommissionRenewal  = 0.05f; // 5%

        // Costos de instalaciones por nivel (1->2->3)
        public int StadiumL2CostM = 5;
        public int StadiumL3CostM = 20;
        public int AcademyL2CostM = 2;
        public int AcademyL3CostM = 8;
        public int MarketingL2CostM = 1;
        public int MarketingL3CostM = 5;
        public int MedicalL2CostM = 1;
        public int MedicalL3CostM = 4;
    }

    public static class GameConfig
    {
        public static BalanceConfig Balance { get; private set; }
        public static TextAsset NamesES { get; private set; }
        public static TextAsset Regions { get; private set; }

        public static void Load()
        {
            var bal = Resources.Load<TextAsset>("Data/balance");
            Balance = JsonUtility.FromJson<BalanceConfig>(bal.text);

            NamesES  = Resources.Load<TextAsset>("Data/names_es");
            Regions  = Resources.Load<TextAsset>("Data/regions");
        }
    }
}
