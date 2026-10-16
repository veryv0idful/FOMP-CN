namespace CustomNight;

using static CustomNightManager;

public static class CustomNightPanel
{
    public static bool Shown;
    private static bool _isMouseDown;
    private static float _pressStart;
    private static float _nextRepeatTime;
    public static float MouseCurrentHold => _isMouseDown ? Time.realtimeSinceStartup - _pressStart : 0f;
    private static AudioClip _blip;
    public static CPanelTab CurrentPanel = CPanelTab.AI;

    static bool Button(string content, Rect rect, Event e)
    {
        if (!_blip) _blip = ResourceUtil.GetEmbeddedAudioClip("CustomNight.assets.blip.wav");

        GUI.Box(rect, content);

        float now = Time.realtimeSinceStartup;
        bool fired = false;

        if (e.rawType == EventType.MouseUp && e.button == 0)
        {
            _isMouseDown = false;
        }
        else if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition))
        {
            _isMouseDown = true;
            _pressStart = now;
            _nextRepeatTime = now + 0.5f;
            e.Use();
            fired = true;
        }
        else if (e.type == EventType.Repaint && _isMouseDown
                 && rect.Contains(e.mousePosition) && now >= _nextRepeatTime)
        {
            _nextRepeatTime = now + 0.09f;
            fired = true;
        }

        if (fired) ResourceUtil.PlayAudioClip(_blip);
        return fired;
    }

    static void Field(string name, ref float property, float min, int max, float x, ref float y, float rowHeight, float increments, bool isBool, ref Event e)
    {
        GUI.Label(new Rect(x + 10, y, 150, rowHeight), name);

        float rightShift = 90;

        int holdMultiplier = 1 << Math.Min((int)(MouseCurrentHold / 0.5f), 6);

        if (Button("-", new Rect(x + 80 + rightShift, y, 25, rowHeight), e))
        {
            property = Mathf.Max(min, property - (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments) * holdMultiplier);
        }

        if (!isBool) GUI.Label(new Rect(x + 110 + rightShift, y, 50, rowHeight), (property == -1) ? "Default" : property.ToString());
        else GUI.Label(new Rect(x + 120 + rightShift, y, 50, rowHeight), property == 1 ? "True" : "False");


        if (Button("+", new Rect(x + 160 + rightShift, y, 25, rowHeight), e))
        {
            property = Mathf.Min(max, property + (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments) * holdMultiplier);
        }

        y += rowHeight;
    }

    static public void ToggleShown()
    {
        Shown = !Shown;
    }

    static public void Draw()
    {
        float y = 20f;
        float rowHeight = 25f;
        float width = 300f;
        float x = (Screen.width - width) / 2f;


        Event e = Event.current;
        GUI.color = new Color(1f, 1f, 1f, 1f);

        float buttonX = (Screen.width - 25) / 2f;
        if (Button("<size=20><</size>", new Rect(buttonX - width / 2.5f, y, 25, rowHeight), e))
        {
            CPanelTab[] values = (CPanelTab[])Enum.GetValues(typeof(CPanelTab));
            int index = Array.IndexOf(values, CurrentPanel);
            index = (index + -1 + values.Length) % values.Length;
            CurrentPanel = values[index];
        }

        if (Button("<size=20>></size>", new Rect(buttonX + width / 2.5f, y, 25, rowHeight), e))
        {
            CPanelTab[] values = (CPanelTab[])Enum.GetValues(typeof(CPanelTab));
            int index = Array.IndexOf(values, CurrentPanel);
            index = (index + 1 + values.Length) % values.Length;
            CurrentPanel = values[index];
        }

        if (CurrentPanel == CPanelTab.AI)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (AiUtil.AnimatronicIDs.Length + 2)), "Custom Night - AI");
            y += rowHeight * 2;
            for (int i = 0; i < AiUtil.AnimatronicIDs.Length; i++)
            {
                AnimatronicID id = AiUtil.AnimatronicIDs[i];
                int index = (int)id;

                Field(id.ToString() + " AI", ref NightSettings.SelectedAiLevels[index], -1, 100, x, ref y, rowHeight, 1, false, ref e);
            }
        }

        if (CurrentPanel == CPanelTab.General)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (AiUtil.AnimatronicIDs.Length + 2)), "Custom Night - GENERAL");
            y += rowHeight * 2;
            Field("Night minutes", ref NightSettings.NightTime, 1, 360, x, ref y, rowHeight, 1, false, ref e);
            Field("Grace period", ref NightSettings.GracePeriod, 1, 30, x, ref y, rowHeight, 1, false, ref e);
            Field("Inf energy", ref NightSettings.InfiniteEnergy, 0, 1, x, ref y, rowHeight, 1, true, ref e);
            Field("Disable wordle", ref NightSettings.DisableWordle, 0, 1, x, ref y, rowHeight, 1, true, ref e);
            Field("Force janitor lms", ref NightSettings.ForceJanitorLms, 0, 1, x, ref y, rowHeight, 1, true, ref e);
            Field("Endo roam early", ref NightSettings.EarlyFreeEndo, 0, 1, x, ref y, rowHeight, 1, true, ref e);
        }

        if (CurrentPanel == CPanelTab.Power)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (AiUtil.AnimatronicIDs.Length + 2)), "Custom Night - POWER");
            y += rowHeight * 2;
            Field("Starting power", ref NightSettings.StartingPower, 0, 10000, x, ref y, rowHeight, 5, false, ref e);
            Field("Max power", ref NightSettings.MaxPower, 0, 10000, x, ref y, rowHeight, 5, false, ref e);
            Field("Starting reserve", ref NightSettings.StartingReserveEnergy, 0, 10000, x, ref y, rowHeight, 5, false, ref e);
        }
    }

    public enum CPanelTab
    {
        AI,
        General,
        Power
    }
}

