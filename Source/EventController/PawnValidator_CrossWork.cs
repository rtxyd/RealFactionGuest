﻿using RimWorld;
using System.Collections.Generic;
using Verse;

namespace EventController_rQP
{
    //working in process maybe?
    public class PawnValidator_CrossWork
    {
        public static PawnGenerationRequest PostProcessRequest(PawnGenerationRequest request)
        {
            if (IsRefugeePodCrash())
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
            if (IsRefugeePodCrash()
                || request.KindDef.defName == "Mincho_SpaceRefugee"
                || request.KindDef.defName == "Mincho_SpaceRefugee_Clothed")
            {
                request.AllowDowned = true;
            }
            if (request.KindDef.defName == "RatkinPriest")
            {
                request.MustBeCapableOfViolence = false;
            }
        }
    }
}