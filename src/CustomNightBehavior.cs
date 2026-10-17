using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;


#if BEPIN
using BepInEx.Unity.IL2CPP.Utils.Collections;
#endif

namespace CustomNight;

#if MELON
[RegisterTypeInIl2Cpp]
#endif

public class CustomNightBehaviour(IntPtr handle) : MonoBehaviour(handle)
{
    private void Awake()
    {
        SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
        CustomNightPanel.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4) && GameManager.Instance && GameManager.Instance.IsHost)
        {
            GameManager.Instance.BackToLobby();
        }

        CustomNightManager.OnUpdateFixes();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            StartCoroutine(CustomNightManager.SetUpCustomNight());
        }
    }

    [HideFromIl2Cpp]
    private void StartCoroutine(IEnumerator coroutine)
    {
#if BEPIN
        StartCoroutine(coroutine.WrapToIl2Cpp());
#elif MELON
        MelonCoroutines.Start(coroutine);
#endif
    }
}