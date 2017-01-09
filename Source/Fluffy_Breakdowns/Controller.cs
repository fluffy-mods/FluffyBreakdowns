// // Karel Kroeze
// // Controller.cs
// // 2016-12-18

using HugsLib;
using HugsLib.Settings;
using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public class Controller : ModBase
    {
        public Controller() { }

        public static SettingHandle<int> MaintenanceThresholdHandle;
        public static SettingHandle<bool> MaintenanceHomeOnly;
        private string _modIdentifier = "FluffyBreakdowns";

        public override string ModIdentifier
        {
            get { return _modIdentifier; }
        }

        public static float MaintenanceThreshold => MaintenanceThresholdHandle / 100f;

        public override void MapLoaded( Map map )
        {
            base.MapLoaded( map );

            if ( map.GetComponent<MapComponent_Durability>() == null )
                map.components.Add( new MapComponent_Durability( map ) );
        }

        public override void DefsLoaded()
        {
            MaintenanceThresholdHandle = Settings.GetHandle( "maintenanceThreshold",
                                                             "FluffyBreakdowns.MaintenanceThreshold".Translate(),
                                                             "FluffyBreakdowns.MaintenanceThresholdTip".Translate(), 70,
                                                             Validators.IntRangeValidator( 0, 100 ) );

            // set up warning for low maintenance threshold.
            SettingHandle<bool> Warning = Settings.GetHandle( "maintenanceThresholdWarning", "", null, false );
            Warning.Unsaved = true;
            Warning.CustomDrawer = WarningDrawer;
            Warning.VisibilityPredicate = () => MaintenanceThreshold < .3f;

            MaintenanceHomeOnly = Settings.GetHandle( "maintenanceHome",
                                                      "FluffyBreakdowns.MaintenanceHome".Translate(),
                                                      "FluffyBreakdowns.MaintenanceHomeTip".Translate(), true );

            // manually set the researchMod actions
            TynanPlease.SetActions();
        }

        private bool WarningDrawer( Rect canvas )
        {
            TextAnchor oldAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Color oldColor = GUI.color;
            GUI.color = Color.red;
            Widgets.Label( canvas, "FluffyBreakdowns.LowMaintenanceThreshold".Translate() );
            Text.Anchor = oldAnchor;
            GUI.color = oldColor;
            return false;
        }
    }
}
