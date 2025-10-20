using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AF.UI.Util
{
    /// <summary>
    /// Proxy que invoca UIRouter.Go(string) por reflexión.
    /// Evita errores de compile por diferencias de namespace.
    /// </summary>
    public static class UIRouterProxy
    {
        // Posibles namespaces/clases donde puede estar tu UIRouter
        private static readonly string[] CandidateTypeNames = new[]
        {
            "AF.UI.Util.UIRouter",
            "AF.UI.UIRouter",
            "AF.UI.Views.UIRouter",
            "UIRouter" // por si quedó en global sin namespace
        };

        private static MethodInfo _goMethod;

        private static bool EnsureBound()
        {
            if (_goMethod != null) return true;

            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                foreach (var fullName in CandidateTypeNames)
                {
                    var t = asm.GetType(fullName, throwOnError: false, ignoreCase: false);
                    if (t == null) continue;

                    // Buscamos método estático público: void Go(string)
                    _goMethod = t.GetMethod("Go", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
                    if (_goMethod != null) return true;
                }
            }

            Debug.LogError("[UIRouterProxy] No encontré un tipo UIRouter con método estático Go(string). " +
                           "Verificá el namespace y el método en tu UIRouter.cs");
            return false;
        }

        /// <summary>Llama a UIRouter.Go(panelId) si existe.</summary>
        public static void Go(string panelId)
        {
            if (!EnsureBound()) return;
            try
            {
                _goMethod.Invoke(null, new object[] { panelId });
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UIRouterProxy] Error invocando Go(\"{panelId}\"): {ex}");
            }
        }
    }
}
