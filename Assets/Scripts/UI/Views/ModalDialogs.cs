using UnityEngine;

namespace AF.UI.Views
{
    public class ModalDialogs : MonoBehaviour
    {
        public static void ShowInfo(string msg) => Debug.Log($"INFO: {msg}");
        public static void ShowError(string msg) => Debug.LogError(msg);
    }
}
