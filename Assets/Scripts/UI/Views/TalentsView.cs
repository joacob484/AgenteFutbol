using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AF.Core;
using AF.Services.World;

namespace AF.UI.Views
{
    public class TalentsView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TMP_Dropdown regionDropdown;

        List<Player> current = new();

        void OnEnable()
        {
            BuildDropdown();
            ShowRegion("Europa");
        }

        void BuildDropdown()
        {
            regionDropdown.ClearOptions();
            regionDropdown.AddOptions(new List<string>(RecruitmentService.GetRegions()));
            regionDropdown.onValueChanged.AddListener(i => ShowRegion(regionDropdown.options[i].text));
        }

        void ShowRegion(string region)
        {
            foreach (Transform t in content) Destroy(t.gameObject);
            current = RecruitmentService.RecruitBatch(region, 5);

            foreach (var p in current)
            {
                var go = Instantiate(rowPrefab, content);
                var ui = go.GetComponent<TalentRowUI>();
                ui.Bind(p);
            }
        }
    }

    public class TalentRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text ageTxt;
        [SerializeField] TMP_Text posTxt;
        [SerializeField] TMP_Text potTxt;

        Player _p;

        public void Bind(Player p)
        {
            _p = p;
            nameTxt.text = p.Name;
            ageTxt.text  = $"{p.Age} años";
            posTxt.text  = p.Pos.ToString();
            potTxt.text  = $"Pot {p.Potential}";
        }
    }
}
