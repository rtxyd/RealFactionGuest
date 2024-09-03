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
        public static void RequestValidator(ref PawnGenerationRequest request)
        {
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
                    return;
                }
                else
                {
                    ValidateRequestKindDef(ref request);
                    return;
                }
            }
            if ((EventController_Work.ongoingEvents & OngoingEvent.RefugeePodCrash) != 0)
            {
                request.AllowDowned = true;
            }
        }
        public static bool IsNotFromVanilla()
        {
            if ((EventController_Work.ongoingEvents & OngoingEvent.RefugeePodCrash) != 0)
            {
                return false;
            }
            return StackCheckIsNotVanilla(6);
        }
        public static bool StackCheckIsNotVanilla(int maxFrames)
        {
            var stackTrace = new StackTrace();
            int frameCount = Math.Min(stackTrace.FrameCount, maxFrames);
            var frame = stackTrace.GetFrame(3);
            var method = frame.GetMethod();
            var ns = method.DeclaringType.Namespace;
            if (ns == "Verse" || ns == "RimWorld")
            {
                return false;
            }
            for (int i = 4; i < frameCount; i++)
            {
                frame = stackTrace.GetFrame(i);
                method = frame.GetMethod();
                if (method.Name.Contains("IncidentWorker"))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsNotBypassShield(ref bool absorbed)
        {
            if ((EventController_Work.ongoingEvents & OngoingEvent.DamageUntilDowned) != 0)
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
                              select bs).RandomElement() ??
                              ((Func<BackstoryDef>)(() =>
                              {
                                  IEnumerable<BackstoryDef> source = DefDatabase<BackstoryDef>.AllDefs.Where((BackstoryDef bs) => bs.shuffleable && fallback.Matches(bs));
                                  return (from bs in source.ToList()
                                          where bs.slot == slot && (bs.workDisables & WorkTags.Violent) == 0
                                          select bs).RandomElement();
                              }))();
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
            if (pawn.creepjoiner == null)
            {
                pawn.creepjoiner = new Pawn_CreepJoinerTracker(pawn);
            }
            var map = QuestGen_Get.GetMap();
            CreepJoinerUtility.GetCreepjoinerSpecifics(map, ref pawn.creepjoiner.form, ref pawn.creepjoiner.benefit, ref pawn.creepjoiner.downside, ref pawn.creepjoiner.aggressive, ref pawn.creepjoiner.rejection);
        }
    }
}
