using EventController_rQP;
using RimWorld;
using Verse;

namespace raceQuestPawn
{
    //working in process maybe?
    internal class RefugeePotCrash_PawnValidator
    {
        public static PawnGenerationRequest PostProcessRequest(PawnGenerationRequest request)
        {
            if (EventController_Work.IsRefugeePodCrash())
            {
                request.AllowDowned = true;
            }
            return request;
        }
    }
}
