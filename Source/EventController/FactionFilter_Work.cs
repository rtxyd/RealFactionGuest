using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Verse.MathEvaluatorCustomFunctions;
using Verse;
using RimWorld;

namespace EventController_rQP
{
    internal class FactionFilter_Work
    {
        public static void FactionFilter(ref Pawn pawn,ref FactionDef factionType)
        {
            if (pawn.Faction != null)
            {
                if (pawn.Faction.def == factionType || factionType.modContentPack.IsOfficialMod)
                {
                    return;
                }
            }
            var factionRaces = EventController_Work.GetFactionPawnRaces();
            var races = factionRaces.TryGetValue(factionType);
            var factionBodies = EventController_Work.GetFactionPawnBodies();
            var bodies = factionBodies.TryGetValue(factionType);

            var race = pawn.kindDef.race;
            var body = pawn.kindDef.RaceProps.body;
            if (races != null && bodies != null)
            {
                if (races.Contains(race) && bodies.Contains(body))
                {
                    return;
                }
                else
                {
                    var factionKinds = EventController_Work.GetFactionPawnKinds();
                    var kinds = factionKinds.TryGetValue(factionType);
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
                    factionType = factions.RandomElement();
                }
            }
        }
    }
}
