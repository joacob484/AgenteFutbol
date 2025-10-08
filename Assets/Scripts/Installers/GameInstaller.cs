using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF.Installers
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField] string nextScene = "Main";
        void Awake()
        {
            if (!string.IsNullOrEmpty(nextScene))
                SceneManager.LoadScene(nextScene);
        }
    }
}
