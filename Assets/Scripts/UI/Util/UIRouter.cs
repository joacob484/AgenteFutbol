using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF.UI
{
    public class UIRouter : MonoBehaviour
    {
        // Singleton liviano para poder llamar UIRouter.Show("Panel") desde cualquier lado
        public static UIRouter Instance { get; private set; }

        [System.Serializable]
        public class Panel
        {
            public string name;
            public CanvasGroup group;
        }

        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private string defaultPanel = "MainMenu";
        [SerializeField] private List<Panel> panels = new();

        private readonly Dictionary<string, CanvasGroup> map = new();
        private string current;
        private Coroutine currentFx;

        private void Awake()
        {
            Instance = this;

            map.Clear();
            foreach (var p in panels)
            {
                if (p != null && p.group != null && !string.IsNullOrEmpty(p.name))
                {
                    map[p.name] = p.group;
                    p.group.alpha = 0f;
                    p.group.blocksRaycasts = false;
                    p.group.interactable = false;
                }
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(defaultPanel))
                Show(defaultPanel);
        }

        // Método de instancia
        public void Show(string panelName)
        {
            if (!map.ContainsKey(panelName))
            {
                Debug.LogWarning($"[UIRouter] Panel '{panelName}' no registrado.");
                return;
            }
            if (current == panelName) return;

            if (currentFx != null) StopCoroutine(currentFx);
            currentFx = StartCoroutine(Swap(panelName));
        }

        // Método estático (atajo) para scripts que llamen UIRouter.Show("...")
        public static void Show(string panelName, bool useStaticOverload)
        {
            Instance?.Show(panelName);
        }

        private IEnumerator Swap(string target)
        {
            CanvasGroup toHide = null;
            if (!string.IsNullOrEmpty(current) && map.TryGetValue(current, out var g))
                toHide = g;

            var toShow = map[target];

            if (toHide != null)
                yield return Fade(toHide, 0f, fadeDuration);

            yield return Fade(toShow, 1f, fadeDuration);
            current = target;
        }

        private static IEnumerator Fade(CanvasGroup group, float to, float duration)
        {
            if (group == null) yield break;
            float from = group.alpha;
            float t = 0f;
            bool interactableTarget = to > 0.99f;

            // bloquea clics durante el fade-out
            group.blocksRaycasts = interactableTarget;
            group.interactable = interactableTarget;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, t / duration);
                yield return null;
            }

            group.alpha = to;
            group.blocksRaycasts = interactableTarget;
            group.interactable = interactableTarget;
        }
    }
}
