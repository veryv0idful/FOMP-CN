using System;
using System.Collections;
using System.Collections.Generic;
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
    public const string Version = "1.0.0"; // Version of the Mod.
}

public class CustomNightCore : MelonMod
{

    bool GUIShown = false;
    int[] SelectedAiLevels = [20,20,20,20,20];

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        if (sceneName == "Game") MelonCoroutines.Start(AIUtil.SetUpAiLevels(SelectedAiLevels));
        if (sceneName != "Lobby" || !MultiplayerManager.Instance.IsHost)
        {
            GUIShown = false;
        }
    }

    public override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name != "Game")
        if (Input.GetKeyDown(KeyCode.F2)) GUIShown = !GUIShown;
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


        GUI.Box(new Rect(x, y, width, rowHeight * (ids.Length + 1)), "Custom Night");
        y += rowHeight;

        Event e = Event.current;

        for (int i = 0; i < ids.Length; i++)
        {
            AnimatronicID id = ids[i];
            int index = (int)id;
            int current = SelectedAiLevels[index];

            GUI.Label(new Rect(x + 10, y, 80, rowHeight), id.ToString());

            Rect minusRect = new Rect(x + 90, y, 25, rowHeight);
            GUI.Box(minusRect, "-");
            if (e.type == EventType.MouseDown && minusRect.Contains(e.mousePosition))
            {
                SelectedAiLevels[index] = Mathf.Max(-1, current - (Input.GetKey(KeyCode.LeftShift) ? 5 : 1));
                e.Use();
            }

            GUI.Label(new Rect(x + 120, y, 40, rowHeight), (current == -1 ? "Default" : current.ToString()) + " AI");

            Rect plusRect = new Rect(x + 160, y, 25, rowHeight);
            GUI.Box(plusRect, "+");
            if (e.type == EventType.MouseDown && plusRect.Contains(e.mousePosition))
            {
                SelectedAiLevels[index] = Mathf.Min(100, current + (Input.GetKey(KeyCode.LeftShift) ? 5 : 1));
                e.Use();
            }

            y += rowHeight;
        }
    }










}
