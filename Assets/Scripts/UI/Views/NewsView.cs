using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Services.World;

namespace AF.UI.Views
{
    public class NewsView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

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
                var row = new GameObject("NewsItemPrefab", typeof(RectTransform), typeof(TextMeshProUGUI));
                row.transform.SetParent(transform, false);
                row.SetActive(false);
                var t = row.GetComponent<TextMeshProUGUI>();
                t.font = TMP_Settings.defaultFontAsset; t.fontSize = 18; t.color = Color.black;
                t.text = "Noticia…";
                itemPrefab = row;
            }
        }

        public void Rebuild()
        {
            if (content == null) SafeWire();
            foreach (Transform c in content) Destroy(c.gameObject);

            List<string> feed = NewsService.GetFeed();

            if (feed == null || feed.Count == 0)
            {
                var row = Instantiate(itemPrefab, content);
                row.SetActive(true);
                row.GetComponent<TextMeshProUGUI>().text = "Sin novedades.";
                return;
            }

            foreach (var msg in feed)
            {
                var row = Instantiate(itemPrefab, content);
                row.SetActive(true);
                row.GetComponent<TextMeshProUGUI>().text = "• " + msg;
            }
        }
    }
}
