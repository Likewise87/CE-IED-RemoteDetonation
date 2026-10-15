using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace RemoteDetonateIED
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var h = new Harmony("TSA.CE.IEDRemoteDetonation");

            var springSub = AccessTools.Method(typeof(Building_TrapExplosive), "SpringSub", new[] { typeof(Pawn) });
            if (springSub != null)
            {
                h.Patch(
                    springSub,
                    prefix: new HarmonyMethod(typeof(Patch_TrapExplosive_SpringSub), nameof(Patch_TrapExplosive_SpringSub.Prefix))
                );
            }
        }
    }

    public static class Patch_TrapExplosive_SpringSub
    {
        public static bool Prefix(Building_TrapExplosive __instance, Pawn p)
        {
            var gate = __instance.GetComp<CompProximityGate>();

            if (gate != null && gate.proximityEnabled == false)
            {
                return false; // skip original — trap remains armed
            }

            return true; // run original
        }
    }
}
