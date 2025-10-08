using System.Collections.Generic;
using AF.Core;

namespace AF.Services.Economy
{
    public static class FinanceService
    {
        private static readonly List<string> _ledger = new();
        public static IReadOnlyList<string> Ledger => _ledger.AsReadOnly();

        private static void Log(string text)
        {
            if (!string.IsNullOrEmpty(text)) _ledger.Insert(0, text);
        }

        public static void AddMoney(SaveData s, long amount, string reason = null)
        {
            if (amount == 0) return;
            s.Agent.Money += amount;
            Log($"Ingreso: €{amount:N0}" + (string.IsNullOrEmpty(reason) ? "" : $" ({reason})"));
        }

        public static bool TrySpend(SaveData s, long amount, string reason)
        {
            if (amount < 0) amount = -amount;
            if (s.Agent.Money < amount) return false;
            s.Agent.Money -= amount;
            Log($"Gasto: €{amount:N0}" + (string.IsNullOrEmpty(reason) ? "" : $" ({reason})"));
            return true;
        }

        public static long CommissionFromFee(double feeM, float commissionRate)
        {
            var gross = (long)(feeM * 1_000_000.0);
            return (long)System.Math.Round(gross * commissionRate);
        }

        public static long CommissionFromRenewal(long yearlyWage, float commissionRate)
        {
            return (long)System.Math.Round(yearlyWage * commissionRate);
        }
    }
}
