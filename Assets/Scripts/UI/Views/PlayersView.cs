using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AF.Core;
using AF.Services.World;
using AF.Services.Game;     // ActionsService
using UnityEngine.UI;

namespace AF.UI.Views
{
    /// <summary>
    /// Lista de jugadores representados con acciones por fila.
    /// Requiere: content (Scroll/Vertical), rowPrefab con PlayerRow.
    /// </summary>
    public class PlayersView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private TMP_Text emptyState;

        private readonly List<Player> _cache = new();

        private void OnEnable() => Rebuild();

        public void Rebuild()
        {
            var s = GameRoot.Current;
            foreach (Transform t in content) Destroy(t.gameObject);
            _cache.Clear();

            if (s == null || s.Agent.RepresentedIds.Count == 0)
            {
                if (emptyState) emptyState.gameObject.SetActive(true);
                return;
            }
            if (emptyState) emptyState.gameObject.SetActive(false);

            foreach (var pid in s.Agent.RepresentedIds)
            {
                if (!s.World.Players.TryGetValue(pid, out var p)) continue;
                _cache.Add(p);

                var go = Instantiate(rowPrefab, content);
                var row = go.GetComponent<PlayerRow>();
                if (row != null) row.Bind(p, OnProposeTransfer);
            }
        }

        private void OnProposeTransfer(Player p)
        {
            var s = GameRoot.Current;
            if (s == null || p == null) return;

            var ok = ActionsService.ProposeTransfer(s, p);
            if (!ok)
            {
                AF.Services.World.NewsService.Add($"No se pudo proponer traspaso para {p.Name}.");
            }
            // El propio ActionsService ya agrega News y AutoSave si corresponde.
            // Refrescamos por si se movió de club u ocurrió algo.
            Rebuild();
        }
    }

    /// <summary>
    /// Script para el prefab de fila. Debe tener:
    /// - TMP_Text: nameTxt, clubTxt, ovrPotTxt, ageTxt
    /// - Button: transferBtn (Proponer traspaso)
    /// </summary>
    public class PlayerRow : MonoBehaviour
    {
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text clubTxt;
        [SerializeField] TMP_Text ovrPotTxt;
        [SerializeField] TMP_Text ageTxt;
        [SerializeField] Button transferBtn;

        private Player _p;
        private System.Action<Player> _onTransfer;

        public void Bind(Player p, System.Action<Player> onTransfer)
        {
            _p = p;
            _onTransfer = onTransfer;

            var s = GameRoot.Current;
            string clubName = "-";
            if (!string.IsNullOrEmpty(p.ClubId) && s.World.Clubs.TryGetValue(p.ClubId, out var c))
                clubName = c.Name;

            if (nameTxt)   nameTxt.text   = p.Name;
            if (clubTxt)   clubTxt.text   = clubName;
            if (ovrPotTxt) ovrPotTxt.text = $"OVR {p.Ovr} / POT {p.Potential}";
            if (ageTxt)    ageTxt.text    = $"{p.Age}";

            if (transferBtn)
            {
                transferBtn.onClick.RemoveAllListeners();
                transferBtn.onClick.AddListener(() => _onTransfer?.Invoke(_p));
            }
        }
    }
}
