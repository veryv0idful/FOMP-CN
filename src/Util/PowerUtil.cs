namespace CustomNight;

public static class PowerUtil
{
    public static void SetReservePower(float Reserve)
    {
        PowerGenerator.Instance.reserve.WritePerm = Unity.Netcode.NetworkVariableWritePermission.Server;
        PowerGenerator.Instance.reserve.Value = Reserve;
    }

    public static void SetPower(this PlayerBehaviour Player, float Power)
    {
        Player.currentPower.Value = Power;
        if (Power > CustomNightPanel.MaxPower)
        {
            ExtraPower.SetExtraPower(Player,Power - CustomNightPanel.MaxPower);
        }
    }
}