using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;

namespace AF.UI.Widgets
{
    /// <summary>
    /// Conjunto de botones para mejorar instalaciones de un club (una fila).
    /// </summary>
    public class FacilityButtons : MonoBehaviour
    {
        [SerializeField] private Button stadiumBtn;
        [SerializeField] private Button academyBtn;
        [SerializeField] private Button marketingBtn;
        [SerializeField] private Button medicalBtn;

        [SerializeField] private TMP_Text stadiumTxt;
        [SerializeField] private TMP_Text academyTxt;
        [SerializeField] private TMP_Text marketingTxt;
        [SerializeField] private TMP_Text medicalTxt;

        private Club _club;
        public System.Action OnUpdated;

        public void Bind(Club club)
        {
            _club = club;
            stadiumBtn.onClick.RemoveAllListeners();
            academyBtn.onClick.RemoveAllListeners();
            marketingBtn.onClick.RemoveAllListeners();
            medicalBtn.onClick.RemoveAllListeners();

            stadiumBtn.onClick.AddListener(() => TryUp(FacilityType.Stadium));
            academyBtn.onClick.AddListener(() => TryUp(FacilityType.Academy));
            marketingBtn.onClick.AddListener(() => TryUp(FacilityType.Marketing));
            medicalBtn.onClick.AddListener(() => TryUp(FacilityType.Medical));

            RefreshTexts();
        }

        void TryUp(FacilityType type)
        {
            if (_club == null) return;
            string msg;
            var ok = AF.Services.Economy.FacilitiesService.TryUpgrade(GameRoot.Current, _club, type, out msg);
            AF.Services.World.NewsService.Add(msg);
            AF.Services.Game.AutoSave.QueueSave();
            RefreshTexts();
            OnUpdated?.Invoke();
        }

        void RefreshTexts()
        {
            if (_club == null) return;

            int s = _club.Facilities.Stadium;
            int a = _club.Facilities.Academy;
            int m = _club.Facilities.Marketing;
            int md= _club.Facilities.Medical;

            var costS  = AF.Services.Economy.FacilitiesService.GetNextCostM(FacilityType.Stadium, s);
            var costA  = AF.Services.Economy.FacilitiesService.GetNextCostM(FacilityType.Academy, a);
            var costM  = AF.Services.Economy.FacilitiesService.GetNextCostM(FacilityType.Marketing, m);
            var costMd = AF.Services.Economy.FacilitiesService.GetNextCostM(FacilityType.Medical, md);

            if (stadiumTxt)  stadiumTxt.text  = $"Estadio L{s} {(costS>0? $"→ €{costS}M":"MAX")}";
            if (academyTxt)  academyTxt.text  = $"Academia L{a} {(costA>0? $"→ €{costA}M":"MAX")}";
            if (marketingTxt)marketingTxt.text= $"Marketing L{m} {(costM>0? $"→ €{costM}M":"MAX")}";
            if (medicalTxt)  medicalTxt.text  = $"Médico L{md} {(costMd>0? $"→ €{costMd}M":"MAX")}";
        }
    }
}
