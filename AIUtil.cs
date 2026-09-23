using System;
using System.Collections;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace CustomNight;

public static class AIUtil
{

    public static IEnumerator SetUpAiLevels(int[] SelectedAiLevels)
    {
        if (!MultiplayerManager.Instance.IsHost) yield break;
        while (!GameManager.Instance.isPlaying) yield return null;

        AnimatronicManager animManager = AnimatronicManager.Instance;
        var Anims = animManager.Animatronics;

        foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
        {
            if (SelectedAiLevels[(int)id] == -1) continue;
            SetCustomAiLevel(id, SelectedAiLevels[(int)id]);
        }
    }
    public static void SetCustomAiLevel(AnimatronicID animid, int AI)
    {
        AnimatronicManager animManager = AnimatronicManager.Instance;
        var Anims = animManager.Animatronics;

        Anims[(int)animid].currentDifficulty.Value = AI;
        Anims[(int)animid].currentMovementWaitTime.Value = GetMovementCoolDownFromAI(animid, AI);
        Anims[(int)animid].timeLeftToMove.Value = GetMovementCoolDownFromAI(animid, AI) * 3;

        Melon<CustomNightCore>.Logger.Msg($"Set {animid.ToString()} To AI:{AI} and MOVE:{GetMovementCoolDownFromAI(animid, AI)}s");
    }

    public static float GetMovementCoolDownFromAI(AnimatronicID animid, int AI)
    {
        if (AI == 0) return 99999999;
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
            default:
                return 10f;
        }
    }
}

public enum AnimatronicID
{
    Freddy = 0,
    Bonnie,
    Chica,
    Foxy,
    Endo
}