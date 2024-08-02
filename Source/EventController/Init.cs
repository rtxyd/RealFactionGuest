using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                MethodInfo MeInfo = typeof(PawnGroupKindWorker_Trader).GetMethod("GenerateTrader", Instance);
                return MeInfo;
            }))();

            var prefix_PawnGroupKindWorker_GeneratePawns = typeof(EventController_Work).GetMethod("Pre_PawnGroupKindWorker_GeneratePawns");
            var postfix_PawnGroupKindWorker_GeneratePawns = typeof(EventController_Work).GetMethod("Postfix_PawnGroupKindWorker_GeneratePawns");

            var prefix_PawnGroupKindWorker_GenerateTrader = typeof(EventController_Work).GetMethod("Pre_PawnGroupKindWorker_GenerateTrader");
            var postfix_PawnGroupKindWorker_GenerateTrader = typeof(EventController_Work).GetMethod("Postfix_PawnGroupKindWorker_GenerateTrader");

            var prefix_PawnGroupKindWorker_GenerateCarriers = typeof(EventController_Work).GetMethod("Pre_PawnGroupKindWorker_GenerateCarriers");
            var postfix_PawnGroupKindWorker_GenerateCarriers = typeof(EventController_Work).GetMethod("Postfix_PawnGroupKindWorker_GenerateCarriers");

            var prefix_PawnGroupKindWorker_GenerateGuards = typeof(EventController_Work).GetMethod("Pre_PawnGroupKindWorker_GenerateGuards");
            var postfix_PawnGroupKindWorker_GenerateGuards = typeof(EventController_Work).GetMethod("Postfix_PawnGroupKindWorker_GenerateGuards");

            harmony.Patch(generate, new HarmonyMethod(prefix_PawnGroupKindWorker_GeneratePawns), new HarmonyMethod(postfix_PawnGroupKindWorker_GeneratePawns));
            harmony.Patch(trader, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateTrader), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateTrader));
            harmony.Patch(carriers, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateCarriers), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateCarriers));
            harmony.Patch(guards, new HarmonyMethod(prefix_PawnGroupKindWorker_GenerateGuards), new HarmonyMethod(postfix_PawnGroupKindWorker_GenerateGuards));
            Log.Message("# Real Faction Guest Event Controller - Init Complete");
        }
    }
}
