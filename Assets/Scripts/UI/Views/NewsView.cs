using UnityEngine;
using TMPro;

namespace AF.UI.Views
{
    public class NewsView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject itemPrefab;

        private void OnEnable() => Rebuild();

        public void Rebuild()
        {
            foreach (Transform t in content) Destroy(t.gameObject);
            foreach (var n in AF.Services.World.NewsService.All())
            {
                var go = Instantiate(itemPrefab, content);
                go.GetComponentInChildren<TMP_Text>().text = n;
            }
        }
    }
}
