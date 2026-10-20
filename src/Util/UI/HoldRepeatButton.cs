using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomNight.Util.UI;

#if MELON
[RegisterTypeInIl2Cpp]
#endif
public class HoldRepeat : MonoBehaviour
{
    public HoldRepeat(IntPtr ptr) : base(ptr) { }

    public Action<float> OnRepeat;
    public float initialDelay = 0.2f, startInterval = 0.25f, minInterval = 0.05f, accel = 0.85f;
    bool held;
    float timer;
    float interval;
    public float TotalHeld { get; private set; }

    public void Begin()
    {
        held = true;
        interval = startInterval;
        timer = initialDelay;
        OnRepeat?.Invoke(0);
    }
    public void End()
    {
        held = false;
        TotalHeld = 0f;
    }

    void OnDisable()
    {
        held = false;
        TotalHeld = 0f;
    }

    void Update()
    {
        if (!held) return;
        timer -= Time.deltaTime;
        while (timer <= 0f)
        {
            OnRepeat?.Invoke(TotalHeld);
            interval = Mathf.Max(minInterval, interval * accel);
            timer += interval;
        }
        TotalHeld += Time.deltaTime;
    }
}

static class ButtonExtension
{
    public static void AddHoldRepeat(this Button button, Action<float> step)
    {
        HoldRepeat hold = button.gameObject.AddComponent<HoldRepeat>();
        hold.OnRepeat += step;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        Add(EventTriggerType.PointerDown, hold.Begin);
        Add(EventTriggerType.PointerUp,   hold.End);
        Add(EventTriggerType.PointerExit, hold.End);
        return;

        void Add(EventTriggerType type, Action action)
        {
            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener((UnityAction<BaseEventData>)(_ => action()));
            trigger.triggers.Add(entry);
        }
    }
}