using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RemoteDetonateIED
{
    public class CompProperties_ProximityGate : CompProperties
    {
        public CompProperties_ProximityGate()
        {
            compClass = typeof(CompProximityGate);
        }
    }

    public class CompProximityGate : ThingComp
    {
        public bool proximityEnabled = true;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad)
                proximityEnabled = true; // default ON on new build
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref proximityEnabled, "proximityEnabled", true);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent?.Spawned != true) yield break;
            if (!parent.Faction?.IsPlayer ?? true) yield break;

            yield return new Command_Toggle
            {
                defaultLabel = proximityEnabled ? "Proximity:ON" : "Proximity:OFF",
                defaultDesc = "Toggle whether this trap can trigger on proximity. OFF prevents automatic springing; manual detonation still works.",
                isActive = () => proximityEnabled,
                toggleAction = () =>
                {
                    proximityEnabled = !proximityEnabled;
                },
                icon = ContentFinder<Texture2D>.Get(
                    proximityEnabled ? "UI/Commands/Proximity" : "UI/Commands/ProximityOff",
                    true
                )
            };
        }
    }
}
