using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace EventController_rQP
{

    public class EventController_Work
    {
        public static bool isTrader = false;
        public static bool isCarrier = false;
        public static bool isGuard = false;
        public static bool isEnd = false;
        public static bool isRefugeePodCrash = false;
        public static bool isInternalGen = false;
        public static bool isCreepJoiner = false;
        public static bool isFactionFix = false;
        public static bool isBackstoryFix = false;
        private static readonly BackstoryCategoryFilter FallbackCategoryGroup = new BackstoryCategoryFilter
        {
            categories = new List<string> { "Civil" },
            commonality = 1f
        };

        public static string ongoingEvent = null;

        public static float GetHumanlikeModFactionNum()
        {
            return FactionFilter_Init.humanlikeModFactionNum;
        }
        public static Dictionary<FactionDef, HashSet<string>> GetFactionBackstoryCategories()
        {
            return FactionFilter_Init.factionBackstoryCategories;
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
            if (request.KindDef.RaceProps.IsAnomalyEntity)
            {
                isCreepJoiner = true;
            }
            else
            {
                isCreepJoiner = false;
            }
            return isCreepJoiner;
        }

        public static bool IsRefugeePodCrash()
        {
            var stack = new StackTrace(0, true);
            if (stack.FrameCount < 18)
            {
                return isRefugeePodCrash = false;
            }
            var frame = stack.GetFrame(8);
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
            isEnd = false;
        }

        public static void Postfix_PawnGroupKindWorker_GeneratePawns()
        {
            isEnd = true;
        }
        public static void Prefix_GenerateNewPawnInternal(ref PawnGenerationRequest request)
        {
            if (IsRefugeePodCrash())
            {
                request.AllowDowned = true;
            }
            isInternalGen = true;
        }

        public static void Postfix_GenerateNewPawnInternal()
        {
            isInternalGen = false;
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
        public static void Prefix_FillBackstorySlotShuffled(ref Pawn pawn,ref BackstorySlot slot,ref List<BackstoryCategoryFilter> backstoryCategories,ref FactionDef factionType)
        {
            if (pawn.kindDef.backstoryFiltersOverride == null)
            {
                for (global::System.Int32 i = 0; i < backstoryCategories.Count; i++)
                {
                    if (backstoryCategories[i].categories != null)
                    {
                        continue;
                    }
                    if (backstoryCategories[i].categoriesChildhood == null && backstoryCategories[i].categoriesAdulthood != null)
                    {
                        HashSet<string> childhood = [];
                        var factionPawnKinds = EventController_Work.GetFactionPawnKinds();
                        foreach (var faction in FactionFilter_Work.validFactions)
                        {
                            var filterKinds = factionPawnKinds[faction].Where(f => !f.backstoryFiltersOverride.Any());
                            foreach (var kind in filterKinds)
                            {
                                foreach (var bkfo in kind.backstoryFiltersOverride)
                                {
                                    childhood.Union(bkfo.categories);
                                }
                            }
                        }
                        if (childhood.Any())
                        {
                            backstoryCategories[i].categoriesChildhood.Union(childhood);
                        }
                        else
                        {
                            backstoryCategories[i].categoriesChildhood = new List<string> { "Civil" };
                        }
                    }
                }
            }
            FactionFilter_Work.BackstoryFilter(ref pawn, ref backstoryCategories, ref factionType);
            isBackstoryFix = true;
        }

        public static void Postfix_FillBackstorySlotShuffled(ref Pawn pawn,ref BackstorySlot slot,ref List<BackstoryCategoryFilter> backstoryCategories,ref FactionDef factionType)
        {
            isBackstoryFix = false;
            return;
        }
    }
}
