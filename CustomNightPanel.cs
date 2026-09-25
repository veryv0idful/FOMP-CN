using System;
using CustomNight;
using UnityEngine;

public class CustomNightPanel
{
    public int InfiniteEnergy = 0;
    public int NightTime = 360;
    public int[] SelectedAiLevels = [20, 20, 20, 20, 20, 20];
    public bool Shown = false;

    public int GracePeriod = 7;

    public CPanelTab CurrentPanel = CPanelTab.AI;



    private static readonly AnimatronicID[] Ids =
        (AnimatronicID[])Enum.GetValues(typeof(AnimatronicID));

    public CustomNightPanel()
    {

    }

    bool Button(string Content, Rect rect, Event e)
    {
        GUI.Box(rect, Content);
        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            e.Use();
            return true;
        }
        return false;
    }

    void Field(string Name, ref int Property, int min, int max, float x, ref float y, float rowHeight, int increments, bool IsBool, ref Event e)
    {
        GUI.Label(new Rect(x + 10, y, 80, rowHeight), Name);

        float RightShift = 90;


        if (Button("-", new Rect(x + 90 + RightShift, y, 25, rowHeight), e))
        {
            Property = Mathf.Max(min, Property - (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
            e.Use();
        }

        if (!IsBool)
        {
            GUI.Label(new Rect(x + 120 + RightShift, y, 40, rowHeight), (Property == -1) ? "Default" : Property.ToString());
        }
        else
        {
            GUI.Label(new Rect(x + 120 + RightShift, y, 40, rowHeight), Property == 1 ? "True" : "False");
        }



        if (Button("+", new Rect(x + 160 + RightShift, y, 25, rowHeight), e))
        {
            Property = Mathf.Min(max, Property + (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
            e.Use();
        }

        y += rowHeight;
    }

    public void ToggleShown()
    {
        Shown = !Shown;
    }

    public void Draw()
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
            Field("Night time", ref NightTime, 60, 360 * 5, x, ref y, rowHeight, 60, false, ref e);
            Field("Grace period", ref GracePeriod, 1, 30, x, ref y, rowHeight, 1, false, ref e);
        }




    }

    public enum CPanelTab
    {
        AI,
        General
    }
}

