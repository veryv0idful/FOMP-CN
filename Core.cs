using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;
[assembly: MelonInfo(typeof(CustomNight.CustomNightCore), CustomNight.BuildInfo.Name, CustomNight.BuildInfo.Version, CustomNight.BuildInfo.Author)]

namespace CustomNight;

public static class BuildInfo
{
    public const string Name = "CustomNight"; // Name of the Mod.
    public const string Description = "Adds custom night."; // Description for the Mod. 
    public const string Author = "BrightVoid"; // Author of the Mod.
    public const string Version = "1.3.0"; // Version of the Mod.
}

public class CustomNightCore : MelonMod
{


    CustomNightPanel CustomPanel = new();

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName == "Game") MelonCoroutines.Start(AIUtil.SetUpCustomNight(CustomPanel));
        if (sceneName != "Lobby" || !MultiplayerManager.Instance.IsHost)
        {
            CustomPanel.Shown = false;
        }
    }

    public override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Lobby" && Input.GetKeyDown(KeyCode.F2) && MultiplayerManager.Instance.IsHost) CustomPanel.ToggleShown();
        AIUtil.OnUpdateFixes(CustomPanel);
    }

    public override void OnGUI()
    {
        if (CustomPanel.Shown) CustomPanel.Draw();
    }

}
