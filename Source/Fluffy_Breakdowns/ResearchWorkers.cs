using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace Fluffy_Breakdowns
{
    public class ComponentLifetimeOne : ResearchMod
    {
        // components degrade 100% over 6 months
        public override void Apply() { Controller.ComponentLifetime = GenDate.TicksPerSeason * 2; }
    }

    public class ComponentLifetimeTwo : ResearchMod
    {
        // components degrade 100% over a year
        public override void Apply() { Controller.ComponentLifetime = GenDate.TicksPerYear; }
    }
}
