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
            var generate = AccessTools.Method(typeof(PawnGroupKindWorker), nameof(PawnGroupKindWorker.GeneratePawns), new Type[] { typeof(PawnGroupMakerParms), typeof(PawnGroupMaker), typeof(bool) });
            var trader = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateTrader", Instance);
                return MeInfo;
            }))();

            var carriers = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateCarriers", Instance);
                return MeInfo;
            }))();

            var guards = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateGuards", Instance);
                return MeInfo;
            }))();

            var generateNewPawnInternal = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnGenerator).GetMethod("GenerateNewPawnInternal", Instance);
                return MeInfo;
            }))();

            var giveShuffledBioTo = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnBioAndNameGenerator).GetMethod("GiveShuffledBioTo", Instance);
                return MeInfo;
            }))();

            var getBackstoryCategoryFiltersFor = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                //FieldInfo Info = ;
                MethodInfo MeInfo = typeof(PawnBioAndNameGenerator).GetMethod("GetBackstoryCategoryFiltersFor", Instance);
                return MeInfo;
            }))();


            //var questNode_Root_RefugeePodCrash_GeneratePawn = AccessTools.Method(typeof(QuestNode_Root_RefugeePodCrash), nameof(QuestNode_Root_RefugeePodCrash.GeneratePawn));

            Type eventWorker = typeof(EventController_Work);
            var prefix_PawnGroupKindWorker_GeneratePawns = eventWorker.GetMethod("Prefix_PawnGroupKindWorker_GeneratePawns");
            var postfix_PawnGroupKindWorker_GeneratePawns = eventWorker.GetMethod("Postfix_PawnGroupKindWorker_GeneratePawns");

            var prefix_PawnGroupKindWorker_GenerateTrader = eventWorker.GetMethod("Prefix_PawnGroupKindWorker_GenerateTrader");
            var postfix_PawnGroupKindWorker_GenerateTrader = eventWorker.GetMethod("Postfix_PawnGroupKindWorker_GenerateTrader");

            var prefix_PawnGroupKindWorker_GenerateCarriers = eventWorker.GetMethod("Prefix_PawnGroupKindWorker_GenerateCarriers");
            var postfix_PawnGroupKindWorker_GenerateCarriers = eventWorker.GetMethod("Postfix_PawnGroupKindWorker_GenerateCarriers");

            var prefix_PawnGroupKindWorker_GenerateGuards = eventWorker.GetMethod("Prefix_PawnGroupKindWorker_GenerateGuards");
            var postfix_PawnGroupKindWorker_GenerateGuards = eventWorker.GetMethod("Postfix_PawnGroupKindWorker_GenerateGuards");

            var prefix_GenerateNewPawnInternal = eventWorker.GetMethod("Prefix_GenerateNewPawnInternal");
            var postfix_GenerateNewPawnInternal = eventWorker.GetMethod("Postfix_GenerateNewPawnInternal");

            //var prefix_GiveShuffledBioTo = eventWorker.GetMethod("Prefix_GiveShuffledBioTo");
            //var postfix_GiveShuffledBioTo = eventWorker.GetMethod("Postfix_GiveShuffledBioTo");

            //var prefix_GetBackstoryCategoryFiltersFor = eventWorker.GetMethod("Prefix_GetBackstoryCategoryFiltersFor");
            //var postfix_GetBackstoryCategoryFiltersFor = eventWorker.GetMethod("Postfix_GetBackstoryCategoryFiltersFor");

            //var prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn = eventWorker.GetMethod("Prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn");
            //var postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn = eventWorker.GetMethod("Postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn");

            harmony.Patch(generate, new HarmonyMethod(prefix_PawnGroupKindWorker_GeneratePawns), new HarmonyMethod(postfix_PawnGroupKindWorker_GeneratePawns));
            harmony.Patch(trader, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateTrader), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateTrader));
            harmony.Patch(carriers, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateCarriers), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateCarriers));
            harmony.Patch(guards, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateGuards), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateGuards));
            harmony.Patch(generateNewPawnInternal, new HarmonyMethod(prefix_GenerateNewPawnInternal), new HarmonyMethod(postfix_GenerateNewPawnInternal));

            //backstoryfixer
            var giveAppropriateBioAndNameTo = AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.GiveAppropriateBioAndNameTo));
            var prefix_GiveAppropriateBioAndNameTo = eventWorker.GetMethod("Prefix_GiveAppropriateBioAndNameTo");
            var postfix_GiveAppropriateBioAndNameTo = eventWorker.GetMethod("Postfix_GiveAppropriateBioAndNameTo");
            harmony.Patch(giveAppropriateBioAndNameTo, new HarmonyMethod(prefix_GiveAppropriateBioAndNameTo), new HarmonyMethod(postfix_GiveAppropriateBioAndNameTo));

            //harmony.Patch(giveShuffledBioTo, new HarmonyMethod(prefix_GiveShuffledBioTo), new HarmonyMethod(postfix_GiveShuffledBioTo));
            //harmony.Patch(getBackstoryCategoryFiltersFor, new HarmonyMethod(prefix_GetBackstoryCategoryFiltersFor), new HarmonyMethod(postfix_GetBackstoryCategoryFiltersFor));
            //harmony.Patch(questNode_Root_RefugeePodCrash_GeneratePawn, new HarmonyMethod(prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn), new HarmonyMethod(postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn));
            Log.Message("# Real Faction Guest Event Controller - Init Complete");
        }
    }
}
