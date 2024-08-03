using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

            if (request.Faction?.def.modContentPack != null &&
                !request.Faction.def.modContentPack.PackageId.Contains("ludeon") &&
                request.KindDef.modContentPack.PackageId.Contains("ludeon"))
            {
                var stack = new StackTrace(0, true);
                StackFrame frame = stack.GetFrame(3);
                MethodBase method = frame.GetMethod();
                Type declaringType = method.DeclaringType;
                //check stack if it's vanilla caravan trader generation request and skip it to vanilla generation.
                //It should be safe to skip that, because it's using vanilla method to generate vanilla pawnkind.
                //in 1.5, frame 1 is this method, 2 is harmony patched generatePawn, and 3 is generate traders/carriers/guards.
                //the method only executes in particular situations, such as remove the "if" right below.
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
                    ChoosePawn.ChoosePawnKind(fd.pawnGroupMakers, combatPower);
                }
                if (p_make != null)
                {
                    request.KindDef = p_make;
                }
                //Log.Message($"A : {p_make.defName} : {p_make.combatPower}");
                return;
            }


            if (request.Faction != null && !request.KindDef.defName.ToLower().Contains("refugee"))
            {
                return;
            }

            // 팩션이 없거나 조난자 일때
            if (Rand.Value <= Core.vanillaRatio)
            {
                return;
            }


            combatPower = request.KindDef.combatPower;

            p_make = null;
            var tryCount = 0;

            IEnumerable<List<PawnGenOption>> options = [];
            while (!options.Any() && tryCount <= 11)
            {
                tryCount++;
                fd = DefDatabase<FactionDef>.AllDefs.RandomElement();

                if (fd is not { pawnGroupMakers: not null, modContentPack: not null })
                {
                    continue;
                }

                if (fd.modContentPack.PackageId.Contains("ludeon"))
                {
                    continue;
                }

                if (fd.modContentPack.PackageId.Contains("ogliss.alienvspredator"))
                {
                    continue;
                }

                if (fd.modContentPack.PackageId.Contains("Kompadt.Warhammer.Dryad"))
                {
                    continue;
                }
                //Log.Message($"B0 : {fd.modContentPack.PackageId}");
                //there may need a filter of animal pawns and wildman.
                options = fd.pawnGroupMakers.Where(t => t.options != null).Select(t => t.options);
            }
            if (options.Any())
            {
                p_make = ChoosePawn.ChoosePawnKindInner(options, combatPower);
            }
            if (p_make != null)
            {
                request.KindDef = p_make;
                RefugeePotCrash_PawnValidator.PostProcessRequest(request);
                //Log.Message($"B : {p_make.defName} : {p_make.combatPower}");
                return;
            }
            else
            {
                return;
            }
        }
        catch
        {
            // ignored
        }
    }
}