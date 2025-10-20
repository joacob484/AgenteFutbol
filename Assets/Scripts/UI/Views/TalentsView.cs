using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AF.Core;
using AF.Services.World;
using AF.Services.Economy;

namespace AF.UI.Views
{
    public class TalentsView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TMP_Dropdown regionDropdown;
        [SerializeField] private TMP_Text emptyState;

        private readonly List<Player> _current = new();

        void Start()
        {
            // Poblar regiones
            if (regionDropdown != null)
            {
                regionDropdown.ClearOptions();
                var opts = new List<TMP_Dropdown.OptionData>();
                foreach (var r in RecruitmentService.GetRegions())
                    opts.Add(new TMP_Dropdown.OptionData(r));
                regionDropdown.AddOptions(opts);
                regionDropdown.onValueChanged.AddListener(_ => GenerateThree());
            }

            GenerateThree();
        }

        public void GenerateThree()
        {
            var region = regionDropdown != null && regionDropdown.options.Count > 0
                ? regionDropdown.options[regionDropdown.value].text
                : "Europa";

            _current.Clear();
            _current.AddRange(RecruitmentService.RecruitBatch(region, 3));
            RebuildList();
        }

        private void RebuildList()
        {
            foreach (Transform t in content) Destroy(t.gameObject);

            if (emptyState != null) emptyState.gameObject.SetActive(_current.Count == 0);

            foreach (var p in _current)
            {
                var go = Instantiate(rowPrefab, content);
                var row = go.GetComponent<TalentRow>();
                if (row != null) row.Bind(p, OnRepresent);
            }
        }

        private void OnRepresent(Player p)
        {
            var s = GameRoot.Current;
            if (s == null || p == null) return;
            if (s.Agent.RepresentedIds.Contains(p.Id)) return;

            s.Agent.RepresentedIds.Add(p.Id);
            s.Agent.Reputation += 1; // reputación ligera
            ReputationService.UpdateSlots(s.Agent, GameConfig.Balance);

            NewsService.Add($"Ahora representás a {p.Name} ({p.Pos}) OVR {p.Ovr}/POT {p.Potential}.");
            AF.Services.Game.AutoSave.QueueSave();

            _current.Remove(p);
            RebuildList();
        }
    }

    /// <summary> Script para el prefab de fila (rowPrefab) de TalentsView. </summary>
    public class TalentRow : MonoBehaviour
    {
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text ageTxt;
        [SerializeField] TMP_Text posTxt;
        [SerializeField] TMP_Text ovrPotTxt;
        [SerializeField] TMP_Text clubTxt;

        private Player _p;
        private System.Action<Player> _onRepresent;

        public void Bind(Player p, System.Action<Player> onRepresent)
        {
            _p = p;
            _onRepresent = onRepresent;

            if (nameTxt)   nameTxt.text = p.Name;
            if (ageTxt)    ageTxt.text  = $"{p.Age} años";
            if (posTxt)    posTxt.text  = p.Pos.ToString();
            if (ovrPotTxt) ovrPotTxt.text = $"OVR {p.Ovr}  POT {p.Potential}";
            if (clubTxt)   clubTxt.text = string.IsNullOrEmpty(p.ClubId) ? "Libre" : p.ClubId;
        }

        // Asignar este método al botón "Representar" del prefab
        public void OnClickRepresent()
        {
            _onRepresent?.Invoke(_p);
        }
    }
}
