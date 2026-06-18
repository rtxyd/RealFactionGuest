using EventController_rQP;
using HarmonyLib;
using RimWorld;
using Verse;

namespace raceQuestPawn;

[HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), typeof(PawnGenerationRequest))]
public class patch_PawnGenerator_GeneratePawn
{
    [HarmonyPriority(1000)]
    public static void Prefix(ref PawnGenerationRequest request)
    {
        try
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            FactionDef assumed_faction = null;
            bool isNotWandererJoin = true;
            if (request.Faction == null)
            {
                if (PawnValidator_CrossWork.IsNotWandererJoin()) return;
                isNotWandererJoin = false;
                assumed_faction = PawnValidator_CrossWork.OutputAssumedFaction(ref request);
            }

            if (request.KindDef.RaceProps != null && (
                    request.KindDef.RaceProps.Animal
                    || request.KindDef.RaceProps.intelligence <= Intelligence.ToolUser
                ))
            {
                return;
            }

            if (request.Faction is { IsPlayer: true })
            {
                return;
            }

            if (request.KindDef == PawnKindDefOf.WildMan
                || request.KindDef.trader)
            {
                return;
            }

            if (request.IsCreepJoiner)
            {
                return;
            }

            if ((EventController_Work.ongoingEvents & OngoingEvent.TraderGroup) != 0)
            {
                return;
            }

            if (isNotWandererJoin && PawnValidator_CrossWork.IsNotFromVanilla())
            {
                return;
            }

            //new TestTool().TestTool_ForceRabbie(ref request);
            //Log.Message($"request : {(request.Faction != null ? request.Faction.def.defName : "none")}, {(request.KindDef != null ? request.KindDef.defName : "none")}");
            if (assumed_faction == null && request.Faction != null) {
                assumed_faction = request.Faction.def;
            }
            var kinddef = request.KindDef;

            bool default_filter = assumed_faction.modContentPack.PackageId != kinddef.modContentPack.PackageId;
            if (RealFactionGuestSettings.alternativeFaction && default_filter)
            {
                var factionpawnraces = EventController_Work.GetFactionPawnRaces();

                if (factionpawnraces.ContainsKey(assumed_faction))
                {
                    default_filter = factionpawnraces[assumed_faction].Contains(kinddef.race);
                }
            }
            bool chance = Rand.Chance(RealFactionGuestSettings.strictChance);
            bool strict = chance && default_filter;
            if (strict
                && (assumed_faction.modContentPack != null
                && (!assumed_faction.modContentPack.PackageId.StartsWith("ludeon")
                || assumed_faction.modContentPack.PackageId.EndsWith("rimworld.biotech")))
               )
            {
                // 팩션이 있을때
                float combatPower = kinddef.combatPower;
                PawnKindDef p_make = null;

                if (assumed_faction.pawnGroupMakers != null)
                {
                    p_make = ChoosePawn.ChoosePawnKind(assumed_faction.pawnGroupMakers, combatPower, true);
                }

                if (p_make != null)
                {
                    request.KindDef = p_make;
                }
                //Log.Message($"A : {request.KindDef}");
                //Log.Message($"A : {p_make.defName} : {p_make.combatPower}");

                return;
            }
        }
        catch
        {
            // ignored
        }
        return;
    }
}