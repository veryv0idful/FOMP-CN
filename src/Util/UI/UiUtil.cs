using UnityEngine.UI;

namespace CustomNight.Util.UI;

public static class UiUtil
{
    public static GameObject CreateUIObject(this Transform parent, string name)
    {
        GameObject go = new(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    public static Image CreateImage(this Transform parent, string name, Color? color = null)
    {
        GameObject go = CreateUIObject(parent, name);
        Image img = go.AddComponent<Image>();
        if (color.HasValue) img.color = color.Value;
        return img;
    }
    
    public static Button CreateButton(this Transform parent, string name,string content = "",int contentFontsSize = 40)
    {
        GameObject go = CreateImage(parent, name).gameObject;
        Text text = go.transform.CreateText("content",content,contentFontsSize);
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.SetAnchors(Vector2.zero, Vector2.one);
        
        return go.AddComponent<Button>();
    }
    

    public static Text CreateText(this Transform parent, string name, string value, int fontSize = 14)
    {
        GameObject go = CreateUIObject(parent, name);
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