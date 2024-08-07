using EventController_rQP;
using RimWorld;
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
        public static bool IsRefugeePodCrash()
        {
            return EventController_Work.IsRefugeePodCrash();
        }
        //wip for backstory filter
        //this is a space for a method
    }
}
