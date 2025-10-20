using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;
using AF.Services.Economy;

namespace AF.UI.Views
{
    public class MarketOffersView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private TextMeshProUGUI emptyState;

        void OnEnable()
        {
            SafeWire();
            Rebuild();
        }

        void SafeWire()
        {
            if (content == null)
            {
                var root = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
                root.transform.SetParent(transform, false);
                var rt = root.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(1,1); rt.pivot = new Vector2(0.5f,1);
                var vlg = root.GetComponent<VerticalLayoutGroup>();
                vlg.spacing = 6; vlg.childForceExpandWidth = true;
                root.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                content = rt;
            }
            if (itemPrefab == null)
            {
                var row = new GameObject("OfferItemPrefab", typeof(RectTransform), typeof(Image));
                row.transform.SetParent(transform, false);
                row.SetActive(false);
                row.GetComponent<Image>().color = new Color(1,1,1,0.94f);
                _ = NewTMP(row.transform, "Title", "Oferta");
                _ = NewTMP(row.transform, "Detail", "Detalle", 16, new Vector2(10, -32));
                itemPrefab = row;
            }
            if (emptyState == null)
            {
                emptyState = NewTMP(transform, "Empty", "No hay ofertas.");
                emptyState.alignment = TextAlignmentOptions.Midline;
                (emptyState.transform as RectTransform).anchoredPosition = new Vector2(12, -8);
            }
        }

        TextMeshProUGUI NewTMP(Transform parent, string name, string txt, int size = 18, Vector2? pos = null)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos ?? new Vector2(10, -8);
            var t = go.GetComponent<TextMeshProUGUI>();
            t.font = TMP_Settings.defaultFontAsset; t.fontSize = size; t.color = Color.black; t.text = txt;
            return t;
        }

        public void Rebuild()
        {
            if (content == null) SafeWire();
            foreach (Transform c in content) Destroy(c.gameObject);

            var save = GameRoot.Current;
            var offers = MarketService.GetPendingOffers(save);

            emptyState.gameObject.SetActive(offers == null || offers.Count == 0);
            if (offers == null) return;

            foreach (var off in offers)
            {
                var row = Instantiate(itemPrefab, content);
                row.SetActive(true);
                var title  = row.transform.Find("Title")?.GetComponent<TextMeshProUGUI>() ?? NewTMP(row.transform, "Title", "");
                var detail = row.transform.Find("Detail")?.GetComponent<TextMeshProUGUI>() ?? NewTMP(row.transform, "Detail", "", 16, new Vector2(10, -32));

                title.text  = $"{off.PlayerName}: {off.FromClubName} → {off.ToClubName}";
                detail.text = $"Fee €{off.FeeM:0.0}M · Salario €{off.WageK:0}k/sem · {off.Years} años";
            }
        }
    }
}
