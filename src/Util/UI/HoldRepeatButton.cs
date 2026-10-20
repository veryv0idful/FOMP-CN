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

    public Action OnRepeat;
    public float initialDelay = 0.4f, startInterval = 0.25f, minInterval = 0.03f, accel = 0.85f;
    bool held; float timer, interval;

    public void Begin()
    {
        held = true; interval = startInterval; timer = initialDelay;
        OnRepeat?.Invoke();
    }
    public void End() => held = false;
    void OnDisable() => held = false;

    void Update()
    {
        if (!held) return;
        timer -= Time.unscaledDeltaTime;
        while (timer <= 0f)
        {
            OnRepeat?.Invoke();
            interval = Mathf.Max(minInterval, interval * accel);
            timer += interval;
        }
    }
}

static class ButtonExtension
{
    public static void AddHoldRepeat(this Button button, Action step)
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