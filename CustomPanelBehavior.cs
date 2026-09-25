using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if MELON
using Il2Cpp;
using MelonLoader;
#elif BEPIN
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Unity.Netcode;
#endif

namespace CustomNight;

public class CustomPanelBehaviour : MonoBehaviour
{
    public CustomPanelBehaviour(IntPtr handle) : base(handle) { }

    private void Awake()
    {
        SceneManager.add_sceneLoaded((UnityAction<Scene, LoadSceneMode>)OnSceneLoaded);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2) && SceneManager.GetActiveScene().name == "Lobby")
        {
            if (CheckIsHost())
            {
                CustomNightPanel.ToggleShown();
            }
        }

        AIUtil.OnUpdateFixes();
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
            StartGameCoroutine();
        }

        if (scene.name != "Lobby" || !CheckIsHost())
        {
            CustomNightPanel.Shown = false;
        }
    }

    private void StartGameCoroutine()
    {
#if BEPIN
        StartCoroutine(AIUtil.SetUpCustomNight().WrapToIl2Cpp());
#elif MELON
        MelonCoroutines.Start(AIUtil.SetUpCustomNight());
#endif
    }

    private bool CheckIsHost()
    {
        try
        {
            return MultiplayerManager.Instance != null && MultiplayerManager.Instance.IsHost;
        }
        catch
        {
            return false;
        }
    }
}