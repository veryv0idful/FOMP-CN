
#if MELON
[assembly: MelonInfo(typeof(CustomNight.CustomNightCore), CustomNight.BuildInfo.Name, CustomNight.BuildInfo.Version, CustomNight.BuildInfo.Author)]
#elif BEPIN

#endif

namespace CustomNight;

public static class BuildInfo
{
    public const string Name = "CustomNight"; // Name of the Mod.
    public const string Description = "Adds custom night."; // Description for the Mod. 
    public const string Author = "BrightVoid"; // Author of the Mod.
    public const string Version = "1.11.0"; // Version of the Mod.
}


#if MELON
public class CustomNightCore : MelonMod
{
    public override void OnInitializeMelon()
    {
        Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<CustomNightBehaviour>();

        var go = new GameObject("[CustomNight_Runner]");
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
        AddComponent<CustomNightBehaviour>();
    }
}
#endif
