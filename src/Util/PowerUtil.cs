namespace CustomNight.Util;

public static class PowerUtil
{
    public static void SetReservePower(float reserve)
    {
        PowerGenerator.Instance.reserve.WritePerm = Unity.Netcode.NetworkVariableWritePermission.Server;
        PowerGenerator.Instance.reserve.Value = reserve;
    }

    public static void SetPower(this PlayerBehaviour player, float power)
    {
        player.currentPower.Value = power;
    }
}