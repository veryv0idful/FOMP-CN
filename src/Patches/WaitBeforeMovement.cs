using HarmonyLib;
using Random = UnityEngine.Random;

[HarmonyPatch(typeof(Animatronic._WaitBeforeMoving_d__69), "MoveNext")]
public static class WaitBeforeMovingPatch
{
    static bool Prefix(ref Animatronic._WaitBeforeMoving_d__69 __instance, ref bool __result)
    {
        Animatronic animatronic = __instance.__4__this;
        NetworkVariable<float> timeLeftToMove = animatronic.timeLeftToMove;

        int state = __instance.__1__state;
        float dt = Time.deltaTime;

        if (state == 0)
        {
            float waitTime = __instance.waitTime;
            if (waitTime > 2f)
            {
                waitTime += Random.Range(-1f, 1f);
                __instance.waitTime = waitTime;
            }

            timeLeftToMove.Value = waitTime;
            __instance.__1__state = -1;

            bool isFreddy = animatronic.IsFreddy;

            if (isFreddy)
            {
                Freddy freddy = AnimatronicManager.Instance?.freddy;
                __instance._freddy_5__2 = freddy;
                __instance.__1__state = 1;
            }
            else
            {
                __instance.__1__state = 2;
            }

            __instance.__2__current = null;
            __result = true;
            return false;
        }

        if (state == 1)
        {
            __instance.__1__state = -1;

            float slowness = 1f;
            Freddy freddy = __instance._freddy_5__2;
            if (freddy != null)
            {
                slowness = Mathf.Max(0.0001f, freddy.GetSlownessFromWindTeir());
            }

            timeLeftToMove.Value -= dt / slowness;

            if (timeLeftToMove.Value > 0f)
            {
                __instance.__2__current = null;
                __instance.__1__state = 1;
                __result = true;
                return false;
            }

            __instance._freddy_5__2 = null;
            timeLeftToMove.Value = 0f;
            __instance.__2__current = null;
            __result = false;
            return false;
        }

        if (state == 2)
        {
            __instance.__1__state = -1;
            timeLeftToMove.Value -= dt;

            if (timeLeftToMove.Value > 0f)
            {
                __instance.__2__current = null;
                __instance.__1__state = 2;
                __result = true;
                return false;
            }

            timeLeftToMove.Value = 0f;
            __instance.__2__current = null;
        }

        __result = false;
        return false;
    }
}