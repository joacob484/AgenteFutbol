using System.Linq;
using UnityEngine;
using TMPro;
using AF.Core;
using AF.UI.Widgets;

namespace AF.UI.Views
{
    public class ClubsView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TMP_InputField search;

        private void OnEnable() => Build();
        public void OnSearchChanged(string _) => Build();

        void Build()
        {
            foreach (Transform t in content) Destroy(t.gameObject);
            var s = GameRoot.Current;
            var q = (search?.text ?? "").Trim().ToLower();

            var clubs = s.World.Clubs.Values
                .Where(c => string.IsNullOrEmpty(q) || c.Name.ToLower().Contains(q))
                .OrderByDescending(c => c.Reputation)
                .ThenByDescending(c => c.Budget)
                .ToList();

            foreach (var c in clubs)
            {
                var go = Instantiate(rowPrefab, content);
                var ui = go.GetComponent<ClubRowUI>();
                ui.Bind(c);
            }
        }
    }

    public class ClubRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text repTxt;
        [SerializeField] TMP_Text budTxt;
        [SerializeField] TMP_Text facTxt;
        [SerializeField] FacilityButtons facilityButtons; // ← asignar en el prefab

        Club _club;

        public void Bind(Club c)
        {
            _club = c;
            nameTxt.text = c.Name;
            repTxt.text  = $"Rep {c.Reputation}";
            budTxt.text  = $"Presupuesto € {c.Budget:N0}";
            var f = c.Facilities;
            facTxt.text  = $"Inst: Est {f.Stadium} · Acad {f.Academy} · Mkt {f.Marketing} · Med {f.Medical}";

            if (facilityButtons != null)
            {
                facilityButtons.Bind(c);
                facilityButtons.OnUpdated = Refresh;
            }
        }

        void Refresh()
        {
            if (_club == null) return;
            var f = _club.Facilities;
            budTxt.text = $"Presupuesto € { _club.Budget:N0}";
            facTxt.text = $"Inst: Est {f.Stadium} · Acad {f.Academy} · Mkt {f.Marketing} · Med {f.Medical}";
        }
    }
}
