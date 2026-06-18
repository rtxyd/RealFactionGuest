using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
        public class ThingFactionEntry : IExposable
        {
            public ThingDef thing;
            public FactionDef faction;

            public void ExposeData()
            {
                Scribe_Defs.Look(ref thing, "thing");
                Scribe_Defs.Look(ref faction, "faction");
            }
        }
        private bool captured = false;
        private bool noCandidateFactions = false;
        private List<ThingDef> starterPawnRaces = new List<ThingDef>();
        private Dictionary<ThingDef, float> raceWeights = new Dictionary<ThingDef, float>();
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
                    HashSet<FactionDef> candidates = EventController_Work.GetRacePawnFactions().TryGetValue(item.Key, []);
                    if (candidates.Count == 0)
                    {
                        EventController_Work.GetRacePawnHiddenFactions().TryGetValue(item.Key, out candidates);
                        if (candidates.Count == 0)
                        {
                            noCandidateFactions = true;
                            Log.Error("Trying to find pawn potential factions, but none.");
                        }
                    }
                    string factionmsg = candidates.Any() ? string.Join(", ", candidates.Select(x => x?.defName ?? "null")) : "None";
                    Log.Message($"# Real Faction Guest - Race [{ item.Key.defName }] ; Weight [{ String.Format("{0:P}", item.Value) }] ; Factions [{ factionmsg }]");
                }
            } else
            {
                Log.Message("Player faction has more than 10 races, skip printing details.");
            }
        }
        public ThingDef GetRandomRaceByWeight()
        {
            if (raceWeights.Count == 0) return FallbackRace();
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
        public ThingDef FallbackRace()
        {
            if (!EventController_Work.GetFactionPawnRaces().TryGetValue(Faction.OfPlayer.def, out var fallback))
            {
                return ThingDefOf.Human;
            }
            else if (fallback.Count == 0)
            {
                var resultset = EventController_Work.GetFactionPawnRaces().RandomElement().Value;
                if (resultset.Count != 0)
                {
                    return resultset.RandomElement();
                }
                else
                {
                    return ThingDefOf.Human;
                }
            }
            return fallback.RandomElement();
        }
        public FactionDef FallbackFaction()
        {
            return EventController_Work.GetAllHumanLikeFactions().RandomElement();
        }
        public FactionDef GetRandomFactionByRace(ThingDef race)
        {
            EventController_Work.GetRacePawnFactions().TryGetValue(race, out var factions);
            if (factions == null) return FallbackFaction();
            else
            {
                return factions.RandomElement();
            }
        }
        private void ComputWeights()
        {
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
                    float weight = (float)count / copy.Count;
                    raceWeights.Add(race, weight);
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref captured, "captured", false);
            Scribe_Collections.Look(ref starterPawnRaces, "startingPawnRaces", LookMode.Def);
            Scribe_Collections.Look(ref raceWeights, "raceWeights", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref startingPawnIDs, "startingPawnIDs", LookMode.Value);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (captured)
                {
                    if (noCandidateFactions || starterPawnRaces.Count == 0 || startingPawnIDs.Count == 0)
                    {
                        captured = false;
                    }
                }
            }
        }

        private void CacheStartingPawns()
        {
            starterPawnRaces.Clear();
            var pawns = Find.Maps
                .SelectMany(m => m.mapPawns.AllPawnsSpawned)
                .Where(p => p.Faction is { IsPlayer: true } && (p.RaceProps == null || p.RaceProps.intelligence == Intelligence.Humanlike || p.RaceProps.Humanlike))
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
