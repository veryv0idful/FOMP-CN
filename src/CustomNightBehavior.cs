using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;


#if BEPIN
using BepInEx.Unity.IL2CPP.Utils.Collections;
#endif

namespace CustomNight;

public class CustomNightBehaviour : MonoBehaviour
{
    public CustomNightBehaviour(IntPtr handle) : base(handle) { }

    private void Awake()
    {
        SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2) && SceneManager.GetActiveScene().name == "Lobby")
        {
            if (MultiplayerManager.Instance && MultiplayerManager.Instance.IsHost)
            {
                CustomNightPanel.ToggleShown();
            }
        }

        if (Input.GetKeyDown(KeyCode.F4) && GameManager.Instance && GameManager.Instance.IsHost)
        {
            GameManager.Instance.BackToLobby();
        }

        CustomNightManager.OnUpdateFixes();
    }

    private void OnGUI()
    {
        if (CustomNightPanel.Shown)
        {
            CustomNightPanel.Draw();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            StartCoroutine(CustomNightManager.SetUpCustomNight());
        }

        if (scene.name != "Lobby" || !MultiplayerManager.Instance || !MultiplayerManager.Instance.IsHost)
        {
            CustomNightPanel.Shown = false;
        }
    }

    private void StartCoroutine(IEnumerator Coroutine)
    {
#if BEPIN
        StartCoroutine(Coroutine.WrapToIl2Cpp());
#elif MELON
        MelonCoroutines.Start(Coroutine);
#endif
    }
}