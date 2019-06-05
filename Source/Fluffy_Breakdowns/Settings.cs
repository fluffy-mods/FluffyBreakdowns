using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public class Settings : ModSettings
    {
        public static float MaintenanceThreshold = .7f;
        public static bool  MaintainHomeOnly     = true;
        public static float ComponentLifetime    = 1f;
        public static float NotUsedFactor        = 1 / 3f;

        public void DoWindowContents( Rect canvas )
        {
            var list = new Listing_Standard();
            list.ColumnWidth = canvas.width;
            list.Begin( canvas );

            // difficulty setting
            list.Label( "FluffyBreakdowns.ComponentLifetimeFactor".Translate( ComponentLifetime.ToStringPercent() ) );
            ComponentLifetime = list.Slider( ComponentLifetime, .1f, 10f );
            if ( ComponentLifetime < .5f || ComponentLifetime > 2f )
            {
                var msg = ComponentLifetime > 1
                    ? "Fluffy.Breakdowns.HighComponentLifetime".Translate()
                    : "Fluffy.Breakdowns.LowComponentLifetime".Translate();
                GUI.color = Color.red;
                Text.Font = GameFont.Tiny;
                list.Label( msg );
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
            }

            // not used degradation factor
            list.Label( "FluffyBreakdowns.NotUsedFactor".Translate( NotUsedFactor.ToStringPercent() ) );
            NotUsedFactor = list.Slider( NotUsedFactor, 0f, 1f );

            // maintenance threshold
            list.Label( "FluffyBreakdowns.MaintenanceThreshold".Translate( MaintenanceThreshold.ToStringPercent() ) );
            MaintenanceThreshold = list.Slider( MaintenanceThreshold, 0, 1 );
            if ( MaintenanceThreshold < .3f )
            {
                GUI.color = Color.red;
                Text.Font = GameFont.Tiny;
                list.Label( "FluffyBreakdowns.LowMaintenanceThreshold".Translate() );
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
            }

            list.Gap();

            // maintain home only?
            list.CheckboxLabeled( "FluffyBreakdowns.MaintenanceHome".Translate(),
                                  ref MaintainHomeOnly,
                                  "FluffyBreakdowns.MaintenanceHomeTip".Translate() );
            list.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look( ref MaintenanceThreshold, "threshold", .7f );
            Scribe_Values.Look( ref MaintainHomeOnly, "homeOnly", true );
            Scribe_Values.Look( ref ComponentLifetime, "componentLifetime", 1f );
            Scribe_Values.Look( ref NotUsedFactor, "notUsedFactor", 1 / 3f );
        }
    }
}