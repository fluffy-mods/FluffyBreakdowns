// // Karel Kroeze
// // JobDriver_Maintenance.cs
// // 2016-12-18

using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Fluffy_Breakdowns
{
    public class JobDriver_Maintenance : JobDriver
    {
        #region Fields

        public const int fullRepairTicks = GenDate.TicksPerHour;
        public float startingDurability;

        #endregion Fields

        #region Methods

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve( TargetIndex.A ).FailOnDespawnedNullOrForbidden( TargetIndex.A );
            yield return
                Toils_Goto.GotoThing( TargetIndex.A, PathEndMode.Touch ).FailOnDespawnedNullOrForbidden( TargetIndex.A );

            startingDurability = durability;
            var maintenance = new Toil();
            maintenance.tickAction = delegate
                                         {
                                             Pawn pawn = maintenance.actor;
                                             durability += pawn.GetStatValue( StatDefOf.ConstructionSpeed ) /
                                                           fullRepairTicks;
                                             pawn.skills.Learn( SkillDefOf.Construction, 0.125f );

                                             if ( durability > .99f )
                                             {
                                                 EndJobWith( JobCondition.Succeeded );
                                                 pawn.records.Increment( RecordDefOf.ThingsRepaired );
                                             }
                                         };
            maintenance.WithEffect( TargetThingA.def.repairEffect, TargetIndex.A );
            maintenance.WithProgressBar( TargetIndex.A, progress, true );
            maintenance.defaultCompleteMode = ToilCompleteMode.Never;
            yield return maintenance;
        }

        public float progress()
        {
            // actual progress / possible progress
            return ( durability - startingDurability ) / ( 1f - startingDurability );
        }

        #endregion Methods

        #region Properties

        public CompBreakdownable comp => TargetA.Thing?.TryGetComp<CompBreakdownable>();

        public float durability
        {
            get { return MapComponent_Durability.ForMap( comp.parent.Map ).GetDurability( comp ); }
            set { MapComponent_Durability.ForMap( comp.parent.Map ).SetDurability( comp, value ); }
        }

        #endregion Properties
    }
}
