using UnityEngine;
using TMPro;
using AF.Core;      // ⬅️ agregar esta línea

namespace AF.UI.Views
{
    public class HeaderBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyTxt;
        [SerializeField] private TMP_Text repTxt;

        void OnEnable()  => InvokeRepeating(nameof(Refresh), 0f, 0.5f);
        void OnDisable() => CancelInvoke(nameof(Refresh));

        void Refresh()
        {
            if (GameRoot.Current == null) return;
            moneyTxt.text = $"€ {GameRoot.Current.Agent.Money:N0}";
            repTxt.text = $"REP {GameRoot.Current.Agent.Reputation} · " +
                          $"Slots {GameRoot.Current.Agent.RepresentedIds.Count}/{GameRoot.Current.Agent.SlotLimit}";
        }
    }
}
