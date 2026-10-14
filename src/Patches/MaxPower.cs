
namespace CustomNight;

using HarmonyLib;
using static CustomNightManager; 

[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.ManagePower))]
public static class ManagePowerPatch
{
    private static bool Prefix(PlayerBehaviour __instance)
    {
        var IsPoweredOn = __instance.isPlayerPoweredOn?.Value ?? false;
        var IsAlive = __instance.isPlayerAlive?.Value ?? false;

        if (!IsPoweredOn || !IsAlive) return false;
        

        __instance.UpdatePowerUsage();

        float Power = __instance.currentPower?.Value ?? 100f;
        float UsageRate = __instance.currentPowerUsage?.Value ?? 1f;

        Power -= UsageRate * Time.deltaTime * 0.14285715f;

        if (Power > NightSettings.MaxPower) Power = NightSettings.MaxPower;

        __instance.currentPower?.Value = Power;

        if (Power <= 0.0f && __instance.IsServer)
        {
            __instance.isPlayerPoweredOn.Value = false;
            __instance.currentPower.Value = 0.0f;

            
            GameManager.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
            PurpleGuy.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
        }
        return false;
    }
}