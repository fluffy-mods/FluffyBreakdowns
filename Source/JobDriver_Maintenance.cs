// // Karel Kroeze
// // JobDriver_Maintenance.cs
// // 2016-12-18

using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Fluffy_Breakdowns {
    public class JobDriver_Maintenance: JobDriver {
        #region Fields

        public const int fullRepairTicks = GenDate.TicksPerHour;
        public float startingDurability;

        #endregion Fields

        #region Methods

        public override bool TryMakePreToilReservations(bool errorOnFailed) {
            return pawn.Reserve(job.targetA, job);
        }

        protected override IEnumerable<Toil> MakeNewToils() {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);

            startingDurability = Durability;
            Toil maintenance = new Toil();
            maintenance.tickAction = delegate {
                Pawn pawn = maintenance.actor;
                Durability += pawn.GetStatValue(StatDefOf.ConstructionSpeed) / fullRepairTicks;
                pawn.skills.Learn(SkillDefOf.Construction, 0.125f);

                if (Durability > .99f) {
                    pawn.records.Increment(RecordDefOf.ThingsRepaired);
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
            };
            maintenance.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            maintenance.WithEffect(TargetThingA.def.repairEffect, TargetIndex.A);
            maintenance.WithProgressBar(TargetIndex.A, progress, true);
            maintenance.defaultCompleteMode = ToilCompleteMode.Never;
            yield return maintenance;
        }

        public float progress() {
            // actual progress / possible progress
            return (Durability - startingDurability) / (1f - startingDurability);
        }

        #endregion Methods

        #region Properties

        public CompBreakdownable Comp => TargetA.Thing?.TryGetComp<CompBreakdownable>();

        public float Durability {
            get => MapComponent_Durability.ForMap(Comp?.parent.Map)?.GetDurability(Comp) ?? 0f;
            set => MapComponent_Durability.ForMap(Comp?.parent.Map)?.SetDurability(Comp, value);
        }

        #endregion Properties
    }
}
