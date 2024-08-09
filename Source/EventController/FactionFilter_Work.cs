using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;
using static System.Collections.Specialized.BitVector32;

namespace EventController_rQP
{
    internal class FactionFilter_Work
    {
        public static List<FactionDef> validFactions;

        public static void FactionFilter(ref Pawn pawn, ref FactionDef factionType)
        {
            var race = pawn.kindDef.race;
            var body = pawn.kindDef.RaceProps.body;
            validFactions = GetValidFactions(race, body);
            if (pawn.Faction != null)
            {
                if (!pawn.Faction.def.isPlayer && (pawn.Faction.def == factionType || factionType.modContentPack.IsOfficialMod))
                {
                    return;
                }
                if (pawn.Faction.def.isPlayer)
                {
                    FactionFilterInner(validFactions, ref factionType);
                    return;
                }
            }

            var factionRaces = EventController_Work.GetFactionPawnRaces();
            var races = factionRaces.TryGetValue(factionType);
            var factionBodies = EventController_Work.GetFactionPawnBodies();
            var bodies = factionBodies.TryGetValue(factionType);

            if (races != null && bodies != null)
            {
                if (!pawn.Faction.def.isPlayer && races.Contains(race) && bodies.Contains(body))
                {
                    return;
                }
                else
                {
                    FactionFilterInner(validFactions, ref factionType);
                }
            }
        }
        public static void FactionFilterInner(List<FactionDef> factions, ref FactionDef factionType)
        {
            if (factions.Any())
            {
                factionType = factions.RandomElement();
            }
            return;
        }

        public static List<FactionDef> GetValidFactions(ThingDef race, BodyDef body)
        {
            var factionKinds = EventController_Work.GetFactionPawnKinds();
            List<FactionDef> factions = new();
            foreach (var faction in factionKinds.Keys)
            {
                foreach (var kind in factionKinds[faction])
                {
                    if (kind.race == race && kind.RaceProps.body == body)
                    {
                        factions.Add(faction);
                        break;
                    }
                }
            }
            return factions;
        }
        public static void BackstoryFilter(ref Pawn pawn, ref List<BackstoryCategoryFilter> backstoryCategories, ref FactionDef factionType)
        {
            if (pawn.Faction != null)
            {
                if (!pawn.Faction.def.isPlayer && (pawn.Faction.def == factionType))
                {
                    return;
                }
            }
            var flag = !backstoryCategories.Where(f => f.categories == null).Any();
            if (flag)
            {
                return;
            }
            BackstoryFilterInner(out HashSet<string> categories);

            for (int i = 0; i < backstoryCategories.Count; i++)
            {
                var category = backstoryCategories[i];
                if (category.categories == null)
                {
                    backstoryCategories[i].categories = [];
                    backstoryCategories[i].categories.AddRange(categories);
                }
            }
        }
        public static void BackstoryFilterInner(out HashSet<string> categories)
        {
            var factionCategories = EventController_Work.GetFactionBackstoryCategories();
            categories = new();

            if (validFactions != null)
            {
                foreach (var faction in validFactions)
                {
                    categories.UnionWith(factionCategories[faction]);
                }
            }
        }
    }
}
