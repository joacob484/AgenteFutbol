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
            if (s == null) return;
            s.Agent.Money += amount;
            Log($"+ €{amount:N0}" + (string.IsNullOrEmpty(reason) ? "" : $" · {reason}"));
        }

        public static bool SpendMoney(SaveData s, long amount, string reason = null)
        {
            if (s == null) return false;
            if (s.Agent.Money < amount)
            {
                Log($"Bloqueado: saldo insuficiente para {reason} (necesita €{amount:N0})");
                return false;
            }
            s.Agent.Money -= amount;
            Log($"− €{amount:N0}" + (string.IsNullOrEmpty(reason) ? "" : $" · {reason}"));
            return true;
        }

        // 👇 Alias para compatibilidad con código existente (p.ej. FacilitiesService)
        public static bool TrySpend(SaveData s, long amount, string reason = null) =>
            SpendMoney(s, amount, reason);

        public static void LogWeeklySummary(SaveData s)
        {
            if (s == null) return;
            Log($"Semana {s.Time.Week}: Dinero €{s.Agent.Money:N0} · REP {s.Agent.Reputation}");
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
