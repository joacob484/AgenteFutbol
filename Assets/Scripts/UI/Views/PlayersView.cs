using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;

namespace AF.UI.Views
{
    public class PlayersView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject rowPrefab;
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

            if (rowPrefab == null)
            {
                var row = new GameObject("PlayerRowPrefab", typeof(RectTransform), typeof(Image));
                row.transform.SetParent(transform, false);
                row.SetActive(false);
                row.GetComponent<Image>().color = new Color(1,1,1,0.94f);
                _ = NewTMP(row.transform, "Line", "Jugador · OVR/POT · Club");
                rowPrefab = row;
            }

            if (emptyState == null)
            {
                emptyState = NewTMP(transform, "Empty", "Aún no representás a nadie.");
                (emptyState.transform as RectTransform).anchoredPosition = new Vector2(12, -8);
                emptyState.alignment = TextAlignmentOptions.Midline;
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
            var items = new List<string>();

            if (save?.Agent != null && save.World != null)
            {
                foreach (var id in save.Agent.RepresentedIds)
                {
                    if (!save.World.Players.TryGetValue(id, out var p)) continue;
                    var club = string.IsNullOrEmpty(p.ClubId) ? "Libre" :
                               (save.World.Clubs.TryGetValue(p.ClubId, out var c) ? c.Name : p.ClubId);
                    items.Add($"{p.Name} · {p.Pos} · OVR {p.Ovr}/POT {p.Potential} · {club}");
                }
            }

            emptyState.gameObject.SetActive(items.Count == 0);

            foreach (var line in items)
            {
                var row = Instantiate(rowPrefab, content);
                row.SetActive(true);
                var text = row.transform.Find("Line")?.GetComponent<TextMeshProUGUI>() ?? NewTMP(row.transform, "Line", "");
                text.text = line;
            }
        }
    }
}
