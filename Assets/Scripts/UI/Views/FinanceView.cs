using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;
using AF.Services.Economy;

namespace AF.UI.Views
{
    public class FinanceView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyTxt;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        void OnEnable()
        {
            SafeWire();
            Rebuild();
        }

        void SafeWire()
        {
            if (moneyTxt == null)
                moneyTxt = NewTMP(transform, "Money", "€ 0", 24, new Vector2(12, -8));

            if (content == null)
            {
                var root = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
                root.transform.SetParent(transform, false);
                var rt = root.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(1,1); rt.pivot = new Vector2(0.5f,1);
                rt.anchoredPosition = new Vector2(12, -48);
                var vlg = root.GetComponent<VerticalLayoutGroup>();
                vlg.spacing = 4; vlg.childForceExpandWidth = true;
                root.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                content = rt;
            }

            if (itemPrefab == null)
            {
                var row = new GameObject("LedgerItemPrefab", typeof(RectTransform), typeof(TextMeshProUGUI));
                row.transform.SetParent(transform, false);
                row.SetActive(false);
                var t = row.GetComponent<TextMeshProUGUI>();
                t.font = TMP_Settings.defaultFontAsset; t.fontSize = 18; t.color = Color.black;
                t.text = "fecha — concepto — monto";
                itemPrefab = row;
            }
        }

        TextMeshProUGUI NewTMP(Transform parent, string name, string txt, int size, Vector2 pos)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0,1); rt.anchorMax = new Vector2(0,1); rt.pivot = new Vector2(0,1);
            rt.anchoredPosition = pos;
            var t = go.GetComponent<TextMeshProUGUI>();
            t.font = TMP_Settings.defaultFontAsset; t.fontSize = size; t.text = txt; t.color = Color.black;
            return t;
        }

        public void Rebuild()
        {
            if (content == null) SafeWire();
            foreach (Transform c in content) Destroy(c.gameObject);

            var save = GameRoot.Current;
            long money = save?.Agent?.Money ?? 0;
            if (moneyTxt != null) moneyTxt.text = $"€ {money:N0}";

            List<FinanceEntry> ledger = FinanceService.GetLedger(save);

            if (ledger == null || ledger.Count == 0)
            {
                var row = Instantiate(itemPrefab, content);
                row.SetActive(true);
                row.GetComponent<TextMeshProUGUI>().text = "Sin movimientos.";
                return;
            }

            foreach (var e in ledger)
            {
                var row = Instantiate(itemPrefab, content);
                row.SetActive(true);
                row.GetComponent<TextMeshProUGUI>().text =
                    $"Sem {e.Week:00} — {e.Label} — {(e.Amount >= 0 ? "+" : "")}€{e.Amount:N0}";
            }
        }
    }
}
