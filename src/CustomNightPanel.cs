using CustomNight.Util;
using CustomNight.Util.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CustomNight;

using static CustomNightManager;
using static UiUtil;

#if MELON
[RegisterTypeInIl2Cpp]
#endif

public class CustomNightPanel(IntPtr handle) : MonoBehaviour(handle)
{
    public static CustomNightPanel Instance { get; private set; }

    public Transform RootTransform => RootCanvas.transform;
    public Canvas RootCanvas { get; private set; }
    private CanvasScaler _canvasScalar;
    private GraphicRaycaster _canvasRayCaster;
    public Text TabTitle { get; private set; }

    public Tab CurrentTab { get; private set; } = Tab.General;
    public bool Shown { get; private set; } = false;
    public RectTransform Content { get; private set; }

    static private AudioClip _blip;

    public static void Init()
    {
        if (Instance) return;
        GameObject go = new GameObject("CustomNightPanel");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<CustomNightPanel>();
    }

    public void BuildUI()
    {

            
        RootCanvas = gameObject.AddComponent<Canvas>();
        _canvasScalar = gameObject.AddComponent<CanvasScaler>();
        _canvasRayCaster = gameObject.AddComponent<GraphicRaycaster>();
        
        RootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        RootCanvas.sortingOrder = 666;

        _canvasScalar.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasScalar.referenceResolution = new Vector2(1920, 1080);
        _canvasScalar.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        _canvasScalar.matchWidthOrHeight = 0.5f;
        
        Image background = RootTransform.CreateImage("BackGround");
        background.rectTransform.SetAnchors(new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f));
        background.color = new Color(0.2f,0.2f,0.2f);
        
