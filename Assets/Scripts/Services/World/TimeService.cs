using AF.Core;
using AF.Services.Economy;

namespace AF.Services.World
{
    public static class TimeService
    {
        // Ventanas por semanas (1..52): 4–7 y 30–33
        public static bool IsInTransferWindow(int week) =>
            (week >= 4 && week <= 7) || (week >= 30 && week <= 33);

        /// <summary>
        /// Avanza una semana, registra resumen, genera ofertas (si aplica) y hace autosave.
        /// </summary>
        public static void AdvanceWeek(SaveData save)
        {
            if (save == null) return;

            save.Time.NextWeek();

            // Resumen semanal
            FinanceService.LogWeeklySummary(save);
            AF.Services.World.NewsService.Add(
                $"Cierre de semana {save.Time.Week} · REP {save.Agent.Reputation} · Representados {save.Agent.RepresentedIds.Count}"
            );

            // Mercado semanal
            AF.Services.Economy.MarketService.TickWeek(save);

            // 👇 Namespace correcto para AutoSave
            AF.Services.Game.AutoSave.QueueSave();
        }
    }
}
