using System.Linq;
using AF.Core;
using AF.Services.World;

namespace AF.Services.Economy
{
    public static class TransferEngine
    {
        public static double EstimateFeeM(Player p, Club from, Club to)
        {
            double baseM = p.Ovr * 0.6 + (p.Pot - p.Ovr) * 0.8;
            baseM *= (1.0 + (to.Reputation - from.Reputation) * 0.15);
            baseM *= (1.0 + (p.Age < 23 ? 0.35 : p.Age > 30 ? -0.25 : 0));
            return System.Math.Max(0.1, System.Math.Round(baseM, 1));
        }

        // Retorna noticia si hubo transferencia; null si no.
        public static string SimulateMarketTick(SaveData save)
        {
            if (save.World.Players.Count == 0) return null;
            var any = save.World.Players.Values.Where(p => p.ClubId != null).ToList();
            if (any.Count == 0) return null;

            var p = any[RandomService.Range(0, any.Count - 1)];
            var from = save.World.Clubs[p.ClubId];
            var clubs = save.World.Clubs.Values.Where(c => c.Id != p.ClubId).ToList();
            if (clubs.Count == 0) return null;

            var to = clubs[RandomService.Range(0, clubs.Count - 1)];
            var feeM = EstimateFeeM(p, from, to);

            from.Squad.Remove(p.Id);
            to.Squad.Add(p.Id);
            p.ClubId = to.Id;

            long fee = (long)(feeM * 1_000_000);
            to.Budget -= fee;
            from.Budget += fee;

            return $"{p.Name} pasa de {from.Name} a {to.Name} por €{feeM:0.0}M";
        }
    }
}
