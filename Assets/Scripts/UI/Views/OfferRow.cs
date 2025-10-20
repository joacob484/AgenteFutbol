using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AF.UI.Views
{
    public class OfferRow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI titleTxt;
        [SerializeField] TextMeshProUGUI detailTxt;
        [SerializeField] Button acceptBtn;
        [SerializeField] Button rejectBtn;

        public void Bind(string title, string detail)
        {
            if (titleTxt)  titleTxt.text  = title;
            if (detailTxt) detailTxt.text = detail;
        }

        public void Wire(System.Action onAccept, System.Action onReject)
        {
            if (acceptBtn) acceptBtn.onClick.AddListener(() => onAccept?.Invoke());
            if (rejectBtn) rejectBtn.onClick.AddListener(() => onReject?.Invoke());
        }
    }
}
