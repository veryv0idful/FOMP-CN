using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
    private RectTransform _content;

    public Tab CurrentTab { get; private set; } = Tab.General;
    public bool Shown { get; private set; } = false;

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
        
        Image background = RootTransform.CreateImageChildObject("BackGround");
        background.rectTransform.SetAnchors(new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f));

        
        background.color = Color.gray;
        background.sprite = ResourceUtil.LoadEmbeddedSprite("CustomNight.assets.Zap.png");
        
        Image titleBackGround = background.transform.CreateImageChildObject("Tab Title Background", new Color(0f, 0f, 0f, 0.6f));
        titleBackGround.rectTransform.pivot = new Vector2(0.5f, 1);
        titleBackGround.rectTransform.SetAnchors(new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -100), new Vector2(0, 0));

        TabTitle = titleBackGround.transform.CreateTextChildObject("Tab Title", CurrentTab.ToString(),48);
        TabTitle.rectTransform.SetAnchors(Vector2.zero, Vector2.one); 
        TabTitle.alignment = TextAnchor.MiddleCenter;
        
        
        Button leftButton = titleBackGround.transform.CreateButtonChildObject("Prev Button");
        leftButton.GetComponent<RectTransform>().SetAnchors(new Vector2(0, 0), new Vector2(0, 1), new Vector2(10, 10), new Vector2(90, -10));
        leftButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        Text leftLabel = leftButton.transform.CreateTextChildObject("Label", "<", 40);
        leftLabel.rectTransform.SetAnchors(Vector2.zero, Vector2.one);
        leftLabel.alignment = TextAnchor.MiddleCenter;

        Button rightButton = titleBackGround.transform.CreateButtonChildObject("Next Button");
        rightButton.GetComponent<RectTransform>().SetAnchors(new Vector2(1, 0), new Vector2(1, 1), new Vector2(-90, 10), new Vector2(-10, -10));
        rightButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        Text rightLabel = rightButton.transform.CreateTextChildObject("Label", ">", 40);
        rightLabel.rectTransform.SetAnchors(Vector2.zero, Vector2.one);
        rightLabel.alignment = TextAnchor.MiddleCenter;


        Action prev = () => SwitchTab(false);
        Action next = () => SwitchTab(true);

        leftButton.onClick.AddListener(prev);
        rightButton.onClick.AddListener(next);
    }
    public void SwitchTab(bool next)
    {
        int count = Enum.GetValues(typeof(Tab)).Length;
        int index = (int)CurrentTab + (next ? 1 : -1);

        CurrentTab = (Tab)((index + count) % count);

        TabTitle.text = CurrentTab.ToString();
        ResourceUtil.GetEmbeddedAudioClip("CustomNight.assets.blip.wav","BLIP").PlayAudioClip();
        Refresh(); 
    }

    public void Refresh()
    {
        
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Lobby" || MultiplayerManager.Instance?.IsHost != true) Shown = false;
        RootCanvas.enabled = Shown;
        if (!Input.GetKeyDown(KeyCode.F2)) return;
        Shown = !Shown;
        Refresh();
    }

    private void Awake()
    {
        BuildUI();
    }
    
    
    public enum Tab
    {
        AI,
        General,
        Power
    }
}


