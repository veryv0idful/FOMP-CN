namespace CustomNight;

public static class CustomNightPanel
{
    public static float InfiniteEnergy = 0;
    public static float NightTime = 6;
    public static float[] SelectedAiLevels = [20, 20, 20, 20, 20, 20];
    public static bool Shown = false;
    public static float StartingPower = 100;
    public static float MaxPower = 100;
    public static float GracePeriod = 7;

    public static float StartingReserveEnergy = 0f;

    public static float DisableWordle = 0f;
    public static float ForceJanitorLMS = 0f;

    private static float PressStartTime;
    private static bool IsMouseDown;
    private static float LastDuration;
    private static bool FirstHold;

    private static AudioClip Blip;

    public static CPanelTab CurrentPanel = CPanelTab.AI;



    private static readonly AnimatronicID[] Ids =
        (AnimatronicID[])Enum.GetValues(typeof(AnimatronicID));


    static bool Button(string Content, Rect rect, Event e)
    {
        if (Blip == null) Blip = ResourceUtil.GetEmbeddedAudioClip("CustomNight.assets.blip.wav");

        GUI.Box(rect, Content);
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            PressStartTime = Time.realtimeSinceStartup;
            IsMouseDown = true;
            FirstHold = true;
        }

        if (e.type == EventType.MouseUp && e.button == 0 && IsMouseDown)
        {
            IsMouseDown = false;
            LastDuration = Time.realtimeSinceStartup - PressStartTime;
            FirstHold = true;
        }

        float currentHold = IsMouseDown ? (Time.realtimeSinceStartup - PressStartTime) : LastDuration;

        if (currentHold > 0.09 && rect.Contains(e.mousePosition) && !FirstHold)
        {
            PressStartTime = Time.realtimeSinceStartup;
            ResourceUtil.PlayAudioClip(Blip);
            return true;
        }

        if (currentHold > 0.5 && IsMouseDown && FirstHold)
        {
            PressStartTime = Time.realtimeSinceStartup;
            FirstHold = false;
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            e.Use();
            ResourceUtil.PlayAudioClip(Blip);
            return true;
        }
        return false;
    }

    static void Field(string Name, ref float Property, float min, int max, float x, ref float y, float rowHeight, float increments, bool IsBool, ref Event e)
    {
        GUI.Label(new Rect(x + 10, y, 150, rowHeight), Name);

        float RightShift = 90;


        if (Button("-", new Rect(x + 80 + RightShift, y, 25, rowHeight), e))
        {
            Property = Mathf.Max(min, Property - (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
        }

        if (!IsBool) GUI.Label(new Rect(x + 110 + RightShift, y, 50, rowHeight), (Property == -1) ? "Default" : Property.ToString());
        else GUI.Label(new Rect(x + 120 + RightShift, y, 50, rowHeight), Property == 1 ? "True" : "False");


        if (Button("+", new Rect(x + 160 + RightShift, y, 25, rowHeight), e))
        {
            Property = Mathf.Min(max, Property + (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
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

        float ButtonX = (Screen.width - 25) / 2f;
        if (Button("<size=20><</size>", new Rect(ButtonX - width / 2.5f, y, 25, rowHeight), e))
        {
            var values = (CPanelTab[])Enum.GetValues(typeof(CPanelTab));
            int index = Array.IndexOf(values, CurrentPanel);
            index = (index + -1 + values.Length) % values.Length;
            CurrentPanel = values[index];
        }

        if (Button("<size=20>></size>", new Rect(ButtonX + width / 2.5f, y, 25, rowHeight), e))
        {
            var values = (CPanelTab[])Enum.GetValues(typeof(CPanelTab));
            int index = Array.IndexOf(values, CurrentPanel);
            index = (index + 1 + values.Length) % values.Length;
            CurrentPanel = values[index];
        }

        if (CurrentPanel == CPanelTab.AI)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (Ids.Length + 2)), "Custom Night - AI");
            y += rowHeight * 2;
            for (int i = 0; i < Ids.Length; i++)
            {
                AnimatronicID id = Ids[i];
                int index = (int)id;

                Field(id.ToString() + " AI", ref SelectedAiLevels[index], -1, 100, x, ref y, rowHeight, 1, false, ref e);
            }
        }

        if (CurrentPanel == CPanelTab.General)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (Ids.Length + 2)), "Custom Night - GENERAL");
            y += rowHeight * 2;
            Field("Inf energy", ref InfiniteEnergy, 0, 1, x, ref y, rowHeight, 1, true, ref e);
            Field("Night minutes", ref NightTime, 1, 360, x, ref y, rowHeight, 1, false, ref e);
            Field("Grace period", ref GracePeriod, 1, 30, x, ref y, rowHeight, 1, false, ref e);
            Field("Disable wordle", ref DisableWordle, 0, 1, x, ref y, rowHeight, 1, true, ref e);
            Field("Force janitor lms", ref ForceJanitorLMS, 0, 1, x, ref y, rowHeight, 1, true, ref e);
        }

        if (CurrentPanel == CPanelTab.Power)
        {
            GUI.Box(new Rect(x, y, width, rowHeight * (Ids.Length + 2)), "Custom Night - POWER");
            y += rowHeight * 2;
            Field("Starting power", ref StartingPower, 1, 1000, x, ref y, rowHeight, 1, false, ref e);
            Field("Max power", ref MaxPower, 1, 100, x, ref y, rowHeight, 1, false, ref e);
            Field("Starting reserve", ref StartingReserveEnergy, 0, 9999, x, ref y, rowHeight, 5, false, ref e);
        }



    }

    public enum CPanelTab
    {
        AI,
        General,
        Power
    }
}

