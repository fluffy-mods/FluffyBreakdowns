using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public class Settings : ModSettings
    {
        public float MaintenanceThreshold = .7f;
        public bool MaintainHomeOnly = true;
        public float ComponentLifetime = 1f;

        public void DoWindowContents( Rect canvas )
        {
            Listing_Standard list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin( canvas );

            // difficulty setting
            list.Label( "FluffyBreakdowns.ComponentLifetimeRatio".Translate( ComponentLifetime.ToStringPercent() ) );
            ComponentLifetime = list.Slider( ComponentLifetime, .5f, 2f );

            // maintenance threshold
            list.Label( "FluffyBreakdowns.MaintenanceThreshold".Translate( MaintenanceThreshold.ToStringPercent() ) );
            MaintenanceThreshold = list.Slider( MaintenanceThreshold, 0, 1 );
            if ( MaintenanceThreshold < .3f )
            {
                GUI.contentColor = Color.red;
                Text.Font = GameFont.Tiny;
                list.Label( "FluffyBreakdowns.LowMaintenanceThreshold".Translate() );
                Text.Font = GameFont.Small;
                GUI.contentColor = Color.white;
            }
            list.Gap();

            // maintain home only?
            list.CheckboxLabeled( "FluffyBreakdowns.MaintenanceHome".Translate(),
                                  ref MaintainHomeOnly,
                                  "FluffyBreakdowns.MaintenanceHomeTip".Translate() );
            list.End();
        }

        #region Overrides of ModSettings

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look( ref MaintenanceThreshold, "threshold", .7f );
            Scribe_Values.Look( ref MaintainHomeOnly, "homeOnly", true );
            Scribe_Values.Look( ref ComponentLifetime, "componentLifetime", 1f );
        }

        #endregion
    }
}