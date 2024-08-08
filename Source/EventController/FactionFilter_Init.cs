using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace EventController_rQP
{
    [StaticConstructorOnStartup]
    internal static class FactionFilter_Init
    {
        public static readonly float humanlikeModFactionNum;
        public static readonly Dictionary<FactionDef, HashSet<PawnKindDef>> factionPawnKinds = new();
        public static readonly Dictionary<FactionDef, HashSet<ThingDef>> factionPawnRaces = new();
        public static readonly Dictionary<FactionDef, HashSet<BodyDef>> factionPawnBodies = new();
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
            //AllFactions.Where((Faction x) => !x.IsPlayer && !x.Hidden && x.def.humanlikeFaction && (allowDefeated || !x.defeated) && (allowTemporary || !x.temporary) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel));
            foreach (var f in factions)
                try
                {
                    var isHumanlike = false;
                    if (f.pawnGroupMakers == null)
                    {
                        continue;
                    }
                    HashSet<PawnKindDef> pawnKindDefs = new ();
                    HashSet<ThingDef> thingDefs = new ();
                    HashSet<BodyDef> bodyDefs = new ();

                    foreach (var pawnGroupMaker in f.pawnGroupMakers)
                    {
                        var options = pawnGroupMaker.options;
                        foreach (var pawnGenOption in options)
                        {
                            var race = pawnGenOption.kind.race;
                            var kind = pawnGenOption.kind;
                            var body = pawnGenOption.kind.RaceProps.body;

                            pawnKindDefs.Add(kind);
                            thingDefs.Add(race);
                            bodyDefs.Add(body);

                            var backsotryFiltersOverride = pawnGenOption.kind.backstoryFiltersOverride;
                            if (!f.modContentPack.PackageId.Contains("ludeon")
                                || pawnGenOption.kind.RaceProps == null
                                || pawnGenOption.kind.RaceProps.intelligence == Intelligence.Humanlike
                                || pawnGenOption.kind.RaceProps.Humanlike)
                            {
                                isHumanlike = true;
                            }
                        }
                    }
                    //create faction -> pawn reflection
                    factionPawnKinds.Add(f, pawnKindDefs);
                    factionPawnRaces.Add(f, thingDefs);
                    factionPawnBodies.Add(f, bodyDefs);

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
            Log.Message("# Real Faction Guest - Faction Filter Init Complete");
        }
    }
}
