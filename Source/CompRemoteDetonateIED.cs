using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace RemoteDetonateIED
{
    public class CompProperties_RemoteDetonateIED : CompProperties
    {
        public CompProperties_RemoteDetonateIED()
        {
            this.compClass = typeof(CompRemoteDetonateIED);
        }
    }

    public class CompRemoteDetonateIED : ThingComp
    {
        private CompExplosive Explosive => parent?.GetComp<CompExplosive>();

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent?.Spawned != true) yield break;
            if (!parent.Faction?.IsPlayer ?? true) yield break;

            var ce = Explosive;
            if (ce == null) yield break;

            var cmd = new Command_Action
            {
                defaultLabel = "Detonate",
                defaultDesc = "Remotely detonate this explosive. Hold Shift for instant detonation.",
                icon = ContentFinder<Texture2D>.Get("UI/Commands/Detonate", true),
                action = () => OnClickDetonate(ce)
            };

            yield return cmd;
        }

        private void OnClickDetonate(CompExplosive ce)
        {
            if (ce == null || parent?.Map == null) return;
            bool shift = (Event.current?.shift ?? false);

            if (shift)
            {
                try
                {
                    var m = typeof(CompExplosive).GetMethod("Detonate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (m != null)
                    {
                        m.Invoke(ce, null);
                    }
                    else
                    {
                        ce.StartWick(null);
                        var fi = typeof(CompExplosive).GetField("wickTicksLeft", BindingFlags.Instance | BindingFlags.NonPublic);
                        if (fi != null) fi.SetValue(ce, 1);
                    }
                }
                catch
                {
                    ce.StartWick(null);
                }
            }
            else
            {
                ce.StartWick(null);
            }
        }
    }
}