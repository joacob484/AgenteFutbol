using UnityEngine;

namespace AF.UI.Dev
{
    /// <summary>
    /// Atajos rápidos para construir UI y diagnosticar en runtime.
    /// F1 = Build UI, F2 = Toggle Dashboard, F3 = Log Jerarquía.
    /// </summary>
    public class DevHotkeys : MonoBehaviour
    {
        AutoUIBootstrap _boot;

        void Awake() { _boot = FindObjectOfType<AutoUIBootstrap>(); }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (_boot == null) _boot = FindObjectOfType<AutoUIBootstrap>();
                if (_boot != null) _boot.BuildGameplayUI();
                else Debug.LogWarning("[DevHotkeys] No hay AutoUIBootstrap en escena.");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var dash = GameObject.Find("Dashboard");
                if (dash) dash.SetActive(!dash.activeSelf);
                Debug.Log("[DevHotkeys] Dashboard active = " + (dash ? dash.activeSelf.ToString() : "null"));
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var canvas = FindObjectOfType<Canvas>();
                if (canvas == null) { Debug.Log("[DevHotkeys] No Canvas"); return; }
                Debug.Log("=== Canvas children (orden de dibujo) ===");
                for (int i = 0; i < canvas.transform.childCount; i++)
                    Debug.Log(i + ": " + canvas.transform.GetChild(i).name);
            }
        }

        // Rótulo gigante para comprobar que la capa UI está arriba
        void OnGUI()
        {
            var dash = GameObject.Find("Dashboard");
            if (dash != null && dash.activeInHierarchy)
            {
                var style = new GUIStyle(GUI.skin.label) {
                    fontSize = 26, normal = { textColor = Color.black }
                };
                var rect = new Rect(20, 20, 400, 30);
                GUI.Label(rect, "UI ON (F2 toggle, F3 log)", style);
            }
        }
    }
}
