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

    bool GUIShown = false;
    int[] SelectedAiLevels = [20, 20, 20, 20, 20, 20, 20];
    int NightTime = 360;
    int InfiniteEnergy = 0;

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName == "Game") MelonCoroutines.Start(AIUtil.SetUpCustomNight(SelectedAiLevels, NightTime));
        if (sceneName != "Lobby" || !MultiplayerManager.Instance.IsHost)
        {
            GUIShown = false;
        }
    }

    public override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "Lobby" && Input.GetKeyDown(KeyCode.F2) && MultiplayerManager.Instance.IsHost) GUIShown = !GUIShown;
        AIUtil.OnUpdateFixes(SelectedAiLevels, InfiniteEnergy);
    }

    public override void OnGUI()
    {
        if (GUIShown) DifficultySelectorGUI();
    }

    private void DifficultySelectorGUI()
    {
        float y = 20f;
        float rowHeight = 25f;
        var ids = (AnimatronicID[])Enum.GetValues(typeof(AnimatronicID));
        float width = 300f;
        float x = (Screen.width - width) / 2f;


        GUI.Box(new Rect(x, y, width, rowHeight * (ids.Length + 3)), "Custom Night");
        y += rowHeight;

        Event e = Event.current;

        GUIUtil.Field("Inf energy", ref InfiniteEnergy, 0, 1, x, ref y, rowHeight, 1, true, ref e);
        GUIUtil.Field("Night Time", ref NightTime, 60, 360 * 5, x, ref y, rowHeight, 60, false, ref e);
        for (int i = 0; i < ids.Length; i++)
        {
            AnimatronicID id = ids[i];
            int index = (int)id;

            GUIUtil.Field(id.ToString() + " AI", ref SelectedAiLevels[index], -1, 100, x, ref y, rowHeight, 1, false, ref e);
        }

    }
}
