using System.Linq;
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
        [SerializeField] private TMP_InputField search;

        private void OnEnable() => Build();
        public void OnSearchChanged(string _) => Build();

        void Build()
        {
            foreach (Transform t in content) Destroy(t.gameObject);

            var s = GameRoot.Current;
            var q = (search?.text ?? "").Trim().ToLower();

            var list = s.Agent.RepresentedIds
                .Where(id => s.World.Players.ContainsKey(id))
                .Select(id => s.World.Players[id])
                .Where(p => string.IsNullOrEmpty(q) || p.Name.ToLower().Contains(q))
                .OrderByDescending(p => p.Ovr)
                .ToList();

            foreach (var p in list)
            {
                var go = Instantiate(rowPrefab, content);
                var ui = go.GetComponent<PlayerRowUI>();
                ui.Bind(p, OnRelease, OnTransfer, OnRenew);
            }
        }

        void OnRelease(Player p)
        {
            var s = GameRoot.Current;
            s.Agent.RepresentedIds.Remove(p.Id);
            AF.Services.World.NewsService.Add($"Dejaste de representar a {p.Name}.");
            Build();
        }

        void OnTransfer(Player p)
        {
            var ok = AF.Services.Game.ActionsService.ProposeTransfer(GameRoot.Current, p);
            if (!ok) AF.Services.World.NewsService.Add($"No se pudo transferir a {p.Name} (sin presupuesto o sin club).");
        }

        void OnRenew(Player p)
        {
            var ok = AF.Services.Game.ActionsService.RenewContract(GameRoot.Current, p);
            if (!ok) AF.Services.World.NewsService.Add($"No se pudo renovar a {p.Name} (debe tener club).");
        }
    }

    public class PlayerRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] TMP_Text clubTxt;
        [SerializeField] TMP_Text ovrTxt;
        [SerializeField] TMP_Text potTxt;
        [SerializeField] TMP_Text contractTxt;
        [SerializeField] Button   releaseBtn;
        [SerializeField] Button   transferBtn;
        [SerializeField] Button   renewBtn;

        Player _p;

        public void Bind(Player p,
            System.Action<Player> onRelease,
            System.Action<Player> onTransfer,
            System.Action<Player> onRenew)
        {
            _p = p;
            nameTxt.text = p.Name;
            clubTxt.text = string.IsNullOrEmpty(p.ClubId) ? "Libre" : p.ClubId;
            ovrTxt.text  = $"OVR {p.Ovr}";
            potTxt.text  = $"POT {p.Pot}";
            contractTxt.text = $"Contrato: {(p.ContractYears<=0 ? "N/A" : p.ContractYears+" años")}";

            releaseBtn.onClick.RemoveAllListeners();
            transferBtn.onClick.RemoveAllListeners();
            renewBtn.onClick.RemoveAllListeners();

            releaseBtn.onClick.AddListener(()=> onRelease?.Invoke(_p));
            transferBtn.onClick.AddListener(()=> onTransfer?.Invoke(_p));
            renewBtn.onClick.AddListener(()=> onRenew?.Invoke(_p));
        }
    }
}
