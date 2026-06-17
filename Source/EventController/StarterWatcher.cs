using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Collections;
using Verse;

namespace EventController_rQP
{
    public class StarterWatcher : GameComponent
    {
        public class ThingFactionEntry
        {
            public ThingDef thing;
            public FactionDef faction;
        }
        private bool captured = false;
        private List<ThingDef> starterPawnRaces = new List<ThingDef>();
        private Dictionary<ThingDef, float> raceWeights = new Dictionary<ThingDef, float>();
        private List<ThingFactionEntry> mappingFactionsList = new List<ThingFactionEntry>();
        private Dictionary<ThingDef, HashSet<FactionDef>> mappingFactions = new Dictionary<ThingDef, HashSet<FactionDef>>();
        private HashSet<int> startingPawnIDs = new HashSet<int>();

        public StarterWatcher(Game game) { }

        public override void StartedNewGame()
        {
        }

        public override void LoadedGame()
        {
        }
        public override void GameComponentTick()
        {
            if (captured) return;
            if (Find.Maps.Count == 0) return;
            if (Find.TickManager.TicksGame < 10) return;

            CacheStartingPawns();
            ComputWeights();
            captured = true;
            if (starterPawnRaces.Count < 10)
            {
                Log.Message($"# Real Faction Guest - Preparing Player Faction Races Weights Capture");
                foreach (var item in raceWeights)
                {
                    HashSet<FactionDef> candidates = mappingFactions.TryGetValue(item.Key, []);
                    Log.Message($"# Real Faction Guest - Race { item.Key.defName } ; Weight { String.Format("{0:P}", item.Value) } ; Factions {String.Join(", ", candidates) }");
                }
            }
        }
        public ThingDef GetRandomRaceByWeight()
        {
            var result = raceWeights.RandomElementByWeight(kv =>
            {
                return kv.Value;
            });
            if (result.Key == null)
            {
                Log.Error("Failed getting valid pawn race for player.");
                return ThingDefOf.Human;
            } else
            {
                return result.Key;
            }
        }
        public FactionDef GetRandomFactionByRace(ThingDef race)
        {
            mappingFactions.TryGetValue(race, out var factions);
            if (factions == null) return Faction.OfPlayer.def;
            else
            {
                return factions.RandomElement();
            }
        }
        private void ComputWeights()
        {
            HashSet<ThingDef> counted = new HashSet<ThingDef>();
            List<ThingDef> copy = starterPawnRaces.ListFullCopy();
            foreach (var race in starterPawnRaces)
            {
                if (!raceWeights.ContainsKey(race))
                {
                    int count = 0;
                    foreach (var r in copy)
                    {
                        if (r == race)
                        {
                            count++;
                        }
                    }
                    float weight = count / copy.Count;
                    raceWeights.Add(race, weight);
                    var factions = from item in EventController_Work.GetFactionPawnRaces()
                                   where item.Value.Contains(race)
                                   select item.Key;
                    mappingFactions.Add(race, factions.ToHashSet());
                    foreach (var f in factions)
                    {
                        ThingFactionEntry entry = new ThingFactionEntry();
                        entry.thing = race;
                        entry.faction = f;
                        mappingFactionsList.Add(entry);
                    }
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref captured, "captured", false);
            Scribe_Collections.Look(ref starterPawnRaces, "startingPawnRaces", LookMode.Def);
            Scribe_Collections.Look(ref raceWeights, "raceWeights", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref mappingFactionsList, "mappingFactionsList", LookMode.Deep);
            Scribe_Collections.Look(ref startingPawnIDs, "startingPawnIDs", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                RebuildMap();
            }
        }

        private void RebuildMap()
        {
            foreach (var e in mappingFactionsList)
            {
                if (e.thing == null || e.faction == null) continue;

                if (!mappingFactions.TryGetValue(e.thing, out var set))
                {
                    set = new HashSet<FactionDef>();
                    mappingFactions[e.thing] = set;
                }

                set.Add(e.faction);
            }
        }

        private void CacheStartingPawns()
        {
            starterPawnRaces.Clear();
            var pawns = Find.Maps
                .SelectMany(m => m.mapPawns.AllPawnsSpawned)
                .Where(p => p.Faction is { IsPlayer: true })
                .ToList();
            foreach (var pawn in pawns)
            {
                starterPawnRaces.Add(pawn.kindDef.race);
                startingPawnIDs.Add(pawn.thingIDNumber);
            }
        }
        public bool IsCaptured() { return captured; }
        public bool IsStarterPawn(Pawn pawn)
        {
            return startingPawnIDs.Contains(pawn.thingIDNumber);
        }

    }
}
