using AF.Core;
using AF.Services.World;

namespace AF.Services.Economy
{
    public static class FacilitiesService
    {
        public static int GetNextCostM(FacilityType type, int currentLevel)
        {
            // Niveles: 1 -> 2 -> 3 (si ya es 3, 0 = no hay siguiente)
            if (currentLevel >= 3) return 0;
            var b = AF.Core.GameConfig.Balance;
            return type switch
            {
                FacilityType.Stadium  => currentLevel == 1 ? b.StadiumL2CostM  : b.StadiumL3CostM,
                FacilityType.Academy  => currentLevel == 1 ? b.AcademyL2CostM  : b.AcademyL3CostM,
                FacilityType.Marketing=> currentLevel == 1 ? b.MarketingL2CostM: b.MarketingL3CostM,
                FacilityType.Medical  => currentLevel == 1 ? b.MedicalL2CostM  : b.MedicalL3CostM,
                _ => 0
            };
        }

        public static int GetCurrentLevel(Club c, FacilityType type) =>
            type switch
            {
                FacilityType.Stadium   => c.Facilities.Stadium,
                FacilityType.Academy   => c.Facilities.Academy,
                FacilityType.Marketing => c.Facilities.Marketing,
                FacilityType.Medical   => c.Facilities.Medical,
                _ => 1
            };

        public static bool TryUpgrade(SaveData s, Club club, FacilityType type, out string message)
        {
            int current = GetCurrentLevel(club, type);
            int costM   = GetNextCostM(type, current);
            if (costM <= 0)
            {
                message = $"{club.Name}: {type} ya está al máximo.";
                return false;
            }

            long cost = costM * 1_000_000L;
            if (!FinanceService.TrySpend(s, cost, $"Mejora {type} en {club.Name}"))
            {
                message = "Fondos insuficientes.";
                return false;
            }

            // Subir nivel
            switch (type)
            {
                case FacilityType.Stadium:   club.Facilities.Stadium++;   break;
                case FacilityType.Academy:   club.Facilities.Academy++;   break;
                case FacilityType.Marketing: club.Facilities.Marketing++; break;
                case FacilityType.Medical:   club.Facilities.Medical++;   break;
            }

            int newLevel = GetCurrentLevel(club, type);

            // Efectos simples de ejemplo (pueden refinarse luego)
            if (type == FacilityType.Marketing)
                club.Budget += 250_000; // más ingresos base = más mercado
            if (type == FacilityType.Stadium)
                club.Budget += 500_000; // estadio mejor → +presupuesto

            message = $"{club.Name}: {type} mejorado a nivel {newLevel}.";
            NewsService.Add(message);
            return true;
        }
    }
}
