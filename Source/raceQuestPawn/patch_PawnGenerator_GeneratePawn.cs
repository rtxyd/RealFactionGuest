using EventController_rQP;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            //Log.Message($"request : {(request.Faction != null ? request.Faction.def.defName : "none")}, {(request.KindDef != null ? request.KindDef.defName : "none")}");

            float combatPower;
            FactionDef fd;
            PawnKindDef p_make;

            var stack = new StackTrace(0, true);
            StackFrame frame = stack.GetFrame(3);
            StackFrame frame2 = stack.GetFrame(5);
            Type declaringType = frame.GetMethod().DeclaringType;
            Type declaringType2 = frame2.GetMethod().DeclaringType;

            bool flag = true;
            bool chance = Rand.Chance(RealFactionGuestSettings.strictChance);
            bool strict = chance && request.Faction.def.modContentPack.PackageId != request.KindDef.modContentPack.PackageId;

            if (declaringType2 == typeof(QuestNode_Root_RefugeePodCrash))
            {
                request.AllowDowned = true;
                EventController_Work.isRefugeePodCrash = true;
                //flag = false;
            }

            if (request.Faction?.def.modContentPack != null
                && !request.Faction.def.modContentPack.PackageId.Contains("ludeon")
                && strict)
            {
                if (declaringType == typeof(RimWorld.PawnGroupKindWorker_Trader))
                {
                    return;
                }
                else
                {
                    var target = stack.GetFrames().Any(f => f.GetMethod().DeclaringType == typeof(RimWorld.PawnGroupKindWorker_Trader));
                    if (target)
                    {
                        return;
                    }
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