using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    [HarmonyPatch( typeof( CompBreakdownable ), "CheckForBreakdown" )]
    public class HarmonyPatch_CheckForBreakdown
    {
        public static bool Prefix( CompBreakdownable __instance )
        {
            if (!__instance.BrokenDown)
            {
                float durabilityLoss = (float)Controller.CheckInterval / (float)Controller.ComponentLifetime;
                if (!__instance.InUse())
                    durabilityLoss *= Settings.NotUsedFactor;

                durabilityLoss *= __instance.MaintenanceComplexityFactor();

                __instance.DurabilityLoss( durabilityLoss );

                // durability below 50%, increasing chance of breakdown ( up to almost guaranteed at 1% (minimum) maintenance.
                if (__instance.Durability() < .5 && Rand.MTBEventOccurs(GenDate.TicksPerYear * __instance.Durability(), 1f, Controller.CheckInterval ))
                    __instance.DoBreakdownMaintenance();
            }
            
            // return false to stop vanilla CheckForBreakdown execution
            return false;
        }
    }
}