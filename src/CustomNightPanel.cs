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
    public static Canvas RootCanvas { get; private set; }
    private static CanvasScaler _canvasScalar;
    private static GraphicRaycaster _canvasRayCaster;
    
    public Tab CurrentTab { get; private set; } = Tab.AI;
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
        
        Image background = RootCanvas.transform.CreateImageChildObject("BackGround");
        RectTransform rect = background.GetComponent<RectTransform>();
        
        background.color = Color.gray;
        background.sprite = ResourceUtil.LoadEmbeddedSprite("CustomNight.assets.Zap.png");

        rect.SetAnchors(new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f));
        
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) Shown = !Shown;
        if (SceneManager.GetActiveScene().name != "Lobby" || MultiplayerManager.Instance?.IsHost != true) Shown = false;
        RootCanvas.enabled = Shown;
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


