using CommunityCoreLibrary;
using RimWorld;
using Verse;

namespace Fluffy_Breakdowns
{
    public class CompBreakdownableMaintenance : CompBreakdownable
    {
        #region Properties
        private int componentLifetime => MapComponent_Durability.componentLifetime;
        private int checkinterval => BreakdownManager.CheckIntervalTicks;
        private float lastFuelAmount = 0f;

        private float durability
        {
            get
            {
                return MapComponent_Durability.GetDurability( this );
            }
            set
            {
                MapComponent_Durability.SetDurability( this, value );
            }
        }

        #endregion Properties

        #region Methods

        public void _checkForBreakdown()
        {
            if ( !BrokenDown )
            {
                var durabilityLoss = (float)checkinterval / (float)componentLifetime;
                if ( !InUse )
                    durabilityLoss *= MapComponent_Durability.notUsedFactor;
                durability -= durabilityLoss;

                // durability below 50%, increasing chance of breakdown ( up to almost guaranteed at 1% (minimum) maintenance.
                if ( durability < .5 && Rand.MTBEventOccurs( GenDate.TicksPerYear * durability, 1f, checkinterval ) )
                    DoBreakdown();
            }
        }

        public bool InUse
        {
            get
            {
                // CCL LowIdleDraw; if not null and in lower power mode, assume not in use.
                var compLowIdleDraw = parent.GetComp<CompPowerLowIdleDraw>();
                if ( compLowIdleDraw != null && compLowIdleDraw.LowPowerMode )
                    return false;

                // CompPowered; if not null and powered off (for any reason), assume not in use.
                var compPowerTrader = parent.GetComp<CompPowerTrader>();
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
        }

        public new void DoBreakdown()
        {
            base.DoBreakdown();

            // reset durability
            durability = 1f;
        }
        
        public string _compInspectStringExtra()
        {
            string text = "";
            if ( this.BrokenDown )
            {
                text += "BrokenDown".Translate() + "\n";
            }

            text += "FluffyBreakdowns.Maintenance".Translate( durability.ToStringPercent() );

            return text;
        }

        #endregion Methods
    }
}