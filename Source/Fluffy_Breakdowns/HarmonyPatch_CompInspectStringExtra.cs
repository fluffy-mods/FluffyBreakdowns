using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    [HarmonyPatch( typeof( CompBreakdownable ), "CompInspectStringExtra" )]
    public static class HarmonyPatch_CompInspectStringExtra
    {
        public static bool Prefix( CompBreakdownable __instance, ref string __result )
        {
            // vanilla can handle broken down
            if ( __instance.BrokenDown )
                return true;

            // we'll handle showing the maintenance
            __result = "FluffyBreakdowns.Maintenance".Translate(__instance.Durability().ToStringPercent());
            return false;
        }
    }
}