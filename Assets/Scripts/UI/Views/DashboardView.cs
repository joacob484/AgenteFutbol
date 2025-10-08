using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;

namespace AF.UI.Views
{
    public class DashboardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyTxt;
        [SerializeField] private TMP_Text repTxt;
        [SerializeField] private TMP_Text timeTxt;
        [SerializeField] private Button nextWeekBtn;
        [SerializeField] private NewsView newsView;     // arrastrar PanelNews
        [SerializeField] private PlayersView playersView; // opcional

        private void Start()
        {
            Refresh();
            nextWeekBtn.onClick.AddListener(() =>
            {
                AF.Services.World.TimeService.AdvanceWeek(GameRoot.Current);
                var n = AF.Services.Economy.TransferEngine.SimulateMarketTick(GameRoot.Current);
                AF.Services.World.NewsService.Add(n);
                Refresh();
                if (newsView != null) newsView.Rebuild();
                if (playersView != null && playersView.isActiveAndEnabled)
                    playersView.SendMessage("OnEnable"); // reconstruye lista
            });
        }

        void Refresh()
        {
            var s = GameRoot.Current;
            moneyTxt.text = $"€ {s.Agent.Money:N0}";
            repTxt.text   = $"Reputación: {s.Agent.Reputation} · Slots {s.Agent.RepresentedIds.Count}/{s.Agent.SlotLimit}";
            timeTxt.text  = $"Semana {s.Time.Week} · Temp {s.Time.Season}";
        }
    }
}
