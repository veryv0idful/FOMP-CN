
using HarmonyLib;

namespace CustomNight.Patches;

using static CustomNightManager; 

[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.ManagePower))]
public static class ManagePowerPatch
{
    private static bool Prefix(PlayerBehaviour __instance)
    {
        bool isPoweredOn = __instance.isPlayerPoweredOn?.Value ?? false;
        bool isAlive = __instance.isPlayerAlive?.Value ?? false;

        if (!isPoweredOn || !isAlive) return false;
        

        __instance.UpdatePowerUsage();

        float power = __instance.currentPower?.Value ?? 100f;
        float usageRate = __instance.currentPowerUsage?.Value ?? 1f;

        power -= usageRate * Time.deltaTime * 0.14285715f;

        if (power > NightSettings.MaxPower) power = NightSettings.MaxPower;

        __instance.currentPower?.Value = power;

        if (!(power <= 0.0f) || !__instance.IsServer) return false;
        __instance.isPlayerPoweredOn.Value = false;
        __instance.currentPower.Value = 0.0f;

            
        GameManager.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
        PurpleGuy.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
        return false;
    }
}