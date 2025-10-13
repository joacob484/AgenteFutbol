using UnityEngine;
using TMPro;
using AF.Core;

namespace AF.UI.Views
{
    public class FinanceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text moneyTxt;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab; // un Text TMP simple

        private void OnEnable() => Rebuild();

        public void Rebuild()
        {
            var s = GameRoot.Current;
            if (moneyTxt != null) moneyTxt.text = $"Dinero: € {s.Agent.Money:N0}";

            foreach (Transform t in content) Destroy(t.gameObject);
            var ledger = AF.Services.Economy.FinanceService.Ledger;
            foreach (var line in ledger)
            {
                var go = Instantiate(itemPrefab, content);
                go.GetComponentInChildren<TMP_Text>().text = line;
            }
        }
    }
}