        Content = RootTransform.CreateUIObject("TabContent");
        Content.SetAnchors(new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f),new Vector2(10,10),new Vector2(-10,-110));
        Content.gameObject.AddComponent<Image>().color = new Color(0f,0f,0f,0.6f);
        
        Image titleBackGround = background.transform.CreateImage("Tab Title Background", new Color(0f, 0f, 0f, 0.6f));
        titleBackGround.rectTransform.pivot = new Vector2(0.5f, 1);
        titleBackGround.rectTransform.SetAnchors(new Vector2(0, 1), new Vector2(1, 1), new Vector2(10, -100), new Vector2(-10, -10));

        TabTitle = titleBackGround.transform.CreateText("Tab Title", CurrentTab.ToString(),48);
        TabTitle.rectTransform.SetAnchors(Vector2.zero, Vector2.one); 
        TabTitle.alignment = TextAnchor.MiddleCenter;
        
        Button leftButton = titleBackGround.transform.CreateButton("Prev Button","<", 40);
                                                                  leftButton.GetComponent<RectTransform>().SetAnchors(new Vector2(0, 0), new Vector2(0, 1), new Vector2(10, 10), new Vector2(90, -10));
        leftButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);

        Button rightButton = titleBackGround.transform.CreateButton("Next Button",">", 40);
        rightButton.GetComponent<RectTransform>().SetAnchors(new Vector2(1, 0), new Vector2(1, 1), new Vector2(-90, 10), new Vector2(-10, -10));
        rightButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        
        
        leftButton.AddHoldRepeat(HeldTime => SwitchTab(false));
        rightButton.AddHoldRepeat(HeldTime => SwitchTab(true));
    }
    public void SwitchTab(bool next)
    {
        int count = Enum.GetValues(typeof(Tab)).Length;
        int index = (int)CurrentTab + (next ? 1 : -1);

        CurrentTab = (Tab)((index + count) % count);

        TabTitle.text = CurrentTab.ToString();
        if (_blip == null)
            _blip = ResourceUtil.GetEmbeddedAudioClip("CustomNight.assets.blip.wav", "BLIP");
        _blip.PlayAudioClip();
        RefreshContent(); 
    }

    public void RefreshContent()
    { 
        ClearContent();
        VerticalLayoutGroup Layout = Content.GetComponent<VerticalLayoutGroup>();
        if (Layout == null)
        {
            Layout = Content.gameObject.AddComponent<VerticalLayoutGroup>();
            Layout.padding = new RectOffset
            {
                left = 10,
                right = 10,
                top = 10,
                bottom = 10
            };
            Layout.spacing = 5;
            Layout.childAlignment = TextAnchor.UpperLeft;
            Layout.childControlWidth = true;
            Layout.childControlHeight = true;
            Layout.childForceExpandWidth = true;
            Layout.childForceExpandHeight = false;
        }

        if (CurrentTab == Tab.General)
        {
            BuildField(Content.transform, "Night minutes", () => NightSettings.NightTime,
                v => NightSettings.NightTime = v, 1, 360, 1);
            BuildField(Content.transform, "Grace period", () => NightSettings.GracePeriod,
                v => NightSettings.GracePeriod = v, 1, 30, 1);
            BuildField(Content.transform, "Inf energy", () => NightSettings.InfiniteEnergy,
                v => NightSettings.InfiniteEnergy = v, 0, 1, 1, true);
            BuildField(Content.transform, "Disable wordle", () => NightSettings.DisableWordle,
                v => NightSettings.DisableWordle = v, 0, 1, 1, true);
            BuildField(Content.transform, "Force janitor lms", () => NightSettings.ForceJanitorLms,
                v => NightSettings.ForceJanitorLms = v, 0, 1, 1, true);
            BuildField(Content.transform, "Endo roam early", () => NightSettings.EarlyFreeEndo,
                v => NightSettings.EarlyFreeEndo = v, 0, 1, 1, true);
        }
        else if (CurrentTab == Tab.Power)
        {
            BuildField(Content.transform, "Starting power", () => NightSettings.StartingPower,
                v => NightSettings.StartingPower = v, 0, 10000, 5);
            BuildField(Content.transform, "Max power", () => NightSettings.MaxPower, v => NightSettings.MaxPower = v, 0,
                10000, 5);
            BuildField(Content.transform, "Starting reserve", () => NightSettings.StartingReserveEnergy,
                v => NightSettings.StartingReserveEnergy = v, 0, 10000, 5);
        }
        else if (CurrentTab == Tab.AI)
        {
            for (int i = 0; i < AiUtil.AnimatronicIDs.Length; i++)
            {
                AnimatronicID id = AiUtil.AnimatronicIDs[i];
                int index = (int)id;
                BuildField(
                    Content.transform,
                    id.ToString() + " AI",
                    () => NightSettings.SelectedAiLevels[index],
                    v => NightSettings.SelectedAiLevels[index] = Mathf.RoundToInt(v),
                    -1,
                    100,
                    1);
            }
        }
        
    }

    public static void BuildField(Transform parent, string name, Func<float> get, Action<float> set,
        float min, float max, float increment, bool isBool = false, float rowHeight = 40)
    {
        RectTransform row = parent.CreateUIObject(name + " Row");
        row.gameObject.AddComponent<LayoutElement>().preferredHeight = rowHeight;


        Text label = row.CreateText("Label", name, 24);
        label.alignment = TextAnchor.MiddleLeft;
        label.rectTransform.SetAnchors(new Vector2(0,0),new Vector2(1,1));
        label.raycastTarget = false;

        Button minus = row.CreateButton("Minus", "-", 28);
        minus.image.color = Color.red;
        minus.GetComponent<RectTransform>()
            .SetAnchors(new Vector2(1, 0), new Vector2(1, 1), new Vector2(-190,0), new Vector2(-150,0));

        Text value = row.CreateText("Value", "", 24);
        value.alignment = TextAnchor.MiddleCenter;
        value.rectTransform.GetComponent<RectTransform>()
            .SetAnchors(new Vector2(1, 0), new Vector2(1, 1), new Vector2(-140,0), new Vector2(-60,0));

        Button plus = row.CreateButton("Plus", "+", 28);
        plus.GetComponent<RectTransform>()
            .SetAnchors(new Vector2(1, 0), new Vector2(1, 1), new Vector2(-50,0), new Vector2(-10,0));
        plus.image.color = Color.green;
        

        void UpdateValue()
        {
            float v = get();
            value.text = isBool ? (v == 1 ? "True" : "False") : v == -1 ? "Default" : v.ToString("0.##");
        }

        void Step(float heldTime, int dir)
        {
            int holdMultiplier = 1 << Math.Min((int)(heldTime / 0.5f), 6);
            MelonLogger.Msg(holdMultiplier);
            float baseStep = Input.GetKey(KeyCode.LeftShift) ? increment * 5 : increment;
            float step = baseStep * holdMultiplier;

            set(Mathf.Clamp(get() + dir * step, min, max));
            UpdateValue();

            if (_blip == null)
                _blip = ResourceUtil.GetEmbeddedAudioClip("CustomNight.assets.blip.wav", "BLIP");
            _blip.PlayAudioClip();
        }

        minus.AddHoldRepeat(HeldTime => Step( HeldTime,-1));
        plus.AddHoldRepeat(HeldTime => Step(HeldTime,+1));
        UpdateValue();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Lobby" || MultiplayerManager.Instance?.IsHost != true) Shown = false;
        RootCanvas.enabled = Shown;
        if (!Input.GetKeyDown(KeyCode.F2)) return;
        Shown = !Shown;
        RefreshContent();
    }

    private void Awake()
    {
        BuildUI();
    }

    public void ClearContent()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Transform child = Content.transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }


    public enum Tab
    {
        AI,
        General,
        Power
    }
}


