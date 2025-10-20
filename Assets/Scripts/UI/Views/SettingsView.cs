using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AF.UI.Views
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown storageDropdown; // 0 Local (otros vendrán)
        [SerializeField] private TMP_Text backendTxt;

        void Start()
        {
            storageDropdown.ClearOptions();
            storageDropdown.AddOptions(new System.Collections.Generic.List<string> { "Local" });
            storageDropdown.onValueChanged.AddListener(OnStorageChanged);
            backendTxt.text = "Backend: Local";
        }

        void OnStorageChanged(int index)
        {
            // Por ahora solo Local
            backendTxt.text = "Backend: Local";
        }    
    }
}
