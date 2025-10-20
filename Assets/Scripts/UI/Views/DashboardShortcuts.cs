using UnityEngine;
using AF.UI.Util; // UIRouterProxy
using AF.UI.Dev;  // DevUIRegistry

namespace AF.UI.Views
{
    public class DashboardShortcuts : MonoBehaviour
    {
        [SerializeField] private string talentsPanel = "Talents";
        [SerializeField] private string playersPanel = "Players"; // si lo sumás luego
        [SerializeField] private string marketPanel  = "Market";
        [SerializeField] private string financePanel = "Finance";
        [SerializeField] private string newsPanel    = "News";

        void Go(string id)
        {
            // 1) Intentar usar tu UIRouter real
            try { UIRouterProxy.Go(id); } catch { }
            // 2) Fallback dev (si no hay router)
            DevUIRegistry.Go(id);
        }

        public void GoTalents() => Go(talentsPanel);
        public void GoPlayers() => Go(playersPanel);
        public void GoMarket()  => Go(marketPanel);
        public void GoFinance() => Go(financePanel);
        public void GoNews()    => Go(newsPanel);
    }
}
