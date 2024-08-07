using Microsoft.SqlServer.Server;
using RimWorld;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Verse;
using static UnityEngine.GraphicsBuffer;

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

        public static string ongoingEvent = null;

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
            //need test
            //if (IsCreepJoiner(ref request))
            //{
            //    request.IsCreepJoiner = true;
            //}
            isInternalGen = true;
        }

        public static void Postfix_GenerateNewPawnInternal()
        {
            isInternalGen = false;
        }
        //public static void Prefix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        //{
        //    isRefugeePodCrash = true;
        //}

        //public static void Postfix_QuestNode_Root_RefugeePodCrash_GeneratePawn()
        //{
        //    isRefugeePodCrash = false;
        //}
    }
}
