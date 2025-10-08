using System.Linq;
using AF.Core;
using AF.Services.World;
using AF.Services.Economy;

namespace AF.Services.Game
{
    public static class ActionsService
    {
        public static bool ProposeTransfer(SaveData s, Player p)
        {
            if (!s.Agent.RepresentedIds.Contains(p.Id)) return false;
            if (string.IsNullOrEmpty(p.ClubId)) return false;

            var from = s.World.Clubs[p.ClubId];
            var candidates = s.World.Clubs.Values.Where(c => c.Id != from.Id).ToList();
            if (candidates.Count == 0) return false;

            var to = candidates.OrderByDescending(c => c.Reputation * 1000000L + c.Budget).First();

            var feeM = AF.Services.Economy.TransferEngine.EstimateFeeM(p, from, to);
            long fee = (long)(feeM * 1_000_000);

            if (to.Budget < fee)
            {
                var richer = candidates.OrderByDescending(c => c.Budget).First();
                to = richer;
                feeM = AF.Services.Economy.TransferEngine.EstimateFeeM(p, from, to);
                fee  = (long)(feeM * 1_000_000);
                if (to.Budget < fee) return false;
            }

            from.Squad.Remove(p.Id);
            to.Squad.Add(p.Id);
            p.ClubId = to.Id;

            to.Budget -= fee;
            from.Budget += fee;

            var commission = FinanceService.CommissionFromFee(feeM, AF.Core.GameConfig.Balance.CommissionTransfer);
            FinanceService.AddMoney(s, commission, "Comisión por transferencia");

            var gain = AF.Services.Economy.ReputationService.GainOnTransfer(feeM, to.Reputation, metObjectives:false, p.Pos);
            s.Agent.Reputation += gain;
            AF.Services.Economy.ReputationService.UpdateSlots(s.Agent, AF.Core.GameConfig.Balance);

            var msg = $"Has transferido a {p.Name} de {from.Name} a {to.Name} por €{feeM:0.0}M (Comisión: €{commission:N0}, +{gain} REP)";
            NewsService.Add(msg);

            AutoSave.QueueSave(); // <-- nuevo
            return true;
        }

        public static bool RenewContract(SaveData s, Player p)
        {
            if (!s.Agent.RepresentedIds.Contains(p.Id)) return false;
            if (string.IsNullOrEmpty(p.ClubId)) return false;

            var club = s.World.Clubs[p.ClubId];

            long yearlyWage = (long)System.Math.Round(p.MarketValueM * 1_000_000 * 0.05);
            var commission  = FinanceService.CommissionFromRenewal(yearlyWage, AF.Core.GameConfig.Balance.CommissionRenewal);
            FinanceService.AddMoney(s, commission, "Comisión por renovación");

            p.ContractYears = System.Math.Clamp(p.ContractYears + 2, 1, 5);
            p.AgentAffinity = System.Math.Clamp(p.AgentAffinity + 0.05f, -1f, 1f);

            int repGain = 1;
            s.Agent.Reputation += repGain;
            AF.Services.Economy.ReputationService.UpdateSlots(s.Agent, AF.Core.GameConfig.Balance);

            var msg = $"Renovaste a {p.Name} en {club.Name}: salario anual €{yearlyWage:N0} (Comisión: €{commission:N0}, +{repGain} REP)";
            NewsService.Add(msg);

            AutoSave.QueueSave(); // <-- nuevo
            return true;
        }
    }
}
