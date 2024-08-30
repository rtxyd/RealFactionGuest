using EventController_rQP;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace raceQuestPawn;

[HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), typeof(PawnGenerationRequest))]
public class patch_PawnGenerator_GeneratePawn
{
    private static AccessTools.FieldRef<DrugPolicy, List<DrugPolicyEntry>> s_entriesInt =
        AccessTools.FieldRefAccess<DrugPolicy, List<DrugPolicyEntry>>("entriesInt");
    [HarmonyPriority(1000)]
    public static void Prefix(ref PawnGenerationRequest request)
    {
        try
        {
            if (Current.ProgramState != ProgramState.Playing)
            {
                return;
            }

            if (request.Faction == null)
            {
                return;
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

            if (request.KindDef == PawnKindDefOf.WildMan)
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

            if (PawnValidator_CrossWork.IsNotFromVanilla())
            {
                return;
            }

            //new TestTool().TestTool_ForceCreepJoiner(ref request);
            //Log.Message($"request : {(request.Faction != null ? request.Faction.def.defName : "none")}, {(request.KindDef != null ? request.KindDef.defName : "none")}");

            bool flag = true;
            bool chance = Rand.Chance(RealFactionGuestSettings.strictChance);
            var faction = request.Faction.def;
            var kinddef = request.KindDef;

            bool default_filter = faction.modContentPack.PackageId != kinddef.modContentPack.PackageId;
            if (RealFactionGuestSettings.alternativeFaction && default_filter)
            {
                var factionpawnraces = EventController_Work.GetFactionPawnRaces();

                if (factionpawnraces.ContainsKey(faction))
                {
                    default_filter = factionpawnraces[faction].Contains(kinddef.race);
                }
            }
            bool strict = chance && default_filter;
            if (strict
                && (request.Faction?.def.modContentPack != null
                && (!request.Faction.def.modContentPack.PackageId.Contains("ludeon")
                || request.Faction.def.modContentPack.PackageId.Contains("rimworld.biotech")))
               )
            {
                // 팩션이 있을때
                float combatPower = kinddef.combatPower;
                PawnKindDef p_make = null;

                if (faction.pawnGroupMakers != null)
                {
                    p_make = ChoosePawn.ChoosePawnKind(faction.pawnGroupMakers, combatPower, flag);
                }

                if (p_make != null)
                {
                    request.KindDef = p_make;
                }

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