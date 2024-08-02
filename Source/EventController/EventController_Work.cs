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

    public class EventController_Work
    {
        public static bool isTrader = false;
        public static bool isCarrier = false;
        public static bool isGuard = false;
        public static bool isEnd = false;
        public static string ongoingEvent = null;

        public static TSource source
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
            isEnd = false;
        }
    }
}
