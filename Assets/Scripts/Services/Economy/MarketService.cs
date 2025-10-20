using System.Linq;
using AF.Core;
using AF.Services.World;

namespace AF.Services.Economy
{
    public static class MarketService
    {
        private const float COMMISSION_TRANSFER = 0.08f; // 8%

        // Helpers para acceder a la lista persistida
        private static System.Collections.Generic.List<MarketOfferDTO> List(SaveData s) => s.PendingOffers;

        public static int PendingCount(SaveData s) => s?.PendingOffers?.Count ?? 0;

        /// <summary>Llamar una vez por semana; si está dentro de ventana genera ofertas.</summary>
        public static void TickWeek(SaveData s)
        {
            if (s == null) return;
            if (!TimeService.IsInTransferWindow(s.Time.Week)) return;

            var repIds = s.Agent.RepresentedIds.ToList();
            var rng = new System.Random(s.Time.Tick);

            foreach (var pid in repIds)
            {
                if (!s.World.Players.TryGetValue(pid, out var p)) continue;
                if (string.IsNullOrEmpty(p.ClubId)) continue; // solo transferencias (jugador ya en club)

                // prob. base de oferta según OVR (60 → 5%, 100 → 65%)
                var pOffer = System.Math.Clamp((p.Ovr - 60) / 40.0, 0.05, 0.65);
                if (rng.NextDouble() > pOffer) continue;

                var from = s.World.Clubs[p.ClubId];
                var to = s.World.Clubs.Values
                    .Where(c => c.Id != from.Id && c.Budget > 300_000)
                    .OrderBy(_ => rng.Next()).FirstOrDefault();
                if (to == null) continue;

                var feeM = TransferEngine.EstimateFeeM(p, from, to);
                long fee = (long)(feeM * 1_000_000);
                long wage = (long)System.Math.Round((p.Ovr * 8_000) * (1.0 + (p.Age < 24 ? 0.2 : 0.0)));
                int years = rng.Next(2, 5);

                List(s).Add(new MarketOfferDTO
                {
                    PlayerId = pid,
                    FromClubId = from.Id,
                    ToClubId = to.Id,
                    Fee = fee,
                    YearlyWage = wage,
                    Years = years,
                    CommissionRate = COMMISSION_TRANSFER
                });

                NewsService.Add($"Oferta: {p.Name} de {from.Name} a {to.Name} por €{feeM:0.0}M · {years}a · Salario €{wage:N0}/año");
            }
        }

        public static bool Accept(SaveData s, int offerIndex, out string msg)
        {
            msg = null;
            if (s == null) { msg = "SaveData nulo."; return false; }
            if (offerIndex < 0 || offerIndex >= List(s).Count) { msg = "Índice de oferta inválido."; return false; }

            var o = List(s)[offerIndex];
            if (!s.World.Players.TryGetValue(o.PlayerId, out var p)) { msg = "Jugador no encontrado."; return false; }
            var from = s.World.Clubs[o.FromClubId];
            var to   = s.World.Clubs[o.ToClubId];
            if (to.Budget < o.Fee) { msg = "El club destino no tiene presupuesto suficiente."; return false; }

            // Mover jugador y presupuestos
            from.Squad.Remove(p.Id);
            to.Squad.Add(p.Id);
            p.ClubId = to.Id;
            to.Budget -= o.Fee;
            from.Budget += o.Fee;

            long commission = FinanceService.CommissionFromFee(o.Fee / 1_000_000.0, o.CommissionRate);
            FinanceService.AddMoney(s, commission, $"Comisión traspaso de {p.Name}");

            // Reputación/slots
            s.Agent.Reputation += 1;
            ReputationService.UpdateSlots(s.Agent, GameConfig.Balance);

            msg = $"{p.Name} traspasado a {to.Name} por €{o.Fee:N0}. Comisión: €{commission:N0}, +1 REP";
            NewsService.Add(msg);

            List(s).RemoveAt(offerIndex);
            AF.Services.Game.AutoSave.QueueSave();
            return true;
        }

        public static void Reject(SaveData s, int offerIndex)
        {
            if (s == null) return;
            if (offerIndex < 0 || offerIndex >= List(s).Count) return;

            List(s).RemoveAt(offerIndex);
            NewsService.Add("Oferta rechazada.");
        }
    }
}
