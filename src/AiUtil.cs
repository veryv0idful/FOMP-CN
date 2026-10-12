
namespace CustomNight;

public static class AiUtil
{
    public static float GetMovementCoolDownFromAI(this AnimatronicID animid, float AI)
    {
        if (AI == 0) return float.PositiveInfinity;
        if (AI == 100) return 0.001f;
        return animid switch
        {
            AnimatronicID.Freddy => 20f * Mathf.Exp(-0.0756f * AI),
            AnimatronicID.Bonnie => 6f * Mathf.Exp(-0.0203f * AI),
            AnimatronicID.Chica => 7f * Mathf.Exp(-0.0254f * AI),
            AnimatronicID.Foxy => 10f * Mathf.Exp(-0.0445f * AI),
            AnimatronicID.Endo => 15f * Mathf.Exp(-0.0602f * AI),
            AnimatronicID.Golden => 20f * Mathf.Exp(-0.0756f * AI),
            _ => 10f,
        };
    }

    public static float GetMaxMovementCoolDownFromAI(this AnimatronicID animid,float AI) => animid.GetMovementCoolDownFromAI(AI) * 3;
    public static float GetMaxFreddyWind(float FreddyAI) => FreddyAI <= 20 ? 1 : Mathf.Max(1f - Mathf.Floor(FreddyAI / 20f) / 5f, 0.2f);
    public static int GetFoxyPreAttacks(float AI) => Mathf.Max(Mathf.FloorToInt(AI / 100 * 8) - 1, 0);
    public static Animatronic GetAnimatronicFromID(this AnimatronicID animid) => AnimatronicManager.Instance?.Animatronics[(int)animid];
}

public enum AnimatronicID
{
    Freddy = 0,
    Bonnie,
    Chica,
    Foxy,
    Endo,
    Golden
}