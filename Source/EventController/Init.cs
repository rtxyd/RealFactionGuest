using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
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
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateTrader", Instance);
                return MeInfo;
            }))();

            var carriers = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateCarriers", Instance);
                return MeInfo;
            }))();

            var guards = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateGuards", Instance);
                return MeInfo;
            }))();

            var generateNewPawnInternal = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                MethodInfo MeInfo = typeof(PawnGenerator).GetMethod("GenerateNewPawnInternal", Instance);
                return MeInfo;
            }))();

            var giveShuffledBioTo = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                MethodInfo MeInfo = typeof(PawnBioAndNameGenerator).GetMethod("GiveShuffledBioTo", Instance);
                return MeInfo;
            }))();

            var getBackstoryCategoryFiltersFor = ((Func<MethodInfo>)(() =>
            {
                BindingFlags Instance = BindingFlags.NonPublic | BindingFlags.Static;
                MethodInfo MeInfo = typeof(PawnBioAndNameGenerator).GetMethod("GetBackstoryCategoryFiltersFor", Instance);
                return MeInfo;
            }))();

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

            var fillBackstorySlotShuffled = AccessTools.Method(typeof(PawnBioAndNameGenerator), nameof(PawnBioAndNameGenerator.FillBackstorySlotShuffled));
            var prefix_FillBackstorySlotShuffled = eventWorker.GetMethod("Prefix_FillBackstorySlotShuffled");
            var postfix_FillBackstorySlotShuffled = eventWorker.GetMethod("Postfix_FillBackstorySlotShuffled");
            harmony.Patch(fillBackstorySlotShuffled, new HarmonyMethod(prefix_FillBackstorySlotShuffled), new HarmonyMethod(postfix_FillBackstorySlotShuffled));

            var passToWorld = AccessTools.Method(typeof(WorldPawns), nameof(WorldPawns.PassToWorld));
            var prefix_PassToWorld = eventWorker.GetMethod("Prefix_PassToWorld");
            var postfix_PassToWorld = eventWorker.GetMethod("Postfix_PassToWorld");
            harmony.Patch(passToWorld, new HarmonyMethod(prefix_PassToWorld), new HarmonyMethod(postfix_PassToWorld));
            Log.Message("# Real Faction Guest Event Controller - Init Complete");
        }
    }
}
