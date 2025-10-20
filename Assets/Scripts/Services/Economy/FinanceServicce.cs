using System.Collections.Generic;
using AF.Core;

namespace AF.Services.Economy
{
    /// <summary>
    /// Ledger simple + helpers de comisión y gasto/ingreso.
    /// </summary>
    public static class FinanceService
    {
        private static readonly List<FinanceEntry> _ledger = new();

        // ===== API que usa la UI =====
        public static List<FinanceEntry> GetLedger(SaveData save)
        {
            return new List<FinanceEntry>(_ledger);
        }

        public static void LogWeeklySummary(SaveData save)
        {
            if (save == null) return;
            AddEntry($"Semana {save.Time.Week}", 0);
        }

        // ===== API que usan otros servicios =====
        public static long CommissionFromFee(double feeM, float pct) =>
            (long)System.Math.Round(feeM * 1_000_000d * pct);

        public static long CommissionFromRenewal(long yearlyWage, float pct) =>
            (long)System.Math.Round(yearlyWage * pct);

        /// <summary> Suma dinero al agente y registra en ledger. </summary>
        public static void AddMoney(SaveData s, long amount, string label)
        {
            if (s == null || s.Agent == null) return;
            s.Agent.Money += amount;
            AddEntry(label, amount);
        }

        /// <summary> Intenta gastar dinero del agente. Registra el egreso si procede. </summary>
        public static bool TrySpend(SaveData s, long cost, string label)
        {
            if (s == null || s.Agent == null) return false;
            if (s.Agent.Money < cost) return false;
            s.Agent.Money -= cost;
            AddEntry(label, -cost);
            return true;
        }

        // ===== Interno =====
        private static void AddEntry(string label, long amount)
        {
            _ledger.Add(new FinanceEntry
            {
                Week   = GameRoot.Current?.Time?.Week ?? 0,
                Label  = label,
                Amount = amount
            });
            if (_ledger.Count > 200) _ledger.RemoveAt(0);
        }
    }

    public class FinanceEntry
    {
        public int Week;
        public string Label;
        public long Amount;
    }
}
