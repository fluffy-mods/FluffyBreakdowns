// // Karel Kroeze
// // WorkGiver_Maintenance.cs
// // 2016-12-18

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Fluffy_Breakdowns
{
    public class WorkGiver_Maintenance : WorkGiver_Scanner
    {
        #region Properties

        public JobDef JobDefOf_Maintain => DefDatabase<JobDef>.GetNamed( "FluffyBreakdowns_Maintenance" );

        #endregion Properties

        #region Methods

        public override bool HasJobOnThing( Pawn pawn, Thing thing, bool forced = false )
        {
            if ( thing.Faction != pawn.Faction )
                return false;

            if ( pawn.Faction == Faction.OfPlayer && Settings.MaintainHomeOnly &&
                 !pawn.Map.areaManager.Home[thing.Position] && !forced )
                return false;

            if ( thing.IsBurning() )
                return false;

            if ( thing.Map.designationManager.DesignationOn( thing, DesignationDefOf.Deconstruct ) != null )
                return false;

            var twc = thing as ThingWithComps;
            if ( twc == null )
                return false;

            var comp = twc.TryGetComp<CompBreakdownable>();
            if ( comp == null )
                return false;

            if ( !MapComponent_Durability.ForMap( thing.Map ).RequiresMaintenance( comp ) )
                return false;

            if ( !pawn.CanReserveAndReach( thing, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, forced ) )
                return false;

            return true;
        }

        public override Job JobOnThing( Pawn pawn, Thing thing, bool forced )
        {
            // Overriding reservations for player given orders appear to be handled in the ReservationManager and Utilities layers,
            // so there's nothing to do with the forced parameter?
            return new Job( JobDefOf_Maintain, thing );
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal( Pawn pawn )
        {
            return MapComponent_Durability.ForMap( pawn.Map ).potentialMaintenanceThings;
        }

        public override bool ShouldSkip( Pawn pawn, bool forced )
        {
            return !MapComponent_Durability.ForMap( pawn.Map ).potentialMaintenanceThings.Any();
        }

        #endregion Methods
    }
}
