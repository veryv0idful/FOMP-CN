using System;
using System.Collections;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace CustomNight;

public static class AIUtil
{

    public static IEnumerator SetUpCustomNight(int[] SelectedAiLevels, int NightTime)
    {
        if (!MultiplayerManager.Instance.IsHost) yield break;
        while (!GameManager.Instance.isPlaying) yield return null;

        foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
        {
            if (SelectedAiLevels[(int)id] == -1) continue;
            SetCustomAiLevel(id, SelectedAiLevels[(int)id]);
        }
        SetNightTime(NightTime);
    }

    public static void SetNightTime(int NightTime)
    {
        if (!MultiplayerManager.Instance.IsHost) return;
        if (!GameManager.Instance.isPlaying) return;

        GameManager.Instance.gameEndTime.Value = NightTime;
    }
    public static void SetCustomAiLevel(AnimatronicID animid, int AI)
    {
        AnimatronicManager animManager = AnimatronicManager.Instance;
        var Anims = animManager.Animatronics;
        Melon<CustomNightCore>.Logger.Msg($"Set {animid.ToString()} To AI:{AI} and MOVE:{GetMovementCoolDownFromAI(animid, AI)}s");

        if (AI == 0)
        {
            Anims[(int)animid].Disable();
            return;
        }

        Anims[(int)animid].currentDifficulty.Value = AI;
        Anims[(int)animid].currentMovementWaitTime.Value = GetMovementCoolDownFromAI(animid, AI);
        Anims[(int)animid].timeLeftToMove.Value = GetMovementCoolDownFromAI(animid, AI) * 3;

    }

    public static float GetMovementCoolDownFromAI(AnimatronicID animid, int AI)
    {
        if (AI == 0) return 999;
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

    public static void FixAnimatronicsCooldown(int[] SelectedAiLevels)
    {
        if (!GameManager.Instance || !GameManager.Instance.isPlaying) return;
        var anims = AnimatronicManager.Instance.Animatronics;
        foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
        {
            if (SelectedAiLevels[(int)id] == -1) continue;
            var anim = anims[(int)id];
            if (anim.timeLeftToMove.Value > anim.currentMovementWaitTime.Value*3) anim.timeLeftToMove.Value = anim.currentMovementWaitTime.Value*3;
        }
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