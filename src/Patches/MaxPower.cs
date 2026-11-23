
using HarmonyLib;

namespace CustomNight.Patches;

using static CustomNightManager; 

[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.ManagePower))]
public static class ManagePowerPatch
{
    private static bool Prefix(PlayerBehaviour __instance)
    {
        if (!IsPlayingAsHost) return true;
        bool isPoweredOn = __instance.isPlayerPoweredOn?.Value ?? false;
        bool isAlive = __instance.isPlayerAlive?.Value ?? false;

        if (!isPoweredOn || !isAlive) return false;
        

        __instance.UpdatePowerUsage();

        float power = __instance.server_currentPower;
        float usageRate = __instance.currentPowerUsage?.Value ?? 1f;

        power -= usageRate * Time.deltaTime * 0.14285715f;

        if (power > NightSettings.MaxPower) power = NightSettings.MaxPower;

        __instance.server_currentPower = power;

        if (power <= 0.0f)
        {
            __instance.isPlayerPoweredOn.Value = false;
            __instance.server_currentPower = 0.0f;


            GameManager.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
            PurpleGuy.Instance.OnPlayerPowerDownServerRpc(__instance.playerRole);
        }
        
        __instance.powerSyncTimer -= Time.deltaTime;
        
        if (__instance.powerSyncTimer <= 0f)
        {
            __instance.powerSyncTimer = 0.2f;

            float powerForSync = __instance.server_currentPower;
               

            float normalized = NightSettings.MaxPower > 0 ? Mathf.Clamp01(powerForSync / NightSettings.MaxPower) : 0;
            byte quantizedByte = (byte)Mathf.RoundToInt(normalized * 255f);

            if (__instance.currentPowerByte.Value != quantizedByte)
            {
                __instance.currentPowerByte.Value = quantizedByte;
            }
        }

        return false;
    }
}


[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.Server_RemovePower))]
public static class RemovePowerPatch
{
    private static bool Prefix(PlayerBehaviour __instance, float value)
    {
        if (__instance.server_currentPower > NightSettings.MaxPower)
            __instance.server_currentPower = NightSettings.MaxPower;
        __instance.server_currentPower -= value;
        
        return false;
    }
}

[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.Server_FoxyDrainPower))]
public static class FoxyRemovePowerPatch
{
    private static bool Prefix(PlayerBehaviour __instance, float drainAmount)
    {
        if (__instance.server_currentPower > NightSettings.MaxPower)
            __instance.server_currentPower = NightSettings.MaxPower;
        __instance.server_currentPower -= drainAmount;
        
        return false;
    }
}