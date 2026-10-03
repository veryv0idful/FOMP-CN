
namespace CustomNight;

public static class AiUtil
{
    public static float GetMovementCoolDownFromAI(AnimatronicID animid, float AI)
    {
        if (AI == 0) return 999;
        if (AI == 100) return 0.001f;
        switch (animid)
        {
            case AnimatronicID.Freddy:
                return 20f * Mathf.Exp(-0.0756f * AI);
            case AnimatronicID.Bonnie:
                return 6f * Mathf.Exp(-0.0203f * AI);
            case AnimatronicID.Chica:
                return 7f * Mathf.Exp(-0.0254f * AI);
            case AnimatronicID.Foxy:
                return 10f * Mathf.Exp(-0.0445f * AI);
            case AnimatronicID.Endo:
                return 15f * Mathf.Exp(-0.0602f * AI);
            case AnimatronicID.Golden:
                return 20f * Mathf.Exp(-0.0756f * AI);
            default:
                return 10f;
        }
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