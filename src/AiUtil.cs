
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

    public static float GetMaxFreddyWind(float FreddyAI)
    {
        if (FreddyAI <= 20) return 1;
        return Mathf.Max(1f - Mathf.Floor(FreddyAI / 20f) / 5f, 0.2f);
    }

    public static int GetFoxyPreAttacks(float AI)
    {
        return Mathf.Max((int)Mathf.Floor(AI / 100 * 8) - 1, 0);
    }
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