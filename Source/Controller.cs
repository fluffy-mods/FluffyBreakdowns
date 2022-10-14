// // Karel Kroeze
// // Controller.cs
// // 2016-12-18

using System.Reflection;
using HarmonyLib;
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
            var harmony = new Harmony( "fluffy.breakdowns" );
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // provide settings
            Settings = GetSettings<Settings>();

            Log.Message( $"Fluffy Breakdowns :: Initialized" );
        }

        public static Settings Settings;
        public static int CheckInterval = BreakdownManager.CheckIntervalTicks;

#if DEBUG
        private const int _baseComponentLifeTime = GenDate.TicksPerDay;
#else
        private const int _baseComponentLifeTime = GenDate.TicksPerSeason;
#endif
        public static float researchFactor = 1f;
        public static int ComponentLifetime => (int)(_baseComponentLifeTime * Settings.ComponentLifetime * researchFactor);

        public override string SettingsCategory() { return "Fluffy Breakdowns"; }
        public override void DoSettingsWindowContents( Rect canvas ) { Settings.DoWindowContents( canvas ); }
        
    }
}
