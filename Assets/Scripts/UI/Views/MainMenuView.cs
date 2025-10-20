using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using AF.Core;
using AF.Services.World;
using AF.Services.Save;
using AF.UI;


namespace AF.UI.Views
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] Button newGameButton;
        [SerializeField] Button continueButton;

        async void Awake()
        {
            // Habilitar/deshabilitar "Continuar" si existe slot1
            bool hasSlot1 = await SaveService.ExistsAsync("slot1");
            continueButton.interactable = hasSlot1;

            newGameButton.onClick.AddListener(() => _ = NewGameFlow());
            continueButton.onClick.AddListener(() => _ = ContinueFlow());
        }

        async Task NewGameFlow()
        {
            // 1) Crear save
            var save = new SaveData();
            // slots base por si tu GameConfig no está aún
            if (save.Agent.SlotLimit <= 0) save.Agent.SlotLimit = 3;

            // 2) Generar mundo base
            WorldGenerator.Generate(save);

            // 3) Guardar en slot1 y setear estado global
            GameRoot.Current = save;
            await SaveService.SaveAsync("slot1", save);

            // 4) Ir al Dashboard (o mantenerte en Main y abrir panel con UIRouter)
            // si querés cambiar de escena, comenta la línea de router y usa SceneManager.LoadScene("Main");
            UIRouter.Go("Dashboard");  ;
        }

        async Task ContinueFlow()
        {
            var (ok, data) = await SaveService.LoadAsync("slot1");
            if (ok)
            {
                GameRoot.Current = data;
                UIRouter.Go("Dashboard");
            }
        }
    }
}
