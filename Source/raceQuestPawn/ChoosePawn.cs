using EventController_rQP;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace raceQuestPawn
{
    public static class ChoosePawn
    {
        public static PawnKindDef ChoosePawnKind(List<PawnGroupMaker> plans, float combatPower, bool flag = true)
        {
            var traders = plans.Where(t => t.traders != null).Select(t => t.traders);
            var guards = plans.Where(t => t.guards != null).Select(t => t.guards);
            PawnKindDef p = null;
            if (EventController_Work.isTrader && traders.Any())
            {
                p = ChoosePawnKindInner(traders, combatPower, flag);
            }
            //carrier is skipped.
            else if (EventController_Work.isGuard && guards.Any())
            {
                p = ChoosePawnKindInner(guards, combatPower, flag);
            }
            if (p == null)
            {
                var options = plans.Where(t => t.options != null).Select(t => t.options);
                p = ChoosePawnKindInner(options, combatPower, flag);
            }
            return p;
        }
        public static PawnKindDef ChoosePawnKindInner(IEnumerable<List<PawnGenOption>> options, float combatPower, bool flag = true)
        {
            var pawnKinds =
                from p in options
                from t in p
                select t.kind;
            var pawnToChoose =
                from p in pawnKinds
                where Mathf.Abs(p.combatPower - combatPower) < 30f
                select p;
            var pawnEquals =
                from p in pawnToChoose
                where p.combatPower == combatPower
                select p;
            if (pawnToChoose.Any())
            {
                // only one = request combatPower
                if (pawnEquals.Any())
                {
                    return pawnEquals.ToHashSet().RandomElement();
                }
                // other situations, get random one
                else
                {
                    return pawnToChoose.ToHashSet().RandomElement();
                }
            }
            else if (flag)
            {
                return ChoosePawnKindInner_A(pawnKinds, combatPower);
            }
            return null;
        }
        public static PawnKindDef ChoosePawnKindInner_A(IEnumerable<PawnKindDef> pawnKinds, float combatPower)
        {
            IEnumerable<PawnKindDef> pawnEquals = [];
            var combatPowerArray =
                (from p in pawnKinds
                 select p.combatPower).ToArray();
            var nearpower = combatPowerArray.MinBy(a => Mathf.Abs(a - combatPower));
            pawnEquals =
                from p in pawnKinds
                where (p.combatPower - nearpower) < 30f
                select p;
            if (pawnEquals.Any())
            {
                return pawnEquals.ToHashSet().RandomElement();
            }
            return null;
        }
    }
}
