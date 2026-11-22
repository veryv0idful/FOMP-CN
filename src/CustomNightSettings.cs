using System.Linq;
using System.Reflection;

namespace CustomNight;



[AttributeUsage(AttributeTargets.Field)]
public sealed class SettingAttribute(CustomNightPanel.Tab tab, string name, float min, float max, float step = 1, bool toggle = false) : Attribute
{
    public CustomNightPanel.Tab Tab { get; } = tab;
    public string Name { get; } = name;
    public float Min { get; } = min;
    public float Max { get; } = max;
    public float Step { get; } = step;
    public bool Toggle { get; } = toggle;
}

public sealed class CustomNightSettings
{
    public static readonly (FieldInfo Field, SettingAttribute Attr)[] Settings =
        typeof(CustomNightSettings)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Select(f => (Field: f, Attr: f.GetCustomAttribute<SettingAttribute>()))
            .Where(x => x.Attr != null)
            .ToArray();
    
    public float[] SelectedAiLevels = [-1, -1, -1, -1, -1, -1];

    [Setting(CustomNightPanel.Tab.General, "Night minutes", 1, 360)]
    public float NightTime = 6;

    [Setting(CustomNightPanel.Tab.General, "Grace period", 1, 30)]
    public float GracePeriod = 9;

    [Setting(CustomNightPanel.Tab.General, "Infinite purple guy energy", 0, 1, toggle: true)]
    public float InfiniteEnergy = 0f;

    [Setting(CustomNightPanel.Tab.General, "Disable wordle", 0, 1, toggle: true)]
    public float DisableWordle = 0f;

    [Setting(CustomNightPanel.Tab.General, "Force janitor last man standing", 0, 1, toggle: true)]
    public float ForceJanitorLms = 0f;

    [Setting(CustomNightPanel.Tab.General, "Let endo roam early <color=red>(unstable)</color>", 0, 1, toggle: true)]
    public float EarlyFreeEndo = 0f;

    [Setting(CustomNightPanel.Tab.Power, "Starting power", 0, 10000, 5)]
    public float StartingPower = 100;

    [Setting(CustomNightPanel.Tab.Power, "Max power", 0, 10000, 5)]
    public float MaxPower = 100;

    [Setting(CustomNightPanel.Tab.Power, "Starting generator reserve", 0, 10000, 5)]
    public float StartingReserveEnergy = 0f;
}
