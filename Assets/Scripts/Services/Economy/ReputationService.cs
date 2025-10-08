using System;
using AF.Core;

namespace AF.Services.Economy
{
    public static class ReputationService
    {
        public static int GainOnTransfer(double feeM, int clubTier, bool metObjectives, Position pos)
        {
            double basePts = Math.Pow(Math.Max(0.1, feeM), 0.7) * Math.Max(1, clubTier);
            double rare = (pos is Position.GK or Position.FW) ? 1.1 : 1.0;
            double bonus = metObjectives ? 1.15 : 1.0;
            return (int)Math.Round(basePts * rare * bonus);
        }

        public static int PenaltyBadPractice(bool earlyContract, bool notTop3)
        {
            int p = 0;
            if (earlyContract) p += 6;
            if (notTop3) p += 6;
            return p; // 0..12
        }

        public static void UpdateSlots(Agent agent, BalanceConfig bal)
        {
            agent.SlotLimit = bal.BaseSlots;
            for (int i = 0; i < bal.ReputationThresholds.Length && i < bal.SlotsPerThreshold.Length; i++)
            {
                if (agent.Reputation >= bal.ReputationThresholds[i])
                    agent.SlotLimit += bal.SlotsPerThreshold[i];
            }
        }
    }
}
