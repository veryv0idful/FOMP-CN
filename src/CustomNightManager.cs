using System.Collections;
namespace CustomNight;

public static class CustomNightManager
{

  public static bool IsHost => MultiplayerManager.Instance && MultiplayerManager.Instance.IsHost;
  public static bool IsPlaying => GameManager.Instance && GameManager.Instance.isPlaying;
  public static bool IsPlayingAsHost => IsHost && IsPlaying;
  public static float GetSelectedAiLevel(this AnimatronicID animid) => NightSettings.SelectedAiLevels[(int)animid];
  public static bool GracePeriodPassed => GameManager.Instance.currentGameTime.Value >= NightSettings.GracePeriod;
  public static CustomNightSettings NightSettings { get; private set; } = new();

  internal static IEnumerator SetUpCustomNight()
  {
    if (!IsHost) yield break;
    while (!IsPlaying) yield return null;

    foreach (AnimatronicID animID in AiUtil.AnimatronicIDs) animID.SetCustomAiLevel(animID.GetSelectedAiLevel());

    SetNightProperties();
    DebugLogs();

    yield return new WaitForSeconds(NightSettings.GracePeriod);
    if (!IsPlayingAsHost) yield break;

    if (!AnimatronicManager.Instance.CanFindNightData(GameManager.Instance.gameNight, "Freddy", out AnimatronicData _)) InitInvalidNight();
  }

  private static void SetNightProperties()
  {
    if (!IsPlayingAsHost) return;

    GameManager.Instance.gameEndTime.Value = NightSettings.NightTime * 60;

    foreach (PlayerRoles role in PlayerRoleManager.Instance.participatingPlayers)
    {
      if (role == PlayerRoles.PurpleGuy || role == PlayerRoles.None) continue;

      PlayerBehaviour player = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(role);
      player.SetPower(NightSettings.StartingPower);
    }

    if (NightSettings.DisableWordle == 1) WordleSystem.Instance.gameState.Value = WordleSystem.WordleGameState.Loss;
  }
  internal static void OnUpdateFixes()
  {
    if (!IsPlayingAsHost) return;

    if (PowerGenerator.Instance.reserve.Value < NightSettings.StartingReserveEnergy && !GracePeriodPassed && NightSettings.StartingReserveEnergy != 0)
      PowerUtil.SetReservePower(NightSettings.StartingReserveEnergy);

    if (NightSettings.ForceJanitorLms == 1 && PlayerRoleManager.Instance.CountPlayersAlive() > 1)
      PlayerRoleManager.Instance.janitorBehaviour.oxygenLevels.Value = 300;


    foreach (AnimatronicID animID in AiUtil.AnimatronicIDs)
    {
      if (animID.GetSelectedAiLevel() == -1) continue;
      Animatronic animatronic = animID.GetAnimatronicFromID();

      if (animID == AnimatronicID.Freddy)
      {
        NetworkVariable<float> wind = GlobalCameraSystem.Instance.freddyMusicWind;
        wind.Value = Mathf.Min(AiUtil.GetMaxFreddyWind(animID.GetSelectedAiLevel()), wind.Value);
      }

      if (animatronic.timeLeftToMove.Value > animID.GetMaxMovementCoolDownFromAI(animID.GetSelectedAiLevel()) && GracePeriodPassed) animatronic.timeLeftToMove.Value = animID.GetMaxMovementCoolDownFromAI(animID.GetSelectedAiLevel());
    }

    if (NightSettings.InfiniteEnergy == 1 && PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value != 800)
    {
      PlayerRoleManager.Instance.purpleGuyBehaviour.energy.Value = 800;
    }

    foreach (PlayerRoles role in PlayerRoleManager.Instance.participatingPlayers)
    {
      if (role == PlayerRoles.PurpleGuy || role == PlayerRoles.None) continue;
      PlayerBehaviour playerBehaviour = PlayerRoleManager.Instance.GetPlayerBehaviourFromRole(role);
      playerBehaviour.currentPower.Value = Mathf.Min(playerBehaviour.currentPower.Value, NightSettings.MaxPower);
    }

    Zap endo = AnimatronicManager.Instance.zap;

    if (endo.enabled == true && NightSettings.EarlyFreeEndo == 1) endo.isApproachingPlayer.Value = false;
    if (endo.enabled == false) endo.moveSpeed = 0;
  }

  public static void SetCustomAiLevel(this AnimatronicID animID, float ai)
  {
    if (ai == -1) return;
    Animatronic animatronic = animID.GetAnimatronicFromID();

#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET {animID.ToString()} AI {ai} || MOVEMENT {AiUtil.GetMovementCoolDownFromAI(animID, ai)}s");
#endif

    if (ai == 0)
    {
      animatronic.enabled = false;

      if (!animatronic.IsZap) animatronic.Disable();
    }

    if (animID == AnimatronicID.Foxy)
    {
      animatronic.TryCast<Foxy>().currentAttackAttempt.Value = AiUtil.GetFoxyPreAttacks(ai);
    }

    animatronic.currentDifficulty.Value = ai;
    animatronic.currentMovementWaitTime.Value = animID.GetMovementCoolDownFromAI(ai);
    animatronic.timeLeftToMove.Value = Mathf.Max(animID.GetMaxMovementCoolDownFromAI(ai), NightSettings.GracePeriod);
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
  }

  private static void InitialiseSpecial(Animatronic anim)
  {
    anim.Initialise();

    if (anim.enabled && anim.IsZap)
    {
      if (NightSettings.EarlyFreeEndo == 1) anim.gameplayLoop ??= anim.StartCoroutine(anim.GameplayLoop());
    }
  }

  private static void DebugLogs()
  {
#if MELON
    Melon<CustomNightCore>.Logger.Msg($"SET NIGHT TIME TO {NightSettings.NightTime * 60}s");
    Melon<CustomNightCore>.Logger.Msg($"SET GRACE PERIOD TO {NightSettings.GracePeriod}s");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX FREDDY WIND TO {AiUtil.GetMaxFreddyWind(AnimatronicID.Freddy.GetSelectedAiLevel()) * 100}%");
    Melon<CustomNightCore>.Logger.Msg($"SET FOXY PREATTACKS TO {AiUtil.GetFoxyPreAttacks(AnimatronicID.Foxy.GetSelectedAiLevel())} ATTACKS");
    Melon<CustomNightCore>.Logger.Msg($"SET MAX POWER TO {NightSettings.MaxPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING POWER TO {NightSettings.StartingPower}%");
    Melon<CustomNightCore>.Logger.Msg($"SET STARTING RESERVE POWER TO {NightSettings.StartingReserveEnergy}%");
    Melon<CustomNightCore>.Logger.Msg($"SET INFINITE PURPLE ENERGY TO {NightSettings.InfiniteEnergy == 1}");
    Melon<CustomNightCore>.Logger.Msg($"SET DISABLE WORDLE TO {NightSettings.DisableWordle == 1}");
    Melon<CustomNightCore>.Logger.Msg($"SET EARLY FREE ENDO TO {NightSettings.EarlyFreeEndo == 1}");
#endif
  }
}