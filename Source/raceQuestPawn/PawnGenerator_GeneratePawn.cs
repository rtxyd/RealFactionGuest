using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace raceQuestPawn;

[HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), typeof(PawnGenerationRequest))]
public class PawnGenerator_GeneratePawn
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

            if (request.KindDef.trader)
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

            float combatPower;
            FactionDef fd;
            PawnKindDef p_make;
            List<PawnKindDef> allPawnKinds;
            if (request.Faction?.def.modContentPack != null &&
                !request.Faction.def.modContentPack.PackageId.Contains("ludeon") &&
                request.KindDef.modContentPack.PackageId.Contains("ludeon"))
            {
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
                        //miss traders.
                        optionsplus.AddRange(pawnGroupMaker.traders);
                        //optionsplus.AddRange(pawnGroupMaker.carriers);
                        optionsplus.AddRange(pawnGroupMaker.guards);
                        foreach (var pawnGenOption in optionsplus)
                        {
                            if (!allPawnKinds.Contains(pawnGenOption.kind))
                            {
                                allPawnKinds.Add(pawnGenOption.kind);
                            }
                        }
                    }
                }

                foreach (var p in allPawnKinds)
                {
                    if (p_make == null || Mathf.Abs(p_make.combatPower - combatPower) >
                        Mathf.Abs(p.combatPower - combatPower))
                    {
                        p_make = p;
                    }
                }

                if (p_make != null)
                {
                    request.KindDef = p_make;
                    return;
                }
            }


            if (request.Faction != null && !request.KindDef.defName.ToLower().Contains("refugee"))
            {
                return;
            }

            // 팩션이 없거나 조난자 일때
            if (Rand.Value <= Core.VanillaRatio)
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
                    //miss traders.
                    optionsplus.AddRange(pawnGroupMaker.traders);
                    //optionsplus.AddRange(pawnGroupMaker.carriers);
                    optionsplus.AddRange(pawnGroupMaker.guards);
                    foreach (var pawnGenOption in optionsplus)
                    {
                        if (allPawnKinds.Contains(pawnGenOption.kind) ||
                            pawnGenOption.kind.RaceProps != null &&
                            pawnGenOption.kind.RaceProps.intelligence != Intelligence.Humanlike &&
                            !pawnGenOption.kind.RaceProps.Humanlike)
                        {
                            continue;
                        }

                        allPawnKinds.Add(pawnGenOption.kind);
                    }
                }
            }

            foreach (var p in allPawnKinds)
            {
                if (p_make == null || Mathf.Abs(p_make.combatPower - combatPower) >
                    Mathf.Abs(p.combatPower - combatPower))
                {
                    p_make = p;
                }
            }

            if (p_make == null || !(Mathf.Abs(request.KindDef.combatPower - p_make.combatPower) <= 30f))
            {
                return;
            }

            request.KindDef = p_make;
        }
        catch
        {
            // ignored
        }
    }
}