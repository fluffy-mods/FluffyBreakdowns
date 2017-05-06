// // Karel Kroeze
// // Controller.cs
// // 2016-12-18

using System.Reflection;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace Fluffy_Breakdowns
{
    public class Controller : Mod
    {
        public Controller( ModContentPack content ) : base( content )
        {
            // detour CheckForBreakdown
            var harmony = HarmonyInstance.Create( "fluffy.breakdowns" );
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // provide settings
            Settings = GetSettings<Settings>();

            Log.Message( $"Fluffy Breakdowns :: Initialized" );
        }

        public static Settings Settings;
        public static int CheckInterval = BreakdownManager.CheckIntervalTicks;

#if DEBUG
        public static int ComponentLifetime = GenDate.TicksPerDay;
#else
        public static int ComponentLifetime = GenDate.TicksPerSeason;
#endif

        public override string SettingsCategory() { return "Fluffy Breakdowns"; }
        public override void DoSettingsWindowContents( Rect canvas ) { Settings.DoWindowContents( canvas ); }

        // todo; verify that vanilla now correctly injects new (map)components into existing save games.
        //public override void MapLoaded( Map map )
        //{
        //    base.MapLoaded( map );

        //    if ( map.GetComponent<MapComponent_Durability>() == null )
        //        map.components.Add( new MapComponent_Durability( map ) );
        //}
        
    }
}
