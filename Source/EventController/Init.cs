using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace EventController_rQP
{
    [StaticConstructorOnStartup]
    public static class Init
    {
        static Init()
        {
            var harmony = new Harmony("EventController_rQP");
            Type eventWorker = typeof(EventController_Work);
            Dictionary<Tuple<string, Tools.Patches>, MethodInfo> pairs = new Dictionary<Tuple<string, Tools.Patches>, MethodInfo>()
            {
                { new Tuple<string, Tools.Patches>("PawnGroupKindWorker_GeneratePawns",             Tools.Patches.Both),       AccessTools.Method(typeof(PawnGroupKindWorker), nameof(PawnGroupKindWorker.GeneratePawns), new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(bool) }) },
                { new Tuple<string, Tools.Patches>("PawnGroupKindWorker_Trader_GeneratePawns",      Tools.Patches.Both),       Tools.MethodTool(Tools.ParamValue.a, typeof(PawnGroupKindWorker_Trader), "GeneratePawns") },
                { new Tuple<string, Tools.Patches>("PawnGroupKindWorker_GenerateTrader",            Tools.Patches.Both),       Tools.MethodTool(Tools.ParamValue.a, typeof(PawnGroupKindWorker_Trader), "GenerateTrader")},
                { new Tuple<string, Tools.Patches>("PawnGroupKindWorker_GenerateCarriers",          Tools.Patches.Both),       Tools.MethodTool(Tools.ParamValue.a, typeof(PawnGroupKindWorker_Trader), "GenerateCarriers")},
                { new Tuple<string, Tools.Patches>("PawnGroupKindWorker_GenerateGuards",            Tools.Patches.Both),       Tools.MethodTool(Tools.ParamValue.a, typeof(PawnGroupKindWorker_Trader), "GenerateGuards")},
                { new Tuple<string, Tools.Patches>("GenerateSkills",                                Tools.Patches.Prefix),     Tools.MethodTool(Tools.ParamValue.b, typeof(PawnGenerator), "GenerateSkills")},
                { new Tuple<string, Tools.Patches>("GenerateOrRedressPawnInternal",                 Tools.Patches.Both),       Tools.MethodTool(Tools.ParamValue.b, typeof(PawnGenerator), "GenerateOrRedressPawnInternal")},
                { new Tuple<string, Tools.Patches>("GiveAppropriateBioAndNameTo",                   Tools.Patches.Both),       AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo))},
                { new Tuple<string, Tools.Patches>("FillBackstorySlotShuffled",                     Tools.Patches.Both),       AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.FillBackstorySlotShuffled))},
                { new Tuple<string, Tools.Patches>("QuestNode_Root_RefugeePodCrash_GeneratePawn",   Tools.Patches.Both),       AccessTools.Method(typeof(QuestNode_Root_RefugeePodCrash), nameof(QuestNode_Root_RefugeePodCrash.GeneratePawn))},
                { new Tuple<string, Tools.Patches>("DamageUntilDowned",                             Tools.Patches.Both),       AccessTools.Method(typeof(HealthUtility), nameof(HealthUtility.DamageUntilDowned))},
                { new Tuple<string, Tools.Patches>("PreApplyDamage",                                Tools.Patches.Both),       AccessTools.Method(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PreApplyDamage))},
                { new Tuple<string, Tools.Patches>("PreApplyDamageThing",                           Tools.Patches.Both),       AccessTools.Method(typeof(Thing), nameof(Thing.PreApplyDamage))},
                { new Tuple<string, Tools.Patches>("PreApplyDamagePawn",                            Tools.Patches.Both),       AccessTools.Method(typeof(Pawn), nameof(Pawn.PreApplyDamage))},
                { new Tuple<string, Tools.Patches>("AdjustXenotypeForFactionlessPawn",              Tools.Patches.Prefix),     AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.AdjustXenotypeForFactionlessPawn))},
                { new Tuple<string, Tools.Patches>("PawnGenerator_GeneratePawn",                    Tools.Patches.Postfix),    AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] {typeof(PawnGenerationRequest)})}
            };
            foreach (var item in pairs)
            {
                var variableName = item.Key.Item1;
                var patches = item.Key.Item2;
                item.Value.PatchTool(eventWorker, ref harmony, variableName, patches);
            }
            Log.Message("# Real Faction Guest Event Controller - Init Complete");
        }
    }
}
