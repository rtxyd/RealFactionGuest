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
        PawnValidator_CrossWork.RequestValidator(ref request);
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

            //Log.Message($"request : {(request.Faction != null ? request.Faction.def.defName : "none")}, {(request.KindDef != null ? request.KindDef.defName : "none")}");

            float combatPower;
            FactionDef fd;
            PawnKindDef p_make;

            bool flag = true;
            bool chance = Rand.Chance(RealFactionGuestSettings.strictChance);
            bool strict = chance && request.Faction.def.modContentPack.PackageId != request.KindDef.modContentPack.PackageId;

            if (request.Faction?.def.modContentPack != null
                && !request.Faction.def.modContentPack.PackageId.Contains("ludeon")
                && strict)
            {
                if (EventController_Work.isTraderGroup)
                {
                    return;
                }

                // 팩션이 있을때
                fd = request.Faction.def;
                combatPower = request.KindDef.combatPower;
                p_make = null;

                if (fd.pawnGroupMakers != null)
                {
                    p_make = ChoosePawn.ChoosePawnKind(fd.pawnGroupMakers, combatPower, flag);
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
    }
}