using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityCoreLibrary;
using CommunityCoreLibrary.UI;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public class ModConfigurationMenu_Breakdowns : ModConfigurationMenu
    {
        public static float maintenanceThreshold = .9f;

        // return window height
        public override float DoWindowContents( UnityEngine.Rect rect )
        {
            Rect labelRect = new Rect( 0f, 0f, rect.width, 30f );
            Rect sliderRect = new Rect( 0f, 30f, rect.width, 30f );
            Widgets.Label( labelRect, "FluffyBreakdowns.MaintenanceThreshold".Translate( maintenanceThreshold.ToStringPercent() ) );
            maintenanceThreshold = GUI.HorizontalSlider( sliderRect, maintenanceThreshold, 0f, 1f );
            TooltipHandler.TipRegion( new Rect( 0f, 0f, rect.width, 60f ), "FluffyBreakdowns.MaintenanceThresholdTip".Translate() );

            if ( maintenanceThreshold < .5 )
            {
                CCL_Widgets.Label( new Rect( 0f, 60, rect.width, 30f ), "FluffyBreakdowns.LowMaintenanceThreshold".Translate(), Color.red, GameFont.Tiny, TextAnchor.UpperLeft );
                return 90;
            }

            return 60f;
        }

        public override void ExposeData()
        {
            Scribe_Values.LookValue( ref maintenanceThreshold, "maintenanceThreshold", .9f );
        }
    }
}
