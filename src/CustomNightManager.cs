using System.Collections;
namespace CustomNight;

public static class CustomNightManager
{

  public static bool IsHost => MultiplayerManager.Instance && MultiplayerManager.Instance.IsHost;
  public static bool IsPlaying => GameManager.Instance && GameManager.Instance.isPlaying;
  public static bool IsPlayingAsHost => IsHost && IsPlaying;

  internal static IEnumerator SetUpCustomNight()
  {
    if (!IsHost) yield break;
    while (!IsPlaying) yield return null;

    foreach (AnimatronicID AnimID in AiUtil.AnimatronicIDs) AnimID.SetCustomAiLevel(AnimID.GetSelectedAiLevel());

    SetNightProperties();
    DebugLogs();

    yield return new WaitForSeconds(CustomNightPanel.GracePeriod);

    if (!AnimatronicManager.Instance.CanFindNightData(GameManager.Instance.gameNight, "Freddy", out var _)) InitInvalidNight();
  }

  private static void SetNightProperties()
  {
    if (!IsPlayingAsHost) return;

    GameManager.Instance.gameEndTime.Value = CustomNightPanel.NightTime * 60;

    foreach (PlayerRoles Role in PlayerRoleManager.Instance.participatingPlayers)
    {
      if (Role == PlayerRoles.PurpleGuy || Role == PlayerRoles.None) continue;

      PlayerBehaviour Player = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(Role);
      Player.SetPower(CustomNightPanel.StartingPower);
    }

    if (CustomNightPanel.DisableWordle == 1) WordleSystem.Instance.gameState.Value = WordleSystem.WordleGameState.Loss;
  }
  internal static void OnUpdateFixes()
  {
    if (!IsPlayingAsHost) return;

    if (PowerGenerator.Instance.reserve.Value < CustomNightPanel.StartingReserveEnergy && GameManager.Instance.currentGameTime.Value < CustomNightPanel.GracePeriod && CustomNightPanel.StartingReserveEnergy != 0)
      PowerUtil.SetReservePower(CustomNightPanel.StartingReserveEnergy);

    if (CustomNightPanel.ForceJanitorLMS == 1 && PlayerRoleManager.Instance.CountPlayersAlive() > 1)
      PlayerRoleManager.Instance.janitorBehaviour.oxygenLevels.Value = 300;


    foreach (AnimatronicID AnimID in AiUtil.AnimatronicIDs)
    {
      if (AnimID.GetSelectedAiLevel() == -1) continue;
      Animatronic Animatronic = AnimID.GetAnimatronicFromID();

      if (AnimID == AnimatronicID.Freddy)
      {
        NetworkVariable<float> Wind = GlobalCameraSystem.Instance.freddyMusicWind;
        Wind.Value = Mathf.Min(AiUtil.GetMaxFreddyWind(AnimID.GetSelectedAiLevel()), Wind.Value);
      }

      if (Animatronic.timeLeftToMove.Value > AnimID.GetMaxMovementCoolDownFromAI(AnimID.GetSelectedAiLevel()) && CustomNightPanel.GracePeriodPassed) Animatronic.timeLeftToMove.Value = AnimID.GetMaxMovementCoolDownFromAI(AnimID.GetSelectedAiLevel());
    }

    if (CustomNightPanel.InfiniteEnergy == 1 && PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value != 800)
    {
      PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value = 800;
    }

    foreach (PlayerRoles Role in PlayerRoleManager.Instance.participatingPlayers)
    {
      if (Role == PlayerRoles.PurpleGuy || Role == PlayerRoles.None) continue;
      PlayerBehaviour PlayerBehaviour = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(Role);
      PlayerBehaviour.currentPower.Value = Mathf.Min(PlayerBehaviour.currentPower.Value, CustomNightPanel.MaxPower);
    }

    if (AnimatronicID.Endo.GetAnimatronicFromID().enabled == true && CustomNightPanel.EarlyFreeEndo == 1) AnimatronicID.Endo.GetAnimatronicFromID().TryCast<Zap>().isApproachingPlayer.Value = false;
    if (AnimatronicID.Endo.GetAnimatronicFromID().enabled == false) AnimatronicID.Endo.GetAnimatronicFromID().TryCast<Zap>().moveSpeed = 0;
  }

  public static void SetCustomAiLevel(this AnimatronicID AnimID, float AI)
  {
    if (AI < 0) return;
    Animatronic Animatronic = AnimID.GetAnimatronicFromID();

#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET {AnimID.ToString()} AI {AI} || MOVEMENT {AiUtil.GetMovementCoolDownFromAI(AnimID, AI)}s");
#endif

    if (AI == 0)
    {
      Animatronic.enabled = false;

      if (!Animatronic.IsZap) Animatronic.Disable();
    }

    if (AnimID == AnimatronicID.Foxy)
    {
      Animatronic.TryCast<Foxy>().currentAttackAttempt.Value = AiUtil.GetFoxyPreAttacks(AI);
    }

    Animatronic.currentDifficulty.Value = AI;
    Animatronic.currentMovementWaitTime.Value = AnimID.GetMovementCoolDownFromAI(AI);
    Animatronic.timeLeftToMove.Value = Mathf.Max(AnimID.GetMaxMovementCoolDownFromAI(AI), CustomNightPanel.GracePeriod);
  }

  public static void InitInvalidNight()
  {
    foreach (Animatronic anim in AnimatronicManager.Instance.Animatronics)
    {
      if (anim.gameplayLoop != null || anim.IsFoxy)
        continue;

      if (anim.IsZap || anim.IsGoldenFreddy)
        InitialiseSpecial(anim);
      else
        anim.gameplayLoop = anim.StartCoroutine(anim.GameplayLoop());
    }

    static void InitialiseSpecial(Animatronic anim)
    {
      anim.Initialise();

      if (anim.enabled && anim.IsZap)
      {
        if (CustomNightPanel.EarlyFreeEndo == 1)
          anim.gameplayLoop ??= anim.StartCoroutine(anim.GameplayLoop());
      }
    }
  }

  private static void DebugLogs()
  {
#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET NIGHT TIME TO {CustomNightPanel.NightTime * 60}s");
    Melon<CustomNightCore>.Logger.Msg($"SET GRACE PERIOD TO {CustomNightPanel.GracePeriod}s");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX FREDDY WIND TO {AiUtil.GetMaxFreddyWind(AnimatronicID.Freddy.GetSelectedAiLevel()) * 100}%");
    Melon<CustomNightCore>.Logger.Msg($"SET FOXY PREATTACKS TO {AiUtil.GetFoxyPreAttacks(AnimatronicID.Foxy.GetSelectedAiLevel())} ATTACKS");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX POWER TO {CustomNightPanel.MaxPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING POWER TO {CustomNightPanel.StartingPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING RESERVE POWER TO {CustomNightPanel.StartingReserveEnergy}%");
    Melon<CustomNightCore>.Logger.Msg($"SET INFINITE PURPLE ENERGY TO {CustomNightPanel.InfiniteEnergy == 1}");
    Melon<CustomNightCore>.Logger.Msg($"SET DISABLE WORDLE TO {CustomNightPanel.DisableWordle == 1}");
    Melon<CustomNightCore>.Logger.Msg($"SET EARLY FREE ENDO TO {CustomNightPanel.EarlyFreeEndo == 1}");
#endif
  }
}

