using Verse;

namespace Fluffy_Breakdowns {
    public class ComponentLifetimeOne: ResearchMod {
        public override void Apply() {
            Logger.Debug("Applying research 1");
            Logger.Debug("\t Current lifetime: " + Controller.ComponentLifetime);
            Controller.researchFactor = 2;
            Logger.Debug("\t New lifetime: " + Controller.ComponentLifetime);
        }
    }

    public class ComponentLifetimeTwo: ResearchMod {
        public override void Apply() {
            Logger.Debug("Applying research 2");
            Logger.Debug("\t Current lifetime: " + Controller.ComponentLifetime);
            Controller.researchFactor = 4;
            Logger.Debug("\t New lifetime: " + Controller.ComponentLifetime);
        }
    }
}
