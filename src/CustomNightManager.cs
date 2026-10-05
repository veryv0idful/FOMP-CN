using System.Collections;
namespace CustomNight;

public static class CustomNightManager
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

    foreach (var Role in PlayerRoleManager.Instance.participatingPlayers)
    {
      if (Role == PlayerRoles.PurpleGuy || Role == PlayerRoles.None) continue;

      var Player = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(Role);

      SetPower(Player, CustomNightPanel.StartingPower);
    }

    if (CustomNightPanel.DisableWordle == 1) WordleSystem.Instance.gameState.Value = WordleSystem.WordleGameState.Loss;

#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET NIGHT TIME TO {CustomNightPanel.NightTime * 60}s");
    Melon<CustomNightCore>.Logger.Msg($"SET GRACE PERIOD TO {CustomNightPanel.GracePeriod}s");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX FREDDY WIND TO {AiUtil.GetMaxFreddyWind(CustomNightPanel.SelectedAiLevels[0]) * 100}%");
    Melon<CustomNightCore>.Logger.Msg($"SET FOXY PREATTACKS TO {AiUtil.GetFoxyPreAttacks(CustomNightPanel.SelectedAiLevels[(int)AnimatronicID.Foxy])} ATTACKS");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX POWER TO {CustomNightPanel.MaxPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING POWER TO {CustomNightPanel.StartingPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING RESERVE POWER TO {CustomNightPanel.StartingReserveEnergy}%");
    Melon<CustomNightCore>.Logger.Msg($"SET INFINITE PURPLE ENERGY TO {CustomNightPanel.InfiniteEnergy == 1}");
    Melon<CustomNightCore>.Logger.Msg($"SET DISABLE WORDLE TO {CustomNightPanel.DisableWordle == 1}");
#endif


  }

  public static void SetPower(PlayerBehaviour Player, float Power)
  {
    Player.currentPower.Value = Power;
    if (Power > CustomNightPanel.MaxPower)
    {
      SetExtraPower(Player, Power - CustomNightPanel.MaxPower);
    }
  }

  public static void SetExtraPower(PlayerBehaviour Player, float ExtraPower)
  {
    if (Player.gameObject.TryGetComponent(out ExtraPower extraPower))
    {
      extraPower.CurrentExtraPower = ExtraPower;
    }
    else
    {
      extraPower = Player.gameObject.AddComponent<ExtraPower>();
      extraPower.CurrentExtraPower = ExtraPower;
    }
  }

  public static void SetGameEndTime(float EndTime)
  {
    GameManager.Instance.gameEndTime.Value = EndTime;

  }

  public static bool IsLocalPlayerHost()
  {
    if (!MultiplayerManager.Instance || !MultiplayerManager.Instance.IsHost) return false;
    return true;
  }

  public static bool IsPlaying()
  {
    return GameManager.Instance && GameManager.Instance.isPlaying;
  }



  public static void SetReservePower(float Reserve)
  {
    if (!IsPlaying() || !IsLocalPlayerHost()) return;

    PowerGenerator.Instance.reserve.WritePerm = Unity.Netcode.NetworkVariableWritePermission.Server;
    PowerGenerator.Instance.reserve.Value = Reserve;
  }




  public static void SetCustomAiLevel(AnimatronicID animid, float AI)
  {
    AnimatronicManager animManager = AnimatronicManager.Instance;
    var Anims = animManager.Animatronics;

#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET {animid.ToString()} AI {AI} || MOVEMENT {AiUtil.GetMovementCoolDownFromAI(animid, AI)}s");
#endif


    if (AI == 0)
    {
      Anims[(int)animid].enabled = false;
    }

    if (animid == AnimatronicID.Foxy)
    {
      Anims[(int)animid]?.TryCast<Foxy>().currentAttackAttempt.Value = AiUtil.GetFoxyPreAttacks(AI);
    }

    Anims[(int)animid].currentDifficulty.Value = AI;
    Anims[(int)animid].currentMovementWaitTime.Value = AiUtil.GetMovementCoolDownFromAI(animid, AI);
    Anims[(int)animid].timeLeftToMove.Value = Mathf.Max(AiUtil.GetMovementCoolDownFromAI(animid, AI) * 3, CustomNightPanel.GracePeriod);
  }

  public static void OnUpdateFixes()
  {
    if (!IsPlaying() || !IsLocalPlayerHost()) return;
    var anims = AnimatronicManager.Instance.Animatronics;

    if (PowerGenerator.Instance.reserve.Value < CustomNightPanel.StartingReserveEnergy && GameManager.Instance.currentGameTime.Value < CustomNightPanel.GracePeriod && CustomNightPanel.StartingReserveEnergy != 0)
    {
      SetReservePower(CustomNightPanel.StartingReserveEnergy);
    }

    if (CustomNightPanel.ForceJanitorLMS == 1 && PlayerRoleManager.Instance.CountPlayersAlive() > 1)
    {
      PlayerRoleManager.Instance.janitorBehaviour.oxygenLevels.Value = 300;
    }



    foreach (AnimatronicID id in Enum.GetValues(typeof(AnimatronicID)))
    {
      if (CustomNightPanel.SelectedAiLevels[(int)id] == -1) continue;

      if (CustomNightPanel.SelectedAiLevels[(int)id] == 0 && anims[(int)id].isActiveAndEnabled)
      {
        anims[(int)id].enabled = false;
      }

      if (id == AnimatronicID.Freddy)
      {
        var Wind = GlobalCameraSystem.Instance.freddyMusicWind;
        Wind.Value = Mathf.Min(AiUtil.GetMaxFreddyWind(CustomNightPanel.SelectedAiLevels[(int)id]), Wind.Value);
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
      PlayerBehave.currentPower.Value = Mathf.Min(PlayerBehave.currentPower.Value, CustomNightPanel.MaxPower);
    }
  }
}

