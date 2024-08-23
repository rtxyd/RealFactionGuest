using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace EventController_rQP
{
    //working in process maybe?
    public class PawnValidator_CrossWork
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
            if (EventController_Work.isRefugeePodCrash
                || request.KindDef.defName == "Mincho_SpaceRefugee"
                || request.KindDef.defName == "Mincho_SpaceRefugee_Clothed")
            {
                request.AllowDowned = true;
            }
            if (request.KindDef.defName == "RatkinPriest")
            {
                request.MustBeCapableOfViolence = false;
            }
            if (request.KindDef is CreepJoinerFormKindDef)
            {
                request.IsCreepJoiner = true;
            }
        }
        public static bool IsFromVanilla()
        {
            var stack = new StackTrace(0, true);
            var frame = stack.GetFrame(3);
            var ns = frame.GetMethod().DeclaringType.Namespace;
            var flag = true;
            if (ns == "Verse" || ns == "RimWorld" || EventController_Work.isRefugeePodCrash || stack.GetFrames().Any(t => t.GetMethod().DeclaringType == typeof(IncidentWorker)))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }
    }
}
