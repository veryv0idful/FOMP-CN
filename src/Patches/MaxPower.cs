
using HarmonyLib;

namespace CustomNight.Patches;

using static CustomNightManager; 

[HarmonyPatch(typeof(PlayerBehaviour), nameof(PlayerBehaviour.ManagePower))]
public static class ManagePowerPatch
{
    private static bool Prefix(PlayerBehaviour instance)
    {
        var isPoweredOn = instance.isPlayerPoweredOn?.Value ?? false;
        var isAlive = instance.isPlayerAlive?.Value ?? false;

        if (!isPoweredOn || !isAlive) return false;
        

        instance.UpdatePowerUsage();

        float power = instance.currentPower?.Value ?? 100f;
        float usageRate = instance.currentPowerUsage?.Value ?? 1f;

        power -= usageRate * Time.deltaTime * 0.14285715f;

        if (power > NightSettings.MaxPower) power = NightSettings.MaxPower;

        instance.currentPower?.Value = power;

        if (power <= 0.0f && instance.IsServer)
        {
            instance.isPlayerPoweredOn.Value = false;
            instance.currentPower.Value = 0.0f;

            
            GameManager.Instance.OnPlayerPowerDownServerRpc(instance.playerRole);
            PurpleGuy.Instance.OnPlayerPowerDownServerRpc(instance.playerRole);
        }
        return false;
    }
}