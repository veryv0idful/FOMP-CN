namespace CustomNight.Util;

public static class PowerUtil
{
    public static void SetReservePower(float reserve)
    {
        PowerGenerator.Instance.ServerReserve.Value = reserve;
    }

    public static void SetPower(this PlayerBehaviour player, float power)
    {
        player.server_currentPower = power;
    }
}