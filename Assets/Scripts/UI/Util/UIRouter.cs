using System.Collections.Generic;
using UnityEngine;

namespace AF.UI.Util
{
    public class UIRouter : MonoBehaviour
    {
        [System.Serializable]
        public class Route
        {
            public string Name;       // "MainMenu", "Dashboard", "Talents", ...
            public GameObject Panel;  // PanelMainMenu, PanelDashboard, ...
        }

        [SerializeField] private List<Route> routes = new();
        private readonly Dictionary<string, GameObject> _map = new();

        void Awake()
        {
            foreach (var r in routes)
                if (r != null && r.Panel != null && !string.IsNullOrEmpty(r.Name))
                    _map[r.Name] = r.Panel;
        }

        public void Go(string routeName)
        {
            foreach (var kv in _map)
                kv.Value.SetActive(kv.Key == routeName);
        }

        [SerializeField] private string initialRoute = "MainMenu";
        void Start() => Go(initialRoute);
    }
}
