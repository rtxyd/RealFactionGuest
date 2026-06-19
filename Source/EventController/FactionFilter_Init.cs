using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace EventController_rQP
{
    [StaticConstructorOnStartup]
    internal static class FactionFilter_Init
    {
        public static readonly float humanlikeModFactionNum;
        public static readonly Dictionary<FactionDef, HashSet<PawnKindDef>> factionPawnKinds = new();
        public static readonly Dictionary<FactionDef, HashSet<ThingDef>> factionPawnRaces = new();
        public static readonly Dictionary<ThingDef, HashSet<FactionDef>> racePawnFactions = new();
        public static readonly Dictionary<ThingDef, HashSet<FactionDef>> racePawnHiddenFactions = new();
        public static readonly Dictionary<FactionDef, HashSet<BodyDef>> factionPawnBodies = new();
        public static readonly HashSet<FactionDef> vanillaFactions = new();
        public static readonly HashSet<FactionDef> hiddenFactions = new();
        public static readonly HashSet<ThingDef> allRaces = new();
        public static readonly HashSet<FactionDef> vanillaHumanlikeFactions = new();
        public static readonly HashSet<FactionDef> moddedHumanlikeFactions = new();
        public static readonly HashSet<FactionDef> allHumanlikeFactions = new();
        public static readonly HashSet<FactionDef> validFactions_RPC = new();
        public static readonly HashSet<string> fallbackBackstory = new();
        static FactionFilter_Init()
        {
            //Log.Message("# Real Faction Guest - Faction Filter Init");
            humanlikeModFactionNum = 0;
            var factions =
                from faction in DefDatabase<FactionDef>.AllDefs
                where
                    faction.modContentPack is { PackageId: not null }
                    && !faction.modContentPack.PackageId.Contains("ogliss.alienvspredator")
                    && !faction.modContentPack.PackageId.Contains("Kompadt.Warhammer.Dryad")
                select faction;

            foreach (var f in factions)
                try
                {
                    var isHumanlike = false;
                    var flag = !f.hidden;
                    var flag2 = f.modContentPack.PackageId.Contains("ludeon");
                    if (f.pawnGroupMakers == null)
                    {
                        continue;
                    }
                    if (flag)
                    {
                        if (flag2)
                        {
                            vanillaFactions.Add(f);
                        }
                        validFactions_RPC.Add(f);
                    }
                    else
                    {
                        hiddenFactions.Add(f);
                    }
                    HashSet<PawnKindDef> pawnKindDefs = new();
                    HashSet<ThingDef> thingDefs = new();
                    HashSet<BodyDef> bodyDefs = new();
                    foreach (var pawnGroupMaker in f.pawnGroupMakers)
                    {
                        var options = pawnGroupMaker.options;
                        bool added = false;
                        foreach (var pawnGenOption in options)
                        {
                            if (flag)
                            {
                                var race = pawnGenOption.kind.race;
                                var kind = pawnGenOption.kind;
                                var body = pawnGenOption.kind.RaceProps.body;
                                pawnKindDefs.Add(kind);
                                thingDefs.Add(race);
                                allRaces.Add(race);
                                bodyDefs.Add(body);
                            }
                            else
                            {
                                var race = pawnGenOption.kind.race;
                                allRaces.Add(race);
                                if (!racePawnHiddenFactions.TryGetValue(race, out var set))
                                {
                                    racePawnHiddenFactions[race] = set;
                                }
                                set.Add(f);
                            }
                            if (pawnGenOption.kind.RaceProps == null
                                || pawnGenOption.kind.RaceProps.intelligence == Intelligence.Humanlike
                                || pawnGenOption.kind.RaceProps.Humanlike)
                            {
                                if (!flag2)
                                {
                                    isHumanlike = true;
                                    moddedHumanlikeFactions.Add(f);
                                }
                                else if (!added)
                                {
                                    added = true;
                                    vanillaHumanlikeFactions.Add(f);
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        //create faction -> pawn mapping
                        factionPawnKinds.Add(f, pawnKindDefs);
                        factionPawnRaces.Add(f, thingDefs);
                        factionPawnBodies.Add(f, bodyDefs);
                    }
                    if (!isHumanlike)
                    {
                        continue;
                    }
                    //Log.Message($"{f.defName} : {f.label}");
                    humanlikeModFactionNum++;
                }
                catch
                {
                    // ignored
                }
            try
            {
                foreach (var f2 in vanillaFactions)
                {
                    if (f2.backstoryFilters != null)
                    {
                        foreach (var item in f2.backstoryFilters)
                        {
                            if (item.categories != null)
                            {
                                fallbackBackstory.UnionWith(item.categories);
                            }
                        }
                    }
                }
                foreach (var r in allRaces)
                {
                    foreach (var entry in factionPawnRaces)
                    {
                        if (entry.Value.Contains(r))
                        {
                            if (!racePawnFactions.TryGetValue(r, out var factions1))
                            {
                                factions1 = new HashSet<FactionDef>();
                                racePawnFactions[r] = factions1;
                            }
                            factions1.Add(entry.Key);
                        }
                    }
                }
                allHumanlikeFactions.UnionWith(vanillaHumanlikeFactions);
                allHumanlikeFactions.UnionWith(moddedHumanlikeFactions);
            }
            catch
            {
                Log.Error("# Real Faction Guest - Finalization Failed.");
            }

            Log.Message("# Real Faction Guest - Faction Filter Init Complete");
        }
    }
}
