using EventController_rQP;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace raceQuestPawn
{
    //working in process maybe?
    internal class PawnValidator_CrossWork
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
    }
}
