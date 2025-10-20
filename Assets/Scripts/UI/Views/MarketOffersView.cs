using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AF.Core;
using AF.Services.Economy;
using AF.Services.World;

namespace AF.UI.Views
{
    public class MarketOffersView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab; // Debe tener OfferRow, 2 TMP_Text y 2 Buttons
        [SerializeField] private TMP_Text emptyState;

        private void OnEnable() => Rebuild();

        public void Rebuild()
        {
            var s = GameRoot.Current;
            foreach (Transform t in content) Destroy(t.gameObject);

            if (s == null || s.PendingOffers.Count == 0)
            {
                if (emptyState) emptyState.gameObject.SetActive(true);
                return;
            }
            if (emptyState) emptyState.gameObject.SetActive(false);

            for (int i = 0; i < s.PendingOffers.Count; i++)
            {
                var dto = s.PendingOffers[i];
                var go = Instantiate(itemPrefab, content);
                var row = go.GetComponent<OfferRow>();
                if (row != null) row.Bind(i, dto, OnAccept, OnReject);
            }
        }

        private void OnAccept(int idx)
        {
            if (MarketService.Accept(GameRoot.Current, idx, out var msg))
            {
                NewsService.Add(msg);
                Rebuild();
            }
            else
            {
                NewsService.Add(msg ?? "No se pudo aceptar la oferta.");
            }
        }

        private void OnReject(int idx)
        {
            MarketService.Reject(GameRoot.Current, idx);
            Rebuild();
        }
    }

    /// <summary> Script para el prefab de cada oferta. </summary>
    public class OfferRow : MonoBehaviour
    {
        [SerializeField] TMP_Text titleTxt;
        [SerializeField] TMP_Text detailTxt;
        [SerializeField] Button acceptBtn;
        [SerializeField] Button rejectBtn;

        private int _index;

        public void Bind(int index, MarketOfferDTO o, System.Action<int> onAccept, System.Action<int> onReject)
        {
            _index = index;

            var s = GameRoot.Current;
            var p    = s.World.Players[o.PlayerId];
            var from = s.World.Clubs[o.FromClubId];
            var to   = s.World.Clubs[o.ToClubId];

            if (titleTxt)  titleTxt.text  = $"{p.Name}: {from.Name} → {to.Name}";
            if (detailTxt) detailTxt.text = $"Fee €{o.Fee:N0} · {o.Years} años · Salario €{o.YearlyWage:N0}/año";

            if (acceptBtn)
            {
                acceptBtn.onClick.RemoveAllListeners();
                acceptBtn.onClick.AddListener(()=> onAccept?.Invoke(_index));
            }
            if (rejectBtn)
            {
                rejectBtn.onClick.RemoveAllListeners();
                rejectBtn.onClick.AddListener(()=> onReject?.Invoke(_index));
            }
        }
    }
}
