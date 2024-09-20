using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace EventController_rQP
{

    public static class EventController_Work
    {
        public static OngoingEvent ongoingEvents = OngoingEvent.None;

        public static HashSet<FactionDef> GetHumanlikeVanillaFactions()
        {
            return FactionFilter_Init.vanillaHumanlikeFactions;
        }
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
            switch (ongoingEvents)
            {
                case OngoingEvent.Trader:
                case OngoingEvent.Carrier:
                case OngoingEvent.Guard:
                case (OngoingEvent)0b1:
                case (OngoingEvent)0b11:
                case (OngoingEvent)0b101:
                case (OngoingEvent)0b1001:
                    return "TraderGroup";
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
                default:
                    var list = new List<OngoingEvent>();
                    foreach (OngoingEvent item in Enum.GetValues(typeof(OngoingEvent)))
                    {
                        if ((ongoingEvents & item) != 0)
                        {
                            list.Add(item);
                        }
                    }
                    return string.Join(Environment.NewLine, list);
            }
        }
        public static void Prefix_PawnGroupKindWorker_GenerateTrader()
        {
            ongoingEvents |= OngoingEvent.Trader;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateTrader()
        {
            ongoingEvents &= ~OngoingEvent.Trader;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateCarriers()
        {
            ongoingEvents |= OngoingEvent.Carrier;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateCarriers()
        {
            ongoingEvents &= ~OngoingEvent.Carrier;
        }

        public static void Prefix_PawnGroupKindWorker_GenerateGuards()
        {
            ongoingEvents |= OngoingEvent.Guard;
        }

        public static void Postfix_PawnGroupKindWorker_GenerateGuards()
        {
            ongoingEvents &= ~OngoingEvent.Guard;
        }
        public static void Prefix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            ongoingEvents |= OngoingEvent.TraderGroup;
        }

        public static void Postfix_PawnGroupKindWorker_Trader_GeneratePawns()
        {
            ongoingEvents &= ~OngoingEvent.TraderGroup;
        }
        public static void Prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            ongoingEvents |= OngoingEvent.RefugeePodCrash;
        }

        public static void Postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        {
            ongoingEvents &= ~OngoingEvent.RefugeePodCrash;
        }
        [HarmonyPriority(200)]
        public static void Prefix_GiveAppropriateBioAndNameTo(ref Pawn pawn, ref FactionDef factionType)
        {
            try { ongoingEvents |= OngoingEvent.FactionFix; FactionFilter_Work.FactionFilter(ref pawn, ref factionType); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.FactionFix; }
        }
        public static void Postfix_GiveAppropriateBioAndNameTo()
        {
            ongoingEvents &= ~OngoingEvent.FactionFix;
        }
        public static void Prefix_GenerateSkills(ref Pawn pawn)
        {
            try { ongoingEvents |= OngoingEvent.FactionLeaderValidator; if (RealFactionGuestSettings.factionLeaderValidator) PawnValidator_CrossWork.FactionLeaderValidator(ref pawn); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.FactionLeaderValidator; }
        }
        [HarmonyPriority(1000)]
        public static bool Prefix_FillBackstorySlotShuffled(ref Pawn pawn, ref BackstorySlot slot, ref List<BackstoryCategoryFilter> backstoryCategories, ref FactionDef factionType)
        {
            try { ongoingEvents |= OngoingEvent.BackstoryFix; FactionFilter_Work.IncludeStoryCategories(pawn, slot, ref backstoryCategories); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.BackstoryFix; }
            bool flag = pawn.kindDef.fixedChildBackstories.Any();
            bool flag2 = pawn.kindDef.fixedAdultBackstories.Any();
            bool flag3 = pawn.ageTracker.AgeBiologicalYearsFloat >= 20f;
            if (slot == BackstorySlot.Childhood)
            {
                FillBackstorySlotShuffled(pawn, BackstorySlot.Childhood, backstoryCategories, factionType, flag3 ? new BackstorySlot?(BackstorySlot.Adulthood) : null);
            }
            else
            {
                FillBackstorySlotShuffled(pawn, BackstorySlot.Adulthood, backstoryCategories, factionType);
            }
            return false;
        }
        #region ces1
        private static float BackstorySelectionWeight(BackstoryDef bs)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bs.workDisables);
        }

        private static float BioSelectionWeight(PawnBio bio)
        {
            return SelectionWeightFactorFromWorkTagsDisabled(bio.adulthood.workDisables | bio.childhood.workDisables);
        }

        private static float SelectionWeightFactorFromWorkTagsDisabled(WorkTags wt)
        {
            float num = 1f;
            if ((wt & WorkTags.ManualDumb) != 0)
            {
                num *= 0.5f;
            }
            if ((wt & WorkTags.ManualSkilled) != 0)
            {
                num *= 1f;
            }
            if ((wt & WorkTags.Violent) != 0)
            {
                num *= 0.6f;
            }
            if ((wt & WorkTags.Social) != 0)
            {
                num *= 0.7f;
            }
            if ((wt & WorkTags.Intellectual) != 0)
            {
                num *= 0.4f;
            }
            if ((wt & WorkTags.Firefighting) != 0)
            {
                num *= 0.8f;
            }
            return num;
        }
        private static readonly BackstoryCategoryFilter FallbackCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string> { "Civil" },
            commonality = 1f
        };
        public static void FillBackstorySlotShuffled(Pawn pawn, BackstorySlot slot, List<BackstoryCategoryFilter> backstoryCategories, FactionDef factionType, BackstorySlot? mustBeCompatibleTo = null)
        {
            BackstoryCategoryFilter categoryFilter = backstoryCategories.RandomElementByWeight((BackstoryCategoryFilter c) => c.commonality) ?? FallbackCategoryGroup;
            IEnumerable<BackstoryDef> source = DefDatabase<BackstoryDef>.AllDefs.Where((BackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs));
            List<BackstoryDef> tmpBackstories = new List<BackstoryDef>();
            tmpBackstories.Clear();
            if (!mustBeCompatibleTo.HasValue)
            {
                tmpBackstories.AddRange(source.Where((BackstoryDef bs) => bs.slot == slot));
            }
            else
            {
                IEnumerable<BackstoryDef> compatibleBackstories = source.Where((BackstoryDef bs) => bs.slot == mustBeCompatibleTo.Value);
                tmpBackstories.AddRange(source.Where((BackstoryDef bs) => bs.slot == slot && compatibleBackstories.Any((BackstoryDef b) => !b.requiredWorkTags.OverlapsWithOnAnyWorkType(bs.workDisables))));
            }
            if (!(from bs in tmpBackstories.TakeRandom(20)
                  where (slot != BackstorySlot.Adulthood || bs.requiredWorkTags == WorkTags.None || !bs.requiredWorkTags.OverlapsWithOnAnyWorkType(pawn.story.Childhood.workDisables)) ? true : false
                  select bs).TryRandomElementByWeight(BackstorySelectionWeight, out var result))
            {
                Log.Error(string.Concat("No shuffled ", slot, " found for ", pawn.ToStringSafe(), " of ", factionType.ToStringSafe(), ". Choosing random."));
                result = DefDatabase<BackstoryDef>.AllDefs.Where((BackstoryDef bs) => bs.slot == slot).RandomElement();
            }
            if (slot == BackstorySlot.Adulthood)
            {
                pawn.story.Adulthood = result;
            }
            else
            {
                pawn.story.Childhood = result;
            }
            tmpBackstories.Clear();
        }
        #endregion
        public static void Postfix_FillBackstorySlotShuffled()
        {
            ongoingEvents &= ~OngoingEvent.BackstoryFix;
        }
        public static void Prefix_GenerateOrRedressPawnInternal(ref PawnGenerationRequest request)
        {
            try { ongoingEvents |= OngoingEvent.InternalGen; PawnValidator_CrossWork.RequestValidator(ref request); }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.InternalGen; }
        }
        public static void Prefix_DamageUntilDowned()
        {
            ongoingEvents |= OngoingEvent.DamageUntilDowned;
        }
        public static void Postfix_DamageUntilDowned()
        {
            ongoingEvents &= ~OngoingEvent.DamageUntilDowned;
        }
        public static bool Prefix_AdjustXenotypeForFactionlessPawn(ref Pawn pawn)
        {
            try { ongoingEvents |= OngoingEvent.AdjustXenotype; return RealFactionGuestSettings.dontAdjustXenotypeForRabbie ? PawnValidator_CrossWork.IsAdjustXenotype(ref pawn) : true; }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.AdjustXenotype; return true; }
        }
        #region DamageUntilDowned
        public static bool Prefix_PreApplyDamage(ref bool absorbed)
        {
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static bool Prefix_PreApplyDamageThing(ref bool absorbed)
        {
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static bool Prefix_PreApplyDamageThingWithComps(ThingWithComps __instance, ref bool absorbed)
        {
            if (__instance.Map == null)
            {
                absorbed = false;
                return false;
            }
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        public static bool Prefix_PreApplyDamagePawn(Pawn __instance, ref bool absorbed)
        {
            if (__instance.Map == null)
            {
                absorbed = false;
                return false;
            }
            return RealFactionGuestSettings.damageUntilDownedBypassShield ? PawnValidator_CrossWork.IsNotBypassShield(ref absorbed) : true;
        }
        #endregion
        public static void Postfix_PawnGenerator_GeneratePawn(ref Pawn __result)
        {
            try
            {
                ongoingEvents |= OngoingEvent.CreepJoinerValidator;
                if (RealFactionGuestSettings.creepJoinerGenerateNoLimit && __result.kindDef is CreepJoinerFormKindDef) PawnValidator_CrossWork.CreepJoinerValidator(ref __result);
                /*Log.Message($"A : {__result.kindDef}");*/
            }
            catch { Log.Error("Real Faction Guest: " + GetOngoingEvent() + " Failed"); ongoingEvents &= ~OngoingEvent.CreepJoinerValidator; }
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
            ongoingEvents = OngoingEvent.None;
        }
    }
}
