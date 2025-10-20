using System.Collections.Generic;
using AF.Core;
using AF.Services.World;

namespace AF.Services.Economy
{
    /// <summary>
    /// Genera ofertas básicas en ventana y expone un buffer consultable por la UI.
    /// </summary>
    public static class MarketService
    {
        private static readonly List<MarketOffer> _offers = new();

        public static void TickWeek(SaveData save)
        {
            if (save == null || save.Time == null) return;
            if (!TimeService.IsInTransferWindow(save.Time.Week)) return;

            // Demo: usamos el TransferEngine para mover 1 jugador y además generamos una "oferta" visible.
            var news = TransferEngine.SimulateMarketTick(save);
            if (news != null) NewsService.Add(news);

            _offers.Add(new MarketOffer
            {
                PlayerName   = "Oferta demo",
                FromClubName = "Club A",
                ToClubName   = "Club B",
                FeeM   = 1.2,
                WageK  = 45,
                Years  = 3
            });

            if (_offers.Count > 25) _offers.RemoveAt(0);
        }

        public static List<MarketOffer> GetPendingOffers(SaveData save)
        {
            return new List<MarketOffer>(_offers);
        }

        // Para HeaderBar (badge)
        public static int PendingOffersCount(SaveData save) => _offers.Count;

        public static void Clear() => _offers.Clear();
    }

    public class MarketOffer
    {
        public string PlayerId;
        public string PlayerName;
        public string FromClubName;
        public string ToClubName;
        public double FeeM;
        public int WageK;
        public int Years;
    }
}
