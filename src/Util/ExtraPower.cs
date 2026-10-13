namespace CustomNight;

#if MELON
[RegisterTypeInIl2Cpp]
#endif
public class ExtraPower : MonoBehaviour
{

    public float CurrentExtraPower = 0f;
    public NetworkVariable<float> CurrentPower => Player.currentPower;
    public PlayerBehaviour Player;

    public void Awake()
    {
        if (!MultiplayerManager.Instance.IsHost || !GameManager.Instance || !GameManager.Instance.isPlaying) { Destroy(this); return; }
        if (!TryGetComponent(out Player)) { Destroy(this); return; }
    }
    public void Update()
    {
        if (CurrentPower.Value < CustomNightPanel.MaxPower && CurrentExtraPower > 0)
        {
            float CurrentPowerIncrease = Mathf.Min(CustomNightPanel.MaxPower - CurrentPower.Value, CurrentExtraPower);
            CurrentPower.Value += CurrentPowerIncrease;
            CurrentExtraPower -= CurrentPowerIncrease;
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
}