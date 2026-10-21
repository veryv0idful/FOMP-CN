
using CustomNight.Util.UI;
using Il2CppInterop.Runtime.Injection;

#if MELON
[assembly: MelonInfo(typeof(CustomNight.CustomNightCore), CustomNight.BuildInfo.Name, CustomNight.BuildInfo.Version, CustomNight.BuildInfo.Author)]
#elif BEPIN

#endif

namespace CustomNight;

public static class BuildInfo
{
    public const string Name = "CustomNight";
    public const string Author = "BrightVoid";
    public const string Version = "1.13.0";
}


#if MELON
public class CustomNightCore : MelonMod
{
    public override void OnInitializeMelon()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<CustomNightBehaviour>();

        GameObject go = new GameObject("[CustomNight_Runner]");
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<CustomNightBehaviour>();
    }
}

#elif BEPIN
[BepInPlugin("com.customnight.mod", "CustomNight", BuildInfo.Version)]
public class CustomNightCore : BasePlugin
{
    public override void Load()
    {
        ClassInjector.RegisterTypeInIl2Cpp<CustomNightBehaviour>();
        ClassInjector.RegisterTypeInIl2Cpp<CustomNightPanel>();
        ClassInjector.RegisterTypeInIl2Cpp<HoldRepeat>();
        AddComponent<CustomNightBehaviour>();
    }
}
#endif
