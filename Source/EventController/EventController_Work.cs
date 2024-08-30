using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace EventController_rQP
{

    public static class EventController_Work
    {
        public static OngoingEvent ongoingEvent = OngoingEvent.None;

        public static float GetHumanlikeModFactionNum()
        {
            return FactionFilter_Init.humanlikeModFactionNum;
        }
        public static HashSet<FactionDef> GetValidFactions_RPC()
        {
            return FactionFilter_Init.validFactions_RPC;
        }
        public static HashSet<string> GetFallbackBackstroy()
        {
            return FactionFilter_Init.fallbackBackstory;
        }
        public static HashSet<FactionDef> GetVanillaFactions()
        {
            return FactionFilter_Init.vanillaFactions;
        }
        public static Dictionary<FactionDef, HashSet<PawnKindDef>> GetFactionPawnKinds()
        {
            return FactionFilter_Init.factionPawnKinds;
        }
        public static Dictionary<FactionDef, HashSet<ThingDef>> GetFactionPawnRaces()
        {
            return FactionFilter_Init.factionPawnRaces;
        }
        public static Dictionary<FactionDef, HashSet<BodyDef>> GetFactionPawnBodies()
        {
            return FactionFilter_Init.factionPawnBodies;
        }
        public static string GetOngoingEvent()
        {
            switch (ongoingEvent)
            {
                case OngoingEvent.Trader: return "Trader";
                case OngoingEvent.Carrier: return "Carrier";
                case OngoingEvent.Guard: return "Guard";
                case OngoingEvent.RefugeePodCrash: return "RefugeePodCrash";
                case OngoingEvent.InternalGen: return "GenerateOrRedressPawnInternal";
                case OngoingEvent.GenerateNewPawnInternal: return "GenerateNewPawnInternal";
                case OngoingEvent.CreepJoiner: return "CreepJoiner";
                case OngoingEvent.FactionFix: return "FactionFix";
                case OngoingEvent.BackstoryFix: return "BackstoryFix";
                case OngoingEvent.QuestGetPawn: return "QuestGetPawn";
                case OngoingEvent.DamageUntilDowned: return "DamageUntilDowned";
                case OngoingEvent.FactionLeaderValidator: return "FactionLeaderValidator";
                case OngoingEvent.RequestValidator: return "RequestValidator";
                case OngoingEvent.AdjustXenotype: return "AdjustXenotype";
                case OngoingEvent.PreApplyDamage: return "PreApplyDamage";
                case OngoingEvent.PreApplyDamagePawn: return "PreApplyDamagePawn";
                case OngoingEvent.PreApplyDamageThing: return "PreApplyDamageThing";
                case OngoingEvent.CreepJoinerValidator: return "CreepJoinerValidator";
            }
            return "NULL";
        }
        public static void Prefix_PawnGroupKindWorker_GenerateTrader()
        {
            ongoingEvent = OngoingEvent.Trader;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateTrader()
        {
            ongoingEvent = OngoingEvent.None;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateCarriers()
        {
            ongoingEvent = OngoingEvent.Carrier;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateCarriers()
        {
            ongoingEvent = OngoingEvent.None;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateGuards()
        {
            ongoingEvent = OngoingEvent.Guard;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateGuards()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static void Prefix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            ongoingEvent = OngoingEvent.TraderGroup;
        }

        public static void Postfix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static void Prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            ongoingEvent = OngoingEvent.RefugeePodCrash;
        }

        public static void Postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static void Prefix_GiveAppropriateBioAndNameTo(ref Pawn pawn, ref FactionDef factionType)
        {
            try { ongoingEvent = OngoingEvent.FactionFix; FactionFilter_Work.FactionFilter(ref pawn, ref factionType); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; }
        }
        public static void Postfix_GiveAppropriateBioAndNameTo()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static void Prefix_GenerateSkills(ref Pawn pawn)
        {
            try { ongoingEvent = OngoingEvent.FactionLeaderValidator; if (RealFactionGuestSettings.factionLeaderValidator) PawnValidator_CrossWork.FactionLeaderValidator(ref pawn); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; }
        }
        public static void Prefix_FillBackstorySlotShuffled(ref Pawn pawn, ref BackstorySlot slot, ref List<BackstoryCategoryFilter> backstoryCategories, ref FactionDef factionType)
        {
            try { ongoingEvent = OngoingEvent.BackstoryFix; FactionFilter_Work.IncludeStoryCategories(pawn, slot, ref backstoryCategories); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; }
        }

        public static void Postfix_FillBackstorySlotShuffled()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static void Prefix_GenerateOrRedressPawnInternal(ref PawnGenerationRequest request)
        {
            try { ongoingEvent = OngoingEvent.InternalGen; PawnValidator_CrossWork.RequestValidator(ref request); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; }
        }
        public static void Prefix_DamageUntilDowned()
        {
            ongoingEvent = OngoingEvent.DamageUntilDowned;
        }
        public static void Postfix_DamageUntilDowned()
        {
            ongoingEvent = OngoingEvent.None;
        }
        public static bool Prefix_AdjustXenotypeForFactionlessPawn(ref Pawn pawn)
        {
            try { ongoingEvent = OngoingEvent.AdjustXenotype; return RealFactionGuestSettings.dontAdjustXenotypeForRabbie ? PawnValidator_CrossWork.IsAdjustXenotype(ref pawn) : true; }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; return true; }
        }
        public static bool Prefix_PreApplyDamage(ref bool absorbed)
        {
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static bool Prefix_PreApplyDamageThing(ref bool absorbed)
        {
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static bool Prefix_PreApplyDamagePawn(ref bool absorbed)
        {
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static void Postfix_PawnGenerator_GeneratePawn(ref Pawn __result)
        {
            try { ongoingEvent = OngoingEvent.CreepJoinerValidator; if (RealFactionGuestSettings.creepJoinerGenerateNoLimit && __result.kindDef is CreepJoinerFormKindDef) PawnValidator_CrossWork.CreepJoinerValidator(ref __result); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvent = OngoingEvent.None; }
            EventController_Reset();
        }
        public static IEnumerable<CodeInstruction> Transpiler_GetCreepjoinerSpecifics(ILGenerator iLGenerator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            var methodinfo = AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow), new System.Type[] { typeof(Map) });
            MethodReplaceHelper replaceHelper = new MethodReplaceHelper();
            Label labelTrue = iLGenerator.DefineLabel();
            var replacer = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldind_Ref),
                new CodeInstruction(OpCodes.Brtrue_S, labelTrue),
                new CodeInstruction(OpCodes.Ldc_R4, 35f),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Ldarg_0){ labels = new List<Label>(){ labelTrue } },
                new CodeInstruction(OpCodes.Call, methodinfo),
                new CodeInstruction(OpCodes.Stloc_0)
            };
            replaceHelper.SetAllNeededProperties(methodinfo, OpCodes.Ldarg_0, OpCodes.Stloc_0, codes, replacer, true);
            return replaceHelper.Run().AsEnumerable();
        }
        public static void EventController_Reset()
        {
            //Log.Message(GetOngoingEvent());
            ongoingEvent = OngoingEvent.None;
        }
    }
}
