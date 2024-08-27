using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EventController_rQP
{

    public static class EventController_Work
    {
        public static bool isTrader = false;
        public static bool isCarrier = false;
        public static bool isGuard = false;
        public static bool isEnd = false;
        public static bool isTraderGroup = false;
        public static bool isRefugeePodCrash = false;
        public static bool isInternalGen = false;
        public static bool isCreepJoiner = false;
        public static bool isFactionFix = false;
        public static bool isBackstoryFix = false;
        public static bool isQuestGetPawn = false;
        public static bool isDamageUntilDowned = false;
        public static bool isCreepJoinerValidatorOn = false;

        public static string ongoingEvent = null;

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

        public static bool IsCreepJoiner(ref PawnGenerationRequest request)
        {
            if (request.KindDef is CreepJoinerFormKindDef)
            {
                isCreepJoiner = true;
            }
            else
            {
                isCreepJoiner = false;
            }
            return isCreepJoiner;
        }

        public static bool IsRefugeePodCrash(int f = 8)
        {
            var stack = new StackTrace(0, true);
            if (stack.FrameCount < 18)
            {
                return isRefugeePodCrash = false;
            }
            var frame = stack.GetFrame(f);
            if (frame.GetMethod().DeclaringType == typeof(RimWorld.QuestGen.QuestNode_Root_RefugeePodCrash))
            {
                isRefugeePodCrash = true;
            }
            else
            {
                isRefugeePodCrash = false;
            }
            return isRefugeePodCrash;
        }
        public static string GetOngoingEvent()
        {
            if (isTrader)
            {
                ongoingEvent = "trader";
            }
            else if (isCarrier)
            {
                ongoingEvent = "carriers";
            }
            else if (isGuard)
            {
                ongoingEvent = "guards";
            }
            else if (isRefugeePodCrash)
            {
                ongoingEvent = "RefugeePodCrash";
            }
            else if (isInternalGen)
            {
                ongoingEvent = "GenerateNewPawnInternal";
            }
            else if (isCreepJoiner)
            {
                ongoingEvent = "CreepJoiner";
            }
            else if (isFactionFix)
            {
                ongoingEvent = "isFactionFix";
            }
            else if (isBackstoryFix)
            {
                ongoingEvent = "isBackstoryFix";
            }
            else
            {
                ongoingEvent = "options or unknown";
            }
            return ongoingEvent;
        }
        public static void Prefix_PawnGroupKindWorker_GenerateTrader()
        {
            isTrader = true;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateTrader()
        {
            isTrader = false;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateCarriers()
        {
            isCarrier = true;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateCarriers()
        {
            isCarrier = false;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateGuards()
        {
            isGuard = true;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateGuards()
        {
            isGuard = false;
        }

        public static void Prefix_PawnGroupKindWorker_GeneratePawns()
        {
            isEnd = true;
        }

        public static void Postfix_PawnGroupKindWorker_GeneratePawns()
        {
            isEnd = true;
        }
        public static void Prefix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            isTraderGroup = true;
        }

        public static void Postfix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            isTraderGroup = false;
        }
        public static void Prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            isRefugeePodCrash = true;
        }

        public static void Postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            isRefugeePodCrash = false;
        }
        public static void Prefix_GiveAppropriateBioAndNameTo(ref Pawn pawn, ref FactionDef factionType)
        {
            FactionFilter_Work.FactionFilter(ref pawn, ref factionType);
            isFactionFix = true;
        }
        public static void Postfix_GiveAppropriateBioAndNameTo()
        {
            isFactionFix = false;
        }
        public static void Prefix_GenerateSkills(ref Pawn pawn)
        {
            if (RealFactionGuestSettings.factionLeaderValidator)
            {
                PawnValidator_CrossWork.FactionLeaderValidator(ref pawn);
            }
        }
        public static void Prefix_FillBackstorySlotShuffled(ref Pawn pawn, ref BackstorySlot slot, ref List<BackstoryCategoryFilter> backstoryCategories, ref FactionDef factionType)
        {
            FactionFilter_Work.IncludeStoryCategories(pawn, slot, ref backstoryCategories);
            isBackstoryFix = true;
        }

        public static void Postfix_FillBackstorySlotShuffled()
        {
            isBackstoryFix = false;
        }
        public static void Prefix_GenerateOrRedressPawnInternal(ref PawnGenerationRequest request)
        {
            PawnValidator_CrossWork.RequestValidator(ref request);
        }
        public static void Prefix_DamageUntilDowned()
        {
            isDamageUntilDowned = true;
        }
        public static void Postfix_DamageUntilDowned()
        {
            isDamageUntilDowned = false;
        }
        public static bool Prefix_AdjustXenotypeForFactionlessPawn(ref Pawn pawn)
        {
            return RealFactionGuestSettings.dontAdjustXenotypeForRabbie ? PawnValidator_CrossWork.IsAdjustXenotype(ref pawn) : true;
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
            if (__result.kindDef is CreepJoinerFormKindDef)
            {
                PawnValidator_CrossWork.CreepJoinerValidator(ref __result);
            }
            EventController_Reset();
        }
        public static IEnumerable<CodeInstruction> Transpiler_GetCreepjoinerSpecifics(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            List<CodeInstruction> codes1 = new List<CodeInstruction>();
            var methodinfo = AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow), new System.Type[] { typeof(Map) });
            //var dm = new DynamicMethod("Replacer_1", typeof(void), new Type[] { typeof(Map) });
            TmpLabel labelFalse = new TmpLabel();
            TmpLabel labelTrue = new TmpLabel();
            var replacer = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Brfalse_S, labelFalse),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, methodinfo),
                //new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Br_S, labelTrue),
                new CodeInstruction(OpCodes.Nop, labelFalse),
                new CodeInstruction(OpCodes.Ldc_R4, 100f),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Nop, labelTrue),
            };
            //var replacer2 = new OpCode, object>
            //{
            //    (OpCodes.Ldarg_0, ),
            //    (OpCodes.Brfalse_S, labelFalse),
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Call, methodinfo),
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Br_S,labelTrue),
            //    new CodeInstruction(OpCodes.Ldc_R4, 100f){ labels = new List<Label> { labelFalse } },
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Nop){ labels = new List<Label> { labelTrue } },
            //};

            //var replacer = new List<CodeInstruction>()
            //{
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Ldnull),
            //    new CodeInstruction(OpCodes.Ceq),
            //    new CodeInstruction(OpCodes.Brfalse_S, labelFalse),
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Call, methodinfo),
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Br_S,labelTrue),
            //    new CodeInstruction(OpCodes.Ldc_R4, 100f){ labels = new List<Label> { labelFalse } },
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Nop){ labels = new List<Label> { labelTrue } },
            //};
            return Tools.MethodReplacer(codes, methodinfo, OpCodes.Ldarg_0, OpCodes.Stloc_0, replacer);

        }

        public static Label Generator(List<CodeInstruction> replacer)
        {
            throw new NotImplementedException();
        }

        public static void EventController_Reset()
        {
            //Log.Message(GetOngoingEvent());
            isTrader = false;
            isCarrier = false;
            isGuard = false;
            isEnd = false;
            isTraderGroup = false;
            isRefugeePodCrash = false;
            isInternalGen = false;
            isCreepJoiner = false;
            isFactionFix = false;
            isBackstoryFix = false;
            isQuestGetPawn = false;
            isDamageUntilDowned = false;
            isCreepJoinerValidatorOn = false;
        }
    }
}
