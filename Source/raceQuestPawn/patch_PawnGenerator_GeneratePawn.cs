using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
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
            HashSet<PawnKindDef> allPawnKinds;

            var stack = new StackTrace(0, true);
            StackFrame frame = stack.GetFrame(3);
            MethodBase method = frame.GetMethod();
            Type declaringType = method.DeclaringType;

            if (request.Faction?.def.modContentPack != null &&
                !request.Faction.def.modContentPack.PackageId.Contains("ludeon") &&
                request.KindDef.modContentPack.PackageId.Contains("ludeon"))
            {
                //check stack if it's vanilla caravan trader generation request and skip it to vanilla generation.
                //in 1.5 frame 1 is this method, 2 is harmony patched generatePawn, and 3 is generate traders/carriers/guards
                if (declaringType == typeof(RimWorld.PawnGroupKindWorker_Trader))
                {
                    return;
                }
                else
                {
                    var target = stack.GetFrames().Where(f => f.GetMethod().DeclaringType == typeof(RimWorld.PawnGroupKindWorker_Trader)).Any();
                    if (target)
                    {
                        return;
                    }
                }
                // 팩션이 있을때
                fd = request.Faction.def;
                combatPower = request.KindDef.combatPower;
                p_make = null;
                allPawnKinds = [];
                if (fd.pawnGroupMakers != null)
                {
                    foreach (var pawnGroupMaker in fd.pawnGroupMakers)
                    {
                        var optionsplus = pawnGroupMaker.options;
                        foreach (var pawnGenOption in optionsplus)
                        {
                                allPawnKinds.Add(pawnGenOption.kind);
                        }
                    }
                }
                //find all pawnkinds match the condition combatPower +- 30,or combatPower = request.combatPower.
                var query1 = allPawnKinds.Where(p => Mathf.Abs(p.combatPower - combatPower) < 30f);
                var query1e = query1.Where(p => p.combatPower == combatPower);
                if (query1.Any())
                {
                    // only one = request combatPower
                    if (query1e.Count() == 1)
                    {
                        p_make = query1e.ToHashSet().First();
                    }
                    // other situations, get random one
                    else
                    {
                        var ipawns1 = query1.ToHashSet();
                        p_make = ipawns1.RandomElement();
                    }
                    request.KindDef = p_make;
                    //Log.Message($"A : {p_make.defName} : {p_make.combatPower}");
                    return;
                }
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
            allPawnKinds = [];
            var tryCount = 0;

            while (allPawnKinds.Count <= 0 && tryCount <= 11)
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
                foreach (var pawnGroupMaker in fd.pawnGroupMakers)
                {
                    var optionsplus = pawnGroupMaker.options;
                    foreach (var pawnGenOption in optionsplus)
                    {
                        if (pawnGenOption.kind.RaceProps != null &&
                            pawnGenOption.kind.RaceProps.intelligence != Intelligence.Humanlike &&
                            !pawnGenOption.kind.RaceProps.Humanlike)
                        {
                            continue;
                        }

                        //Log.Message($"B0 : {fd.modContentPack.PackageId}");
                        allPawnKinds.Add(pawnGenOption.kind);
                    }
                }
            }
            // the same with above
            var query2 = allPawnKinds.Where(p => p.combatPower <= combatPower && Mathf.Abs(p.combatPower - combatPower) < 30f);
            var query2e = query2.Where(p => p.combatPower == combatPower);
            if (query2.Any())
            {
                if (query2e.Count() == 1)
                {
                    p_make = query2e.ToHashSet().First();
                }
                else
                {
                    var ipawns2 = query2.ToHashSet();
                    p_make = ipawns2.RandomElement();
                }

                request.KindDef = p_make;
                return;
            }
            //Log.Message($"B : {p_make.defName} : {p_make.combatPower}");
        }
        catch
        {
            // ignored
        }
    }
}