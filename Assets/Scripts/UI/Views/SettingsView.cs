using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Services.Save;

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
            backendTxt.text = $"Backend: {SaveService.BackendName}";
        }

        void OnStorageChanged(int index)
        {
            // Por ahora solo Local
            SaveService.SetBackend(new LocalBackend());
            backendTxt.text = $"Backend: {SaveService.BackendName}";
        }
    }
}
