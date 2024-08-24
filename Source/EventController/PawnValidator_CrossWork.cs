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
        }
        public static bool IsNotValidCreepjoinerRequest_Fix(ref PawnGenerationRequest request)
        {
            if (request.KindDef is CreepJoinerFormKindDef && !request.IsCreepJoiner)
            {
                var faction = request.Faction.def;
                var kinddef = request.KindDef;
                if (faction.pawnGroupMakers != null)
                {
                    float combatPower = kinddef.combatPower;
                    PawnKindDef p_make = null;
                    int i = 0;
                    while (p_make == null && i < 10)
                    {
                        i++;
                        if (faction.pawnGroupMakers != null)
                        {
                            p_make = ChoosePawn.ChoosePawnKind(faction.pawnGroupMakers, combatPower);
                        }
                        if (p_make != null)
                        {
                            request.KindDef = p_make;
                            return true;
                        }
                        else
                        {
                            var fallback1 = EventController_Work.GetValidFactions_RPC().RandomElement();
                            p_make = ChoosePawn.ChoosePawnKind(fallback1.pawnGroupMakers, combatPower);
                            if (p_make != null)
                            {
                                request.KindDef = p_make;
                                return true;
                            }
                        }
                    }
                    if (p_make == null)
                    {
                        var fallback2 = EventController_Work.GetVanillaFactions().RandomElement();
                        p_make = ChoosePawn.ChoosePawnKind(fallback2.pawnGroupMakers, combatPower);
                        if (p_make != null)
                        {
                            request.KindDef = p_make;
                        }
                    }
                }
            }
            return false;
        }
        public static bool IsNotFromVanilla()
        {
            var stack = new StackTrace(0, true);
            var frame = stack.GetFrame(3);
            var ns = frame.GetMethod().DeclaringType.Namespace;
            if (ns == "Verse" || ns == "RimWorld" || EventController_Work.isRefugeePodCrash
                || (from frame1 in stack.GetFrames() select frame1.GetMethod().DeclaringType).Any(t => t == typeof(IncidentWorker)))
            {
                return false;
            }
            else
            {
                return true;
            }
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
    }
}
