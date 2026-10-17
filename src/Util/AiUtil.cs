namespace CustomNight;

public static class AiUtil
{
    public static float GetMovementCoolDownFromAI(this AnimatronicID animid, float ai)
    {
        if (ai == 0) return float.PositiveInfinity;
        if (ai == 100) return 0.001f;
        return animid switch
        {
            AnimatronicID.Freddy => 20f * Mathf.Exp(-0.0756f * ai),
            AnimatronicID.Bonnie => 6f * Mathf.Exp(-0.0203f * ai),
            AnimatronicID.Chica => 7f * Mathf.Exp(-0.0254f * ai),
            AnimatronicID.Foxy => 10f * Mathf.Exp(-0.0445f * ai),
            AnimatronicID.Zap => 15f * Mathf.Exp(-0.0602f * ai),
            AnimatronicID.Golden => 20f * Mathf.Exp(-0.0756f * ai),
            _ => 10f,
        };
    }

    public static float GetMaxMovementCoolDownFromAI(this AnimatronicID animid,float ai) => animid.GetMovementCoolDownFromAI(ai) * 3;
    public static float GetMaxFreddyWind(float freddyAI) => freddyAI <= 20 ? 1 : Mathf.Max(1f - Mathf.Floor(freddyAI / 20f) / 5f, 0.2f);
    public static int GetFoxyPreAttacks(float ai) => Mathf.Max(Mathf.FloorToInt(ai / 100 * 8) - 1, 0);
    public static AnimatronicID GetIdFromAnimatronic(this Animatronic animatronic) => Enum.Parse<AnimatronicID>(animatronic.name,false);
    public static Animatronic GetAnimatronicFromID(this AnimatronicID animid) => AnimatronicManager.Instance?.Animatronics[(int)animid];
    public static readonly AnimatronicID[] AnimatronicIDs = (AnimatronicID[])Enum.GetValues(typeof(AnimatronicID));


}

public enum AnimatronicID
{
    Freddy = 0,
    Bonnie,
    Chica,
    Foxy,
    Zap,
    Golden
}