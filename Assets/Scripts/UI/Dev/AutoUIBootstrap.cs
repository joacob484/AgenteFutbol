using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AF.Core;
using AF.Services.World;
using AF.Services.Economy;
using AF.UI.Views;
using AF.UI.Dev;

namespace AF.UI.Dev
{
    /// <summary>
    /// Construye en runtime: Header (Dinero/REP), Dashboard en bloques,
    /// y paneles Talents/Market/Finance/News/Players.
    /// Se puede invocar desde NewGameBootstrapper o manual (F1 si usás DevHotkeys).
    /// </summary>
    public class AutoUIBootstrap : MonoBehaviour
    {
        [Header("IDs de panel")]
        [SerializeField] string playersId = "Players";
        [SerializeField] string talentsId = "Talents";
        [SerializeField] string marketId  = "Market";
        [SerializeField] string financeId = "Finance";
        [SerializeField] string newsId    = "News";

        Transform _canvas;
        Transform _panelsRoot;

        // PALETA: fondo VERDE, cards BLANCAS, texto NEGRO
        static readonly Color ColBg     = new Color(0.18f, 0.50f, 0.18f); // verde
        static readonly Color ColCard   = Color.white;                   // tarjetas
        static readonly Color ColText   = Color.black;

        // ===== Ciclo de vida =====
        void Start()
        {
            // Si ya estamos dentro del juego y no hay UI, la construimos como fallback.
            if (GameObject.Find("MainMenu") == null && GameObject.Find("HeaderBar") == null)
            {
                Debug.Log("[AutoUIBootstrap] Start() → BuildGameplayUI (fallback)");
                BuildGameplayUI();
            }
        }

        public void BuildGameplayUI()
        {
            // Ocultar menú principal si existe
            var mainMenu = GameObject.Find("MainMenu");
            if (mainMenu) mainMenu.SetActive(false);

            // Canvas
            var canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = go.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var cs = go.GetComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = new Vector2(1280, 720);
            }
            _canvas = canvas.transform;

            // BACKDROP (fondo verde)
            var backdrop = GameObject.Find("Backdrop");
            if (!backdrop)
            {
                backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
                backdrop.transform.SetParent(_canvas, false);
                Stretch(backdrop.GetComponent<RectTransform>());
                var img = backdrop.GetComponent<Image>();
                img.color = ColBg;
                img.raycastTarget = false;
            }
            backdrop.transform.SetAsFirstSibling(); // al fondo

            // Carpeta Panels (contenedor de pantallas)
            var panelsGo = GameObject.Find("Panels");
            if (panelsGo == null)
            {
                panelsGo = new GameObject("Panels", typeof(RectTransform));
                panelsGo.transform.SetParent(_canvas, false);
                Stretch(panelsGo.GetComponent<RectTransform>());
            }
            _panelsRoot = panelsGo.transform;
            DevUIRegistry.SetRoot(_panelsRoot);

            // Header + paneles funcionales + Dashboard
            EnsureHeaderBar();
            EnsurePlayersPanel();
            EnsureTalentsPanel();
            EnsureMarketPanel();
            EnsureFinancePanel();
            EnsureNewsPanel();
            EnsureDashboardBlocks();

            // Forzar orden: Backdrop (0) → HeaderBar (arriba) → Panels
            var header = GameObject.Find("HeaderBar");
            if (header) header.transform.SetSiblingIndex(_canvas.childCount - 1);
            if (panelsGo) panelsGo.transform.SetSiblingIndex(_canvas.childCount - 2);

            // Mostrar dashboard
            var dash = GameObject.Find("Dashboard");
            if (dash) dash.SetActive(true);
            DevUIRegistry.Go("Dashboard");

            Debug.Log("[AutoUIBootstrap] Build OK.");
        }

        // ================== DASHBOARD EN BLOQUES ==================
        void EnsureDashboardBlocks()
        {
            if (GameObject.Find("Dashboard") != null) return;

            var dash = new GameObject("Dashboard", typeof(RectTransform));
            dash.transform.SetParent(_panelsRoot, false);
            Stretch(dash.GetComponent<RectTransform>());
            DevUIRegistry.Register("Dashboard", dash);
            dash.SetActive(true);

            // Layout según tu boceto: 2x2 + botón grande abajo
            float left = 40, top = -120, gutter = 24, colW = (1280 - left*2 - gutter) / 2f;
            float rowH = 180;

            var cardPlayers = NewCard(dash.transform, "Mis jugadores", "Gestioná tus representados",
                                      new Vector2(left, top), new Vector2(colW, rowH));
            cardPlayers.onClick.AddListener(()=> DevUIRegistry.Go(playersId));

            var cardTalents = NewCard(dash.transform, "Talentos", "Reclutá libres por región",
                                      new Vector2(left + colW + gutter, top), new Vector2(colW, rowH));
            cardTalents.onClick.AddListener(()=> DevUIRegistry.Go(talentsId));

            var cardInvest = NewCard(dash.transform, "Inversiones", "Próximamente",
                                     new Vector2(left, top - (rowH + gutter)), new Vector2(colW, rowH));
            cardInvest.interactable = false;

            var cardLeagues = NewCard(dash.transform, "Ligas", "Próximamente",
                                      new Vector2(left + colW + gutter, top - (rowH + gutter)), new Vector2(colW, rowH));
            cardLeagues.interactable = false;

            var cont = NewCard(dash.transform, "Continuar", "Avanzar una semana",
                               new Vector2(left, top - 2*(rowH + gutter)), new Vector2(colW*2 + gutter, 90));
            cont.onClick.AddListener(()=> TimeService.AdvanceWeek(GameRoot.Current));
        }

        // ================== HEADER ==================
        void EnsureHeaderBar()
        {
            if (GameObject.Find("HeaderBar") != null) return;

            var bar = NewTopStrip("HeaderBar", height: 72);
            var hb = bar.AddComponent<HeaderBar>();

            var money = NewTMP("Money", bar.transform, new Vector2(16, -14), 26);
            var rep   = NewTMP("Rep",   bar.transform, new Vector2(360, -14), 26);
            var badge = NewTMP("OffersBadge", bar.transform, new Vector2(620, -14), 22);
            badge.color = new Color(0.1f,0.1f,0.1f);
            badge.gameObject.SetActive(false);

            SetPrivateField(hb, "moneyTxt", money);
            SetPrivateField(hb, "repTxt", rep);
            SetPrivateField(hb, "offersBadge", badge);

            bar.SetActive(true);
        }

        // ================== PANELES FUNCIONALES ==================
        void EnsurePlayersPanel()
        {
            if (GameObject.Find(playersId) != null) return;

            var panel = NewPanel(playersId);
            DevUIRegistry.Register(playersId, panel);

            // Si tenés PlayersView real, lo cableamos (no falla si los campos no existen).
            var type = typeof(PlayersView);
            if (type != null)
            {
                var view = panel.AddComponent<PlayersView>();
                var (_, content) = NewScroll("List", panel.transform, new Vector2(12, -12), new Vector2(-12, 12));
                var empty = NewTMP("Empty", panel.transform, new Vector2(12, -12), 20, "Aún no representás a nadie.");
                empty.alignment = TextAlignmentOptions.Midline;

                SetPrivateField(view, "content", content);
                SetPrivateField(view, "emptyState", empty);
            }
            panel.SetActive(false);
        }

        void EnsureTalentsPanel()
        {
            if (GameObject.Find(talentsId) != null) return;

            var panel = NewPanel(talentsId);
            DevUIRegistry.Register(talentsId, panel);

            var view = panel.AddComponent<TalentsView>();

            var top = NewRow(panel.transform, "TopRow", 12, -8, width: 680, height: 40);
            var dd  = NewTMPDropdown("RegionDropdown", top);
            dd.GetComponent<TMP_Dropdown>().AddOptions(
                new System.Collections.Generic.List<string>{ "Europa", "Sudamérica", "Asia", "África" }
            );
            var gen = NewButton("Generar 3", top, new Vector2(240, -6), new Vector2(160, 28));

            var (_, content) = NewScroll("List", panel.transform, new Vector2(12, -56), new Vector2(-12, 12));
            var empty = NewTMP("Empty", panel.transform, new Vector2(12, -56), 20, "Generá 3 talentos…");
            empty.alignment = TextAlignmentOptions.Midline;

            var rowPrefab = NewTalentRowPrefab(panel.transform);

            SetPrivateField(view, "regionDropdown", dd.GetComponent<TMP_Dropdown>());
            SetPrivateField(view, "content", content);
            SetPrivateField(view, "rowPrefab", rowPrefab);
            SetPrivateField(view, "emptyState", empty);

            gen.onClick.AddListener(()=> view.GenerateThree());

            panel.SetActive(false);
        }

        void EnsureMarketPanel()
        {
            if (GameObject.Find(marketId) != null) return;

            var panel = NewPanel(marketId);
            DevUIRegistry.Register(marketId, panel);

            var view = panel.AddComponent<MarketOffersView>();
            var (_, content) = NewScroll("Offers", panel.transform, new Vector2(12, -8), new Vector2(-12, 12));
            var empty = NewTMP("Empty", panel.transform, new Vector2(12, -8), 20, "No hay ofertas.");
            empty.alignment = TextAlignmentOptions.Midline;

            var rowPrefab = NewOfferRowPrefab(panel.transform);

            SetPrivateField(view, "content", content);
            SetPrivateField(view, "itemPrefab", rowPrefab);
            SetPrivateField(view, "emptyState", empty);

            panel.SetActive(false);
        }

