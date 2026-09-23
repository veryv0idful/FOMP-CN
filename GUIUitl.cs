
using UnityEngine;

public static class GUIUtil
{
    public static void Field(string Name, ref int Property, int min, int max, float x, ref float y, float rowHeight, int increments, ref Event e)
    {
        GUI.Label(new Rect(x + 10, y, 80, rowHeight), Name);

        Rect minusRect = new Rect(x + 90, y, 25, rowHeight);
        GUI.Box(minusRect, "-");
        if (e.type == EventType.MouseDown && minusRect.Contains(e.mousePosition))
        {
            Property = Mathf.Max(min, Property - (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
            e.Use();
        }

        GUI.Label(new Rect(x + 120, y, 40, rowHeight), Property == -1 ? "Default" : Property.ToString());

        Rect plusRect = new Rect(x + 160, y, 25, rowHeight);
        GUI.Box(plusRect, "+");
        if (e.type == EventType.MouseDown && plusRect.Contains(e.mousePosition))
        {
            Property = Mathf.Min(max, Property + (Input.GetKey(KeyCode.LeftShift) ? increments * 5 : increments));
            e.Use();
        }

        y += rowHeight;
    }
}