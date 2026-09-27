using System.Collections;
using System;
using UnityEngine;

#if MELON
using Il2Cpp;
#elif BEPIN

#endif


namespace CustomNight;

public static class AIUtil
{

    public static IEnumerator SetUpCustomNight()
    {
        if (!IsLocalPlayerHost()) yield break;
        while (!GameManager.Instance.isPlaying) yield return null;

        foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
        {
            if (CustomNightPanel.SelectedAiLevels[(int)id] == -1) continue;
            SetCustomAiLevel(id, (int)CustomNightPanel.SelectedAiLevels[(int)id]);
        }
        SetNightProperties();
    }

    public static void SetNightProperties()
    {
        if (!IsPlaying() || !IsLocalPlayerHost()) return;

        SetGameEndTime(CustomNightPanel.NightTime * 60);
        SetReservePower(CustomNightPanel.StartingReserveEnergy);

        foreach (var Role in PlayerRoleManager.Instance.participatingPlayers)
        {
            if (Role == PlayerRoles.PurpleGuy || Role == PlayerRoles.None) continue;
            
            var Player = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(Role);

            SetPower(Player,CustomNightPanel.StartingPower);
        };

    }

    public static void SetPower(PlayerBehaviour Player,float Power)
    {
        Player.currentPower.Value = Power;
    }

    public static void SetGameEndTime (float EndTime)
    {
        GameManager.Instance.gameEndTime.Value = EndTime;
    }

    public static bool IsLocalPlayerHost()
    {
        if (!MultiplayerManager.Instance|| !MultiplayerManager.Instance.IsHost) return false;
        return true;
    }

    public static bool IsPlaying()
    {
        return GameManager.Instance && GameManager.Instance.isPlaying;
    }

    public static float GetMaxFreddyWind(float FreddyAI)
    {
        if (FreddyAI <= 20) return 1;
        return Mathf.Max(1f - Mathf.Floor(FreddyAI / 20f) / 5f, 0.2f);
    }

    public static void SetReservePower(float Reserve)
    {
        if (!IsPlaying() || !IsLocalPlayerHost()) return;
        PowerGenerator.Instance.reserve.Value = Reserve;
    }


    public static void SetCustomAiLevel(AnimatronicID animid, float AI)
    {
        AnimatronicManager animManager = AnimatronicManager.Instance;
        var Anims = animManager.Animatronics;

        if (AI == 0)
        {
            Anims[(int)animid].Disable();
            return;
        }

        if (animid == AnimatronicID.Foxy)
        {
            Anims[(int)animid]?.TryCast<Foxy>().currentAttackAttempt.Value = Mathf.Max((int)Mathf.Floor(AI/100*8)-1,0);
        }

        Anims[(int)animid].currentDifficulty.Value = AI;
        Anims[(int)animid].currentMovementWaitTime.Value = GetMovementCoolDownFromAI(animid, AI);
        Anims[(int)animid].timeLeftToMove.Value = Mathf.Max(GetMovementCoolDownFromAI(animid, AI) * 3, CustomNightPanel.GracePeriod);

    }

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

    public static void OnUpdateFixes()
    {
        if (!IsPlaying() || !IsLocalPlayerHost()) return;
        var anims = AnimatronicManager.Instance.Animatronics;

        if (PowerGenerator.Instance.reserve.Value < CustomNightPanel.StartingReserveEnergy && GameManager.Instance.currentGameTime.Value < CustomNightPanel.GracePeriod)
        {
            PowerGenerator.Instance.reserve.WritePerm = Unity.Netcode.NetworkVariableWritePermission.Server;
            PowerGenerator.Instance.reserve.Value = CustomNightPanel.StartingReserveEnergy;
        }
        


        foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
        {
            if (CustomNightPanel.SelectedAiLevels[(int)id] == -1 || CustomNightPanel.SelectedAiLevels[(int)id] == 0) continue;

            if (id == AnimatronicID.Freddy)
            {
                var Wind = GlobalCameraSystem.Instance.freddyMusicWind;
                Wind.Value = Mathf.Min(GetMaxFreddyWind(CustomNightPanel.SelectedAiLevels[(int)id]), Wind.Value);
            }

            var anim = anims[(int)id];
            if (anim.timeLeftToMove.Value > anim.currentMovementWaitTime.Value * 3 && GameManager.Instance.currentGameTime.Value > CustomNightPanel.GracePeriod) anim.timeLeftToMove.Value = anim.currentMovementWaitTime.Value * 3;
            
        }

        if (CustomNightPanel.InfiniteEnergy == 1 && PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value != 800)
        {
            PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value = 800;
        }

        foreach (var Role in PlayerRoleManager.Instance.participatingPlayers)
        {
            if (Role == PlayerRoles.PurpleGuy || Role == PlayerRoles.None) continue;
            var PlayerBehave = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(Role);
            PlayerBehave.currentPower.Value = Mathf.Min(PlayerBehave.currentPower.Value,CustomNightPanel.MaxPower);
        };
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