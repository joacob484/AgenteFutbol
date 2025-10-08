using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using AF.Core;
using AF.Services.Save;
using AF.Services.World;

namespace AF.UI.Views
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;

        private async void Awake()
        {
            // Habilitar / deshabilitar "Continuar" según si existe el slot
            var slots = await SaveService.ListAsync();
            bool hasSlot1 = System.Array.Exists(slots, s => s == "slot1");
            if (continueButton != null) continueButton.interactable = hasSlot1;

            if (newGameButton != null)
                newGameButton.onClick.AddListener(NewGame);

            if (continueButton != null)
                continueButton.onClick.AddListener(async () =>
                {
                    var (ok, data) = await SaveService.LoadAsync("slot1");
                    if (ok) GameRoot.Current = data;
                    SceneManager.LoadScene("Main");
                });
        }

        private async void NewGame()
        {
            var save = new SaveData();

            // Config y mundo inicial
            GameConfig.Load();
            save.Agent.SlotLimit = GameConfig.Balance.BaseSlots;
            WorldGenerator.Generate(save);

            GameRoot.Current = save;
            await SaveService.SaveAsync("slot1", save);

            SceneManager.LoadScene("Main");
        }
    }
}
