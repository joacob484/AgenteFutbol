using System.Collections.Generic;
using UnityEngine;

namespace AF.UI.Dev
{
    /// <summary>
    /// Registro liviano de paneles para fallback si no hay UIRouter.
    /// </summary>
    public static class DevUIRegistry
    {
        private static readonly Dictionary<string, GameObject> _panels = new();
        private static Transform _root;

        public static void SetRoot(Transform t) => _root = t;

        public static void Register(string name, GameObject panel)
        {
            if (string.IsNullOrEmpty(name) || panel == null) return;
            _panels[name] = panel;
        }

        public static void Go(string name)
        {
            if (_root == null || !_panels.ContainsKey(name)) return;
            foreach (var kv in _panels)
                if (kv.Value) kv.Value.SetActive(false);
            _panels[name].SetActive(true);
        }
    }
}
