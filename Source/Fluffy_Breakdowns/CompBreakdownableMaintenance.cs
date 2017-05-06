// // Karel Kroeze
// // CompMaintainanceBreakdown.cs
// // 2016-12-18

using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public static class CompBreakdownableMaintenance
    {
        internal static float Durability(this CompBreakdownable _this )
        {
            return MapComponent_Durability.ForMap( _this.parent.Map ).GetDurability( _this );
        }

        internal static void Durability( this CompBreakdownable _this, float value )
        {
            MapComponent_Durability.ForMap( _this.parent.Map ).SetDurability( _this, value );
        }

        internal static void DurabilityLoss( this CompBreakdownable _this, float loss )
        {
            _this.Durability( _this.Durability() - loss );
        }

        public static bool InUse( this CompBreakdownable _this )
        {
                // TODO: Add back in if/when CCL gets out.
                // CCL LowIdleDraw; if not null and in lower power mode, assume not in use.
                // var compLowIdleDraw = parent.GetComp<CompPowerLowIdleDraw>();
                // if ( compLowIdleDraw != null && compLowIdleDraw.LowPowerMode )
                //     return false;

                // CompPowered; if not null and powered off (for any reason), assume not in use.
                var compPowerTrader = _this.parent.GetComp<CompPowerTrader>();
                if ( compPowerTrader != null && !compPowerTrader.PowerOn )
                    return false;

                // TODO: figure out why this section causes a hard CTD
                //// CompFueled; if not null and not consumed fuel between ticks, assume not in use
                //var compFueled = parent.GetComp<CompRefuelable>();
                //if ( compFueled != null && compFueled.FuelPercent >= lastFuelAmount )
                //    return false;
                //if ( compFueled != null )
                //    lastFuelAmount = compFueled.FuelPercent;

                // nothing stuck, assume in use.
                return true;
        }

        public static void DoBreakdownMaintenance( this CompBreakdownable _this )
        {
            _this.DoBreakdown();

            // reset durability
            _this.Durability( 1f );
        }
    }
}
