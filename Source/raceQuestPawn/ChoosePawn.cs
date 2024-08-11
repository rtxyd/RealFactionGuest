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
        public static PawnKindDef ChoosePawnKind(List<PawnGroupMaker> plans, float combatPower)
        {
            var traders = plans.Where(t => t.traders != null).Select(t => t.traders);
            var guards = plans.Where(t => t.guards != null).Select(t => t.guards);
            PawnKindDef p = null;
            if (EventController_Work.isTrader && traders.Any())
            {
                p = ChoosePawnKindInner(traders, combatPower);
            }
            //carrier is skipped.
            else if (EventController_Work.isGuard && guards.Any())
            {
                p = ChoosePawnKindInner(guards, combatPower);
            }
            if (p == null)
            {
                var options = plans.Where(t => t.options != null).Select(t => t.options);
                p = ChoosePawnKindInner(options, combatPower);
            }
            return p;
        }
        public static PawnKindDef ChoosePawnKindInner(IEnumerable<List<PawnGenOption>> options, float combatPower)
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
            else
            {
                return ChoosePawnKindInner_A(pawnKinds, combatPower);
            }
        }
        public static PawnKindDef ChoosePawnKindInner_A(IEnumerable<PawnKindDef> pawnKinds, float combatPower)
        {
            IEnumerable<PawnKindDef> pawnEquals = [];
            var combatPowerArray =
                (from p in pawnKinds
                 select p.combatPower).ToArray();
            var maxCombatPower = combatPowerArray.Max();
            var minCombatPower = combatPowerArray.Min();
            if (combatPower > maxCombatPower)
            {
                pawnEquals =
                    from p in pawnKinds
                    where p.combatPower == maxCombatPower
                    select p;
            }
            else
            {
                pawnEquals =
                    from p in pawnKinds
                    where p.combatPower == minCombatPower
                    select p;
            }
            if (pawnEquals.Any())
            {
                return pawnEquals.ToHashSet().RandomElement();
            }
            return null;
        }
    }
}