        void EnsureFinancePanel()
        {
            if (GameObject.Find(financeId) != null) return;

            var panel = NewPanel(financeId);
            DevUIRegistry.Register(financeId, panel);

            var view = panel.AddComponent<FinanceView>();
            var money = NewTMP("MoneyBig", panel.transform, new Vector2(12, -8), 26, "€ 0");

            var (_, content) = NewScroll("Ledger", panel.transform, new Vector2(12, -48), new Vector2(-12, 12));
            var itemPrefab = NewTMPPrefab(panel.transform, "LedgerItem");

            SetPrivateField(view, "moneyTxt", money);
            SetPrivateField(view, "content", content);
            SetPrivateField(view, "itemPrefab", itemPrefab);

            panel.SetActive(false);
        }

        void EnsureNewsPanel()
        {
            if (GameObject.Find(newsId) != null) return;

            var panel = NewPanel(newsId);
            DevUIRegistry.Register(newsId, panel);

            var view = panel.AddComponent<NewsView>();
            var (_, content) = NewScroll("Feed", panel.transform, new Vector2(12, -8), new Vector2(-12, 12));
            var itemPrefab = NewTMPPrefab(panel.transform, "NewsItem");

            SetPrivateField(view, "content", content);
            SetPrivateField(view, "itemPrefab", itemPrefab);

            panel.SetActive(false);
        }

        // ================== HELPERS UI ==================
        GameObject NewTopStrip(string name, float height, float anchoredY = 0)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(_canvas, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(0, anchoredY);
            rt.sizeDelta = new Vector2(0, height);
            var img = go.GetComponent<Image>();
            img.color = ColCard; // tira una franja blanca
            AddOutline(go, 1f);
            go.SetActive(true);
            return go;
        }

        GameObject NewPanel(string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            go.transform.SetParent(_panelsRoot, false);
            Stretch(go.GetComponent<RectTransform>());
            go.GetComponent<Image>().color = new Color(1,1,1,0); // transparente (panel contenedor)
            go.SetActive(true);
            return go;
        }

