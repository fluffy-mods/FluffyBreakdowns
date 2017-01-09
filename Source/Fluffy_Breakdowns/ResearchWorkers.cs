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
    public static class TynanPlease
    {
        public static FieldInfo specialActionFieldInfo = typeof( ResearchMod ).GetField( "specialAction",
                                                                                         BindingFlags.Instance |
                                                                                         BindingFlags.NonPublic );

        public static FieldInfo researchModsFieldInfo = typeof( ResearchProjectDef ).GetField( "researchMods",
                                                                                      BindingFlags.Instance |
                                                                                      BindingFlags.NonPublic );

        public static void SetActions()
        {
            // check fieldinfo
            if ( researchModsFieldInfo == null )
                throw new Exception( "Failed to get fieldInfo for RimWorld.ResearchProjectDef.researchMods" );

            // first research
            var ComponentLifetimeOne = DefDatabase<ResearchProjectDef>.GetNamed( "FluffyBreakdowns_ComponentLifetimeOne" );
            var researchModsOne = new List<ResearchMod>();
            researchModsOne.Add( new ComponentLifetimeOne() );
            researchModsFieldInfo.SetValue( ComponentLifetimeOne, researchModsOne );

            // second research
            var ComponentLifetimeTwo = DefDatabase<ResearchProjectDef>.GetNamed( "FluffyBreakdowns_ComponentLifetimeTwo" );
            var researchModsTwo = new List<ResearchMod>();
            researchModsTwo.Add( new ComponentLifetimeTwo() );
            researchModsFieldInfo.SetValue( ComponentLifetimeTwo, researchModsTwo );
        }
    }

    public class ComponentLifetimeOne : ResearchMod
    {
        public ComponentLifetimeOne()
        {
            // jump through hoops because Apply() isn't virtual, and specialAction is private.
            if ( TynanPlease.specialActionFieldInfo == null )
                throw new Exception( "Failed to get fieldInfo for Verse.ResearchMod.specialAction" );

            Action specialAction = TynanPlease.specialActionFieldInfo.GetValue( this ) as Action;
            specialAction += Apply;
            TynanPlease.specialActionFieldInfo.SetValue( this, specialAction );

            if ( TynanPlease.specialActionFieldInfo.GetValue( this ) == null )
                throw new Exception( "Failed to insert Apply() logic in specialAction" );
        }

        // Tynan, please.
        // todo: change to override when vanilla is fixed.
        public new void Apply() { MapComponent_Durability.ComponentLifetime = GenDate.DaysPerSeason * 2; }
    }

    public class ComponentLifetimeTwo : ResearchMod
    {
        public ComponentLifetimeTwo()
        {
            // jump through hoops because Apply() isn't virtual, and specialAction is private.
            if ( TynanPlease.specialActionFieldInfo == null )
                throw new Exception( "Failed to get fieldInfo for Verse.ResearchMod.specialAction" );

            Action specialAction = TynanPlease.specialActionFieldInfo.GetValue( this ) as Action;
            specialAction += Apply;
            TynanPlease.specialActionFieldInfo.SetValue( this, specialAction );

            if ( TynanPlease.specialActionFieldInfo.GetValue( this ) == null )
                throw new Exception( "Failed to insert Apply() logic in specialAction" );
        }

        // Tynan, please.
        // todo: change to override when vanilla is fixed.
        public new void Apply() { MapComponent_Durability.ComponentLifetime = GenDate.DaysPerYear; }
    }
}
