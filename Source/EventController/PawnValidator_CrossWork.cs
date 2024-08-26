using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace EventController_rQP
{
    //working in process maybe?
    public static class PawnValidator_CrossWork
    {
        public static PawnGenerationRequest PostProcessRequest(PawnGenerationRequest request)
        {
            if (EventController_Work.isRefugeePodCrash)
            {
                request.AllowDowned = true;
            }
            return request;
        }
        public static bool IsRefugeePodCrash(int f = 8)
        {
            return EventController_Work.IsRefugeePodCrash(f);
        }
        public static HashSet<FactionDef> GetValidFactions_RPC()
        {
            return EventController_Work.GetValidFactions_RPC();
        }
        public static void RequestValidator(ref PawnGenerationRequest request)
        {
            if (EventController_Work.isRefugeePodCrash)
            {
                request.AllowDowned = true;
            }
            if (request.KindDef.defName == "Mincho_SpaceRefugee"
                || request.KindDef.defName == "Mincho_SpaceRefugee_Clothed")
            {
                request.AllowDowned = true;
                return;
            }
            if (request.KindDef.defName == "RatkinPriest")
            {
                request.MustBeCapableOfViolence = false;
                return;
            }
            if (RealFactionGuestSettings.creepJoinerValidator && request.KindDef is CreepJoinerFormKindDef && !request.IsCreepJoiner)
            {
                if (RealFactionGuestSettings.creepJoinerGenerateNoLimit)
                {
                    request.IsCreepJoiner = true;
                    EventController_Work.isCreepJoinerValidatorOn = true;
                    return;
                }
                else
                {
                    ValidateRequestKindDef(ref request);
                    return;
                }
            }
        }
        public static bool IsNotFromVanilla()
        {
            var stack = new StackTrace(0, true);
            var frame = stack.GetFrame(3);
            var ns = frame.GetMethod().DeclaringType.Namespace;
            return ns == "Verse" || ns == "RimWorld" || EventController_Work.isRefugeePodCrash
                || (from frame1 in stack.GetFrames() select frame1.GetMethod().DeclaringType).Any(t => t == typeof(IncidentWorker)) ? false : true;
        }
        public static bool IsNotBypassShield(ref bool absorbed)
        {
            if (EventController_Work.isDamageUntilDowned)
            {
                absorbed = false;
                return false;
            }
            return true;
        }
        public static bool IsAdjustXenotype(ref Pawn pawn)
        {
            return pawn.kindDef.race.defName == "Rabbie" ? false : true;
        }
        public static void FactionLeaderValidator(ref Pawn pawn)
        {
            if (pawn.kindDef.factionLeader && pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                if (pawn.story.Childhood != null && (pawn.story.Childhood.workDisables & WorkTags.Violent) != 0)
                {
                    FactionLeaderValidatorInner(ref pawn, BackstorySlot.Childhood);
                }
                if (pawn.story.Adulthood != null && (pawn.story.Adulthood.workDisables & WorkTags.Violent) != 0)
                {
                    FactionLeaderValidatorInner(ref pawn, BackstorySlot.Adulthood);
                }
                if (pawn.WorkTagIsDisabled(WorkTags.Violent))
                {
                    PostFactionLeaderValidatorInner(ref pawn);
                }
            }
        }
        public static void FactionLeaderValidatorInner(ref Pawn pawn, BackstorySlot slot)
        {
            var childhood = pawn.story.Childhood;
            var adulthood = pawn.story.Adulthood;
            var pawnKind = pawn.kindDef;
            BackstoryCategoryFilter fallback = new BackstoryCategoryFilter()
            {
                categories = EventController_Work.GetFallbackBackstroy().ToList(),
                categoriesChildhood = null,
                categoriesAdulthood = null,
                commonality = 1f,
                excludeChildhood = null,
                excludeAdulthood = null,
                exclude = null
            };
            if ((pawnKind.requiredWorkTags & WorkTags.Violent) != 0 || (adulthood?.requiredWorkTags & WorkTags.Violent) != 0 || (childhood?.requiredWorkTags & WorkTags.Violent) != 0)
            {
                var categoryFilter = pawnKind.backstoryFilters != null ? pawnKind.backstoryFilters.RandomElementByWeightWithFallback((BackstoryCategoryFilter c) => c.commonality, fallback) : fallback;
                IEnumerable<BackstoryDef> source = DefDatabase<BackstoryDef>.AllDefs.Where((BackstoryDef bs) => bs.shuffleable && categoryFilter.Matches(bs));
                var result = (from bs in source.ToList()
                              where bs.slot == slot && (bs.workDisables & WorkTags.Violent) == 0
                              select bs).RandomElement();
                if (slot == BackstorySlot.Childhood)
                {
                    pawn.story.Childhood = result;
                }
                else
                {
                    pawn.story.Adulthood = result;
                }
            }
        }
        public static void PostFactionLeaderValidatorInner(ref Pawn pawn)
        {
            if (pawn.story.traits != null)
            {
                foreach (var item in pawn.story.traits.allTraits)
                {
                    List<Trait> traits = pawn.story.traits.allTraits;
                    WorkTags workTags = WorkTags.Violent;
                    for (int i = 0; i < traits.Count; i++)
                    {
                        if (!traits[i].Suppressed && (traits[i].def.disabledWorkTags & workTags) != 0)
                        {
                            pawn.story.traits.RemoveTrait(item);
                        }
                    }
                }
            }
            if (pawn.WorkTagIsDisabled(WorkTags.Violent) && pawn.genes != null)
            {
                List<Gene> genesListForReading = pawn.genes.GenesListForReading;
                WorkTags workTags = WorkTags.Violent;
                for (int i = 0; i < genesListForReading.Count; i++)
                {
                    if (genesListForReading[i].Active && (genesListForReading[i].def.disabledWorkTags & workTags) != 0)
                    {
                        pawn.genes.RemoveGene(genesListForReading[i]);
                    }
                }
            }
        }
        public static void ValidateRequestKindDef(ref PawnGenerationRequest request)
        {
            var combatPower = request.KindDef.combatPower;
            if (request.Faction?.def.pawnGroupMakers == null)
            {
                var tmpFaction = EventController_Work.GetValidFactions_RPC().RandomElement();
                var pawnKind = ChoosePawn.ChoosePawnKind(tmpFaction.pawnGroupMakers, combatPower);
                request.KindDef = pawnKind == null ? PawnKindDefOf.Refugee : pawnKind;
            }
            else
            {
                var pawnKind = ChoosePawn.ChoosePawnKind(request.Faction.def.pawnGroupMakers, combatPower);
                request.KindDef = pawnKind == null ? PawnKindDefOf.Refugee : pawnKind;
            }
        }
        public static void CreepJoinerValidator(ref Pawn pawn)
        {
            pawn.creepjoiner = new Pawn_CreepJoinerTracker(pawn);
            CreepJoinerUtility.GetCreepjoinerSpecifics(QuestGen_Get.GetMap(), ref pawn.creepjoiner.form, ref pawn.creepjoiner.benefit, ref pawn.creepjoiner.downside, ref pawn.creepjoiner.aggressive, ref pawn.creepjoiner.rejection);
        }
    }
}
