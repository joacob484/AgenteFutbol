using UnityEngine;
using TMPro;
using AF.Core;
using AF.Services.Economy;

namespace AF.UI.Views
{
    /// <summary>
    /// Muestra dinero, reputación y badge de ofertas pendientes.
    /// </summary>
    public class HeaderBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyTxt;
        [SerializeField] private TextMeshProUGUI repTxt;
        [SerializeField] private TextMeshProUGUI offersBadge;

        void OnEnable()
        {
            // Refresco ligero y periódico (barato)
            InvokeRepeating(nameof(Refresh), 0.1f, 0.5f);
        }

        void OnDisable()
        {
            CancelInvoke(nameof(Refresh));
        }

        public void Refresh()
        {
            var save = GameRoot.Current;
            if (save == null) return;

            if (moneyTxt != null)
                moneyTxt.text = $"€ {save.Agent.Money:N0}";

            if (repTxt != null)
                repTxt.text = $"REP {save.Agent.Reputation}";

            // Badge de ofertas pendientes
            if (offersBadge != null)
            {
                int count = MarketService.PendingOffersCount(save);
                if (count > 0)
                {
                    offersBadge.gameObject.SetActive(true);
                    offersBadge.text = $"Ofertas: {count}";
                }
                else
                {
                    offersBadge.gameObject.SetActive(false);
                }
            }
        }
    }
}
