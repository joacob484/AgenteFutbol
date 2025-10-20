using UnityEngine;
using AF.UI.Util; // usamos el proxy, no el UIRouter directo

namespace AF.UI.Views
{
    public class DashboardShortcuts : MonoBehaviour
    {
        [SerializeField] private string talentsPanel = "Talents";
        [SerializeField] private string playersPanel = "Players";
        [SerializeField] private string marketPanel  = "Market";
        [SerializeField] private string financePanel = "Finance";
        [SerializeField] private string newsPanel    = "News";

        public void GoTalents() => UIRouterProxy.Go(talentsPanel);
        public void GoPlayers() => UIRouterProxy.Go(playersPanel);
        public void GoMarket()  => UIRouterProxy.Go(marketPanel);
        public void GoFinance() => UIRouterProxy.Go(financePanel);
        public void GoNews()    => UIRouterProxy.Go(newsPanel);
    }
}
