using UnityEngine;
using UnityEngine.UI;

namespace AF.UI.Dev
{
    /// <summary>
    /// Engancha el botón "Nuevo Juego" (y "Continuar" si existe) del MainMenu
    /// para construir la UI de juego cuando se inicia una partida.
    /// No interfiere con la lógica que ya tengas: se ejecuta además de tus listeners.
    /// </summary>
    public class NewGameBootstrapper : MonoBehaviour
    {
        [SerializeField] string newGamePath = "Canvas/MainMenu/MenuButtons/NewGameBtn";
        [SerializeField] string continuePath = "Canvas/MainMenu/MenuButtons/ContinueBtn";

        AutoUIBootstrap _bootstrap;

        void Awake()
        {
            _bootstrap = FindObjectOfType<AutoUIBootstrap>();
            Hook(newGamePath);
            Hook(continuePath);
        }

        void Hook(string path)
        {
            var go = GameObject.Find(path);
            if (!go) return;
            var btn = go.GetComponent<Button>();
            if (!btn) return;
            btn.onClick.AddListener(BuildUIAfterStart);
        }

        void BuildUIAfterStart()
        {
            // Dejar que tu lógica de "Nuevo Juego" corra primero; construir un frame después
            StartCoroutine(BuildNextFrame());
        }

        System.Collections.IEnumerator BuildNextFrame()
        {
            yield return null; // esperar un frame
            if (_bootstrap == null) _bootstrap = FindObjectOfType<AutoUIBootstrap>();
            if (_bootstrap != null) _bootstrap.BuildGameplayUI();
        }
    }
}
