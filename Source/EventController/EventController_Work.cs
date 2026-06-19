using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
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
        public static Dictionary<ThingDef, HashSet<FactionDef>> GetRacePawnHiddenFactions()
        {
            return FactionFilter_Init.racePawnHiddenFactions;
        }
        public static Dictionary<ThingDef, HashSet<FactionDef>> GetRacePawnFactions()
        {
            return FactionFilter_Init.racePawnFactions;
        }
        public static HashSet<FactionDef> GetHumanlikeVanillaFactions()
        {
            return FactionFilter_Init.vanillaHumanlikeFactions;
        }
        public static float GetHumanlikeModFactionNum()
        {
            return FactionFilter_Init.humanlikeModFactionNum;
        }
        public static HashSet<FactionDef> GetAllHumanLikeFactions()
        {
            return FactionFilter_Init.allHumanlikeFactions;
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
            var tempEvent = OngoingEvent.FactionFix;
            try { ongoingEvents |= tempEvent; FactionFilter_Work.FactionFilter(ref pawn, ref factionType); }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
        }
        public static void Postfix_GiveAppropriateBioAndNameTo()
        {
            ongoingEvents &= ~OngoingEvent.FactionFix;
        }
        public static void Prefix_GenerateSkills(ref Pawn pawn)
        {
            var tempEvent = OngoingEvent.FactionLeaderValidator;
            try
            {
                ongoingEvents |= tempEvent;
                if (RealFactionGuestSettings.factionLeaderValidator && pawn.kindDef.factionLeader)
                {
                    PawnValidator_CrossWork.FactionLeaderValidator(ref pawn);
                    return;
                }
                //PawnValidator_CrossWork.WorkDisableValidator(ref pawn);
            }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
        }
        [HarmonyPriority(1000)]
        public static void Prefix_FillBackstorySlotShuffled(ref Pawn pawn, ref BackstorySlot slot, ref List<BackstoryCategoryFilter> backstoryCategories, ref FactionDef factionType)
        {
            var tempEvent = OngoingEvent.BackstoryFix;
            try
            {
                ongoingEvents |= tempEvent;
                //FactionFilter_Work.ExcludeStoryCategories(pawn, ref backstoryCategories, factionType);
                FactionFilter_Work.IncludeStoryCategories(pawn, slot, ref backstoryCategories);
            }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
        }
        public static void Postfix_FillBackstorySlotShuffled()
        {
            ongoingEvents &= ~OngoingEvent.BackstoryFix;
        }
        public static void Prefix_GenerateOrRedressPawnInternal(ref PawnGenerationRequest request)
        {
            var tempEvent = OngoingEvent.InternalGen;
            try { ongoingEvents |= tempEvent; PawnValidator_CrossWork.RequestValidator(ref request); }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
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
            var tempEvent = OngoingEvent.AdjustXenotype;
            bool result = true;
            try { ongoingEvents |= tempEvent; result = RealFactionGuestSettings.dontAdjustXenotypeForRabbie ? PawnValidator_CrossWork.IsAdjustXenotype(ref pawn) : true; }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
            return result;
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
            var tempEvent = OngoingEvent.CreepJoinerValidator;
            try
            {
                ongoingEvents |= tempEvent;
                if (RealFactionGuestSettings.creepJoinerGenerateNoLimit && __result.kindDef is CreepJoinerFormKindDef) PawnValidator_CrossWork.CreepJoinerValidator(ref __result);
                /*Log.Message($"A : {__result.kindDef}");*/
            }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
            EventController_Reset();
        }
        #region issue fix of ver 1.6.4850
        public static void Prefix_IncidentWorker_GiveQuest_GiveQuest(IncidentWorker __instance, IncidentParms parms, QuestScriptDef questDef)
        {
            // slate is initialized in this method, no ongoing event
            GlobalParams.SetCachedIncident(__instance.def);
        }
        public static void Prefix_QuestUtility_GenerateQuestAndMakeAvailable_B(QuestScriptDef root, Slate vars)
        {
            IncidentDef def = GlobalParams.GetCachedIncidentAndReset();
            if (def != null)
            {
                vars.Set("RFG_incident_cache", def);
            }
        }
        public static void Prefix_QuestNode_Root_WandererJoin_RunInt()
        {
            var tempEvent = OngoingEvent.WandererJoin;
            try
            {
                ongoingEvents |= tempEvent;
                var slate = QuestGen.slate;
                IncidentDef incident;
                //Log.Message(slate.ToString());
                if (!slate.TryGet("RFG_incident_cache", out incident))
                {
                    return;
                }
                if (incident == null)
                {
                    return;
                }
                Gender? fixedGender = null;
                if (!slate.TryGet<PawnGenerationRequest>("overridePawnGenParams", out PawnGenerationRequest var, false))
                {
                    var = new PawnGenerationRequest(PawnKindDefOf.Villager, null, PawnGenerationContext.NonPlayer, -1, true, false, false, true, false, 20f, false, true, true, true, true, false, false, false, false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, fixedGender, null, null, null, null, false, false, false, false, null, null, null, null, null, 0f, DevelopmentalStage.Adult, null, null, null, true, false, false, -1, 0, false);
                    if (incident.pawnKind != null)
                    {
                        var.KindDef = incident.pawnKind;
                        slate.Set("overridePawnGenParams", var);
                    }
                }
            }
            catch (Exception ex) { Tools.HandleEventControllerError(ex, tempEvent); }
        }
        #endregion
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