        Button NewCard(Transform parent, string title, string subtitle, Vector2 topLeft, Vector2 size)
        {
            var go = new GameObject("Card_" + title, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.anchoredPosition = topLeft;
            rt.sizeDelta = size;

            var img = go.GetComponent<Image>();
            img.color = ColCard;
            AddOutline(go, 2f);

            _ = NewTMP("Title", go.transform, new Vector2(16, -12), 26, title);
            _ = NewTMP("Sub",   go.transform, new Vector2(16, -44), 18, subtitle);

            go.SetActive(true);
            return go.GetComponent<Button>();
        }

        (GameObject, RectTransform) NewScroll(string name, Transform parent, Vector2 topLeft, Vector2 bottomRight)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect));
            root.transform.SetParent(parent, false);
            var img = root.GetComponent<Image>();
            img.color = ColCard;
            AddOutline(root, 1.5f);

            var rt = root.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot     = new Vector2(0, 1);
            rt.offsetMin = new Vector2(topLeft.x, bottomRight.y);
            rt.offsetMax = new Vector2(bottomRight.x, topLeft.y);

            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content.transform.SetParent(root.transform, false);

            var crt = content.GetComponent<RectTransform>();
            crt.anchorMin = new Vector2(0, 1);
            crt.anchorMax = new Vector2(1, 1);
            crt.pivot     = new Vector2(0.5f, 1f);
            crt.anchoredPosition = Vector2.zero;

            var vlg = content.GetComponent<VerticalLayoutGroup>();
            vlg.spacing = 6; vlg.childForceExpandWidth = true;

            var fit = content.GetComponent<ContentSizeFitter>();
            fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var sr = root.GetComponent<ScrollRect>();
            sr.content = crt; sr.horizontal = false;

            root.SetActive(true);
            return (root, crt);
        }

        TMP_FontAsset DefaultTMPFont()
        {
            // Usa la fuente por defecto configurada en TMP Settings (Editor → Window → TextMeshPro → Project Settings)
            return TMP_Settings.defaultFontAsset;
        }

        TMP_Text NewTMP(string name, Transform parent, Vector2 pos, int size, string txt = "")
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.anchoredPosition = pos;

            var t = go.GetComponent<TextMeshProUGUI>();
            if (DefaultTMPFont() != null) t.font = DefaultTMPFont();
            t.fontSize = size;
            t.text = txt;
            t.color = ColText;
            t.raycastTarget = false;

            go.SetActive(true);
            return t;
        }

        GameObject NewTMPPrefab(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            go.SetActive(false);
            var t = go.GetComponent<TextMeshProUGUI>();
            if (DefaultTMPFont() != null) t.font = DefaultTMPFont();
            t.fontSize = 18;
            t.text = "Item";
            t.color = ColText;
            return go;
        }

        Button NewButton(string label, Transform parent, Vector2 pos, Vector2? size = null)
        {
            var go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.anchoredPosition = pos;
            rt.sizeDelta = size ?? new Vector2(140, 28);

            var img = go.GetComponent<Image>();
            img.color = ColCard;
            AddOutline(go, 1.5f);

            _ = NewTMP("Text", go.transform, new Vector2(10, -6), 18, label);

            go.SetActive(true);
            return go.GetComponent<Button>();
        }

        RectTransform NewRow(Transform parent, string name, float left, float top, float width = 680, float height = 40)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(left, top);
            rt.sizeDelta = new Vector2(width, height);
            go.SetActive(true);
            return rt;
        }

        GameObject NewTMPDropdown(string name, Transform parent)
        {
            // Dropdown mínimo (solo etiqueta visible para MVP).
            var go = new GameObject(name, typeof(RectTransform), typeof(TMP_Dropdown));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot     = new Vector2(0, 1);
            rt.sizeDelta = new Vector2(220, 28);

            var label = NewTMP("Label", go.transform, new Vector2(8, -6), 18, "Europa");
            var dd = go.GetComponent<TMP_Dropdown>();
            dd.captionText = label;
            dd.template = null; // sin lista desplegable (MVP)

            go.SetActive(true);
            return go;
        }

        // ---- Prefabs “por código” ----
        GameObject NewTalentRowPrefab(Transform parent)
        {
            var row = new GameObject("TalentRowPrefab", typeof(RectTransform), typeof(Image), typeof(Button), typeof(TalentRow));
            row.transform.SetParent(parent, false);
            row.SetActive(false);
            row.GetComponent<Image>().color = ColCard;
            AddOutline(row, 1.5f);

            var nameT = NewTMP("Name", row.transform, new Vector2(10, -8), 20, "Nombre");
            var infoT = NewTMP("Info", row.transform, new Vector2(10, -32), 16, "POS | OVR/POT | Edad");
            var btn = NewButton("Representar", row.transform, new Vector2(500, -8), new Vector2(140, 30));

            var tr = row.GetComponent<TalentRow>();
            SetPrivateField(tr, "nameTxt", nameT);
            SetPrivateField(tr, "ovrPotTxt", infoT);
            SetPrivateField(tr, "ageTxt", NewTMP("HiddenAge", row.transform, new Vector2(0,0), 1, ""));
            SetPrivateField(tr, "posTxt", NewTMP("HiddenPos", row.transform, new Vector2(0,0), 1, ""));
            SetPrivateField(tr, "clubTxt", NewTMP("HiddenClub", row.transform, new Vector2(0,0), 1, ""));

            btn.onClick.AddListener(() => tr.OnClickRepresent());
            return row;
        }

        GameObject NewOfferRowPrefab(Transform parent)
        {
            var row = new GameObject("OfferRowPrefab", typeof(RectTransform), typeof(Image), typeof(OfferRow));
            row.transform.SetParent(parent, false);
            row.SetActive(false);
            row.GetComponent<Image>().color = ColCard;
            AddOutline(row, 1.5f);

            var title  = NewTMP("Title",  row.transform, new Vector2(10, -8), 20, "Jugador: Origen → Destino");
            var detail = NewTMP("Detail", row.transform, new Vector2(10, -32), 16, "Fee/Salario/Años");

            var accBtn = NewButton("Aceptar",  row.transform, new Vector2(500, -8), new Vector2(100, 30));
            var rejBtn = NewButton("Rechazar", row.transform, new Vector2(610, -8), new Vector2(100, 30));

            var or = row.GetComponent<OfferRow>();
            SetPrivateField(or, "titleTxt",  title);
            SetPrivateField(or, "detailTxt", detail);
            SetPrivateField(or, "acceptBtn", accBtn);
            SetPrivateField(or, "rejectBtn", rejBtn);

            return row;
        }

        // ===== Utils =====
        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }

        static void AddOutline(GameObject go, float size)
        {
            var ol = go.GetComponent<Outline>();
            if (ol == null) ol = go.AddComponent<Outline>();
            ol.effectColor = new Color(0,0,0,0.16f);
            ol.effectDistance = new Vector2(size, -size);
        }

        static void SetPrivateField(object obj, string fieldName, object value)
        {
            if (obj == null) return;
            var f = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (f != null) f.SetValue(obj, value);
        }
    }
}
