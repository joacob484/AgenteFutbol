using UnityEngine;
using TMPro;
using AF.Core;
using AF.Services.Economy;

namespace AF.UI.Views
{
    public class HeaderBar : MonoBehaviour
    {
        [SerializeField] TMP_Text moneyTxt;
        [SerializeField] TMP_Text repTxt;

        // ⬇️ NUEVO (opcional): badge de ofertas pendientes
        [SerializeField] TMP_Text offersBadge;

        void OnEnable()  => InvokeRepeating(nameof(Refresh), 0f, 0.4f);
        void OnDisable() => CancelInvoke(nameof(Refresh));

        void Refresh()
        {
            var s = GameRoot.Current;
            if (s == null) return;

            if (moneyTxt) moneyTxt.text = $"€ {s.Agent.Money:N0}";
            if (repTxt)   repTxt.text   = $"REP {s.Agent.Reputation} · Slots {s.Agent.RepresentedIds.Count}/{s.Agent.SlotLimit}";

            if (offersBadge)
            {
                int n = MarketService.PendingCount(s);
                offersBadge.gameObject.SetActive(n > 0);
                if (n > 0) offersBadge.text = n.ToString();
            }
        }
    }
}
