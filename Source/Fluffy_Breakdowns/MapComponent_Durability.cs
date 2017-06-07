// // Karel Kroeze
// // MapComponent_Durability.cs
// // 2016-12-18

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

// todo: split off config/static parts into a gameComponent

namespace Fluffy_Breakdowns
{
    public class MapComponent_Durability : MapComponent
    {
        public MapComponent_Durability( Map map ) : base( map ) { }

        public static MapComponent_Durability ForMap( Map map )
        {
            if (map == null)
                return null;

            var comp = map.GetComponent<MapComponent_Durability>();
            if ( comp == null )
            {
                comp = new MapComponent_Durability( map );
                map.components.Add( comp );
            }
            return comp;
        }

        #region Fields

        public const float notUsedFactor = 1 / 3f;
        private const int _moteIntervalRequiresCriticalRepair = 15;
        private const int _moteIntervalRequiresRepair = 30;
        private Dictionary<CompBreakdownable, float> _durabilities = new Dictionary<CompBreakdownable, float>();
        private List<DurabilityPair> _durabilityScribeHelper;

#endregion Fields

        public class DurabilityPair : IExposable
        {
            public float durability = 1f;
            public ThingWithComps thing;

            public DurabilityPair()
            {
                // scribe
            }

            public DurabilityPair( ThingWithComps thing, float durability = 1f )
            {
                this.thing = thing;
                this.durability = durability;
            }

            public void ExposeData()
            {
                Scribe_References.Look( ref thing, "thing" );
                Scribe_Values.Look( ref durability, "durability" );
            }
        }

#region Properties

        public IEnumerable<Thing> potentialMaintenanceThings
        {
            get
            {
                return _durabilities.Select( p => p.Key.parent ).Where( twc => twc?.Spawned ?? false ).Cast<Thing>();
            }
        }

#endregion Properties

#region Methods

        public override void ExposeData()
        {
            // create a list of saveable thing/durability pairs
            if ( Scribe.mode == LoadSaveMode.Saving )
            {
                _durabilityScribeHelper =
                    _durabilities.Select( pair => new DurabilityPair( pair.Key.parent, pair.Value ) ).ToList();
            }

            // save/load the list
            Scribe_Collections.Look( ref _durabilityScribeHelper, "durabilities", LookMode.Deep );

            // reconstruct durability dictionary from saved list
            if ( Scribe.mode == LoadSaveMode.PostLoadInit )
            {
                foreach ( DurabilityPair helper in _durabilityScribeHelper )
                {
                    var comp = helper?.thing?.TryGetComp<CompBreakdownable>();
                    if ( comp != null && !_durabilities.ContainsKey( comp ) )
                    {
                        _durabilities.Add( comp, helper.durability );
                    }
                }
            }
        }

        public float GetDurability( CompBreakdownable comp )
        {
            float durability;
            if ( !_durabilities.TryGetValue( comp, out durability ) )
            {
                durability = 1f;
                _durabilities.Add( comp, durability );
            }
            return durability;
        }

        public float GetDurability( Building building )
        {
            var comp = building.TryGetComp<CompBreakdownable>();
            if ( comp == null )
                return 1f;
            else
                return GetDurability( comp );
        }

        public bool RequiresMaintenance( CompBreakdownable comp )
        {
            return GetDurability( comp ) < Controller.Settings.MaintenanceThreshold;
        }

        public void SetDurability( CompBreakdownable comp, float durability )
        {
            _durabilities[comp] = Mathf.Clamp( durability, .001f, 1f );
        }

        public void SetDurability( Building building, float durability )
        {
            var comp = building.TryGetComp<CompBreakdownable>();
            if ( comp != null )
                SetDurability( comp, durability );
        }

#if DEBUG
        public override void MapComponentOnGUI()
        {
            base.MapComponentOnGUI();

            string status = $"Threshold: {Controller.Settings.MaintenanceThreshold}\nHomeOnly: {Controller.Settings.MaintainHomeOnly}\n";
            status += $"ComponentLifetime: {Controller.ComponentLifetime}\nCheckInterval: {Controller.CheckInterval}\n";
            status += string.Join( "\n", _durabilities.Select( p => p.Key.parent.LabelCap + ": " + p.Value.ToStringPercent() ).ToArray() );
            
            Rect statusRect = new Rect( 0f, Screen.height * 1/4f, Screen.width * 1/2f, Screen.height * 1/2f );
            Widgets.Label( statusRect, status );
        }
#endif

        public override void MapComponentTick()
        {
            base.MapComponentTick();

            int tick = Find.TickManager.TicksGame;
            var orphaned = new List<CompBreakdownable>();

            foreach ( KeyValuePair<CompBreakdownable, float> _dur in _durabilities )
            {
                float durability = _dur.Value;
                CompBreakdownable comp = _dur.Key;
                if ( comp?.parent?.Spawned ?? false )
                {
                    if ( durability < .5 && ( tick + comp.GetHashCode() ) % _moteIntervalRequiresRepair == 0 )
                        MoteMaker.ThrowSmoke( comp.parent.DrawPos, map, ( 1f - durability ) * 1 / 2f );

                    if ( durability < .25 && ( tick + comp.GetHashCode() ) % _moteIntervalRequiresCriticalRepair == 0 )
                        MoteMaker.ThrowMicroSparks( comp.parent.DrawPos, map );
                }

                // can't simply use !Spawned, since that would allow resetting durability by moving furniture.
                if ( comp?.parent?.DestroyedOrNull() ?? true )
                {
                    // mark for removal
                    orphaned.Add( comp );
                }
            }

            // remove
            foreach ( CompBreakdownable comp in orphaned )
                _durabilities.Remove( comp );
        }

#endregion Methods
    }
}
