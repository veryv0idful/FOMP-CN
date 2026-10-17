using UnityEngine;
using UnityEngine.UI;

namespace CustomNight;

public static class UiUtil
{
    public static GameObject CreateChildUIObject(this Transform parent, string name)
    {
        GameObject go = new(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    public static Image CreateImageChildObject(this Transform parent, string name, Color? color = null)
    {
        GameObject go = CreateChildUIObject(parent, name);
        Image img = go.AddComponent<Image>();
        if (color.HasValue) img.color = color.Value;
        return img;
    }
    
    public static Button CreateButtonChildObject(this Transform parent, string name)
    {
        GameObject go = CreateChildUIObject(parent, name);
        go.AddComponent<Image>();
        return go.AddComponent<Button>();
    }

    public static Text CreateTextChildObject(this Transform parent, string name, string value, int fontSize = 14)
    {
        GameObject go = CreateChildUIObject(parent, name);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.color = Color.white;
        return text;
    }

    public static RectTransform SetAnchors(this RectTransform rect, Vector2 min, Vector2 max, Vector2 offsetMin = default, Vector2 offsetMax = default)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        return rect;
    }
}