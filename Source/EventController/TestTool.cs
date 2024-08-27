using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using Verse;

namespace EventController_rQP
{
    public class TestTool
    {
        public void TestTool_ForceCreepJoiner(ref PawnGenerationRequest request)
        {
            if (QuestGen_Get.GetMap() != null)
            {
                if (!request.KindDef.factionLeader)
                {
                    request.KindDef = DefDatabase<CreepJoinerFormKindDef>.AllDefs.RandomElement();
                    request.IsCreepJoiner = true;
                }
            }
        }
        public void TestTool_ForceRabbie(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
        public void TestTool_ForceGenerateWithCertainFaction(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
        public void TestTool_ForceGenerateWithCertainApparel(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
        private static readonly List<CreepJoinerBaseDef> requires = new List<CreepJoinerBaseDef>();

        private static readonly List<CreepJoinerBaseDef> exclude = new List<CreepJoinerBaseDef>();

        private static readonly List<ICreepJoinerDef> temp = new List<ICreepJoinerDef>();

        public static void GetCreepjoinerSpecifics(Map map, ref CreepJoinerFormKindDef form, ref CreepJoinerBenefitDef benefit, ref CreepJoinerDownsideDef downside, ref CreepJoinerAggressiveDef aggressive, ref CreepJoinerRejectionDef rejection)
        {
            //var method = AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow), new System.Type[] { typeof(IIncidentTarget) });
            //float combatPoints = (float)method.Invoke(null, new object[] { map });
            //float combatPoints = map == null ? 100f : StorytellerUtility.DefaultThreatPointsNow(map);
            float combatPoints = map == null ? 100f : StorytellerUtility.DefaultThreatPointsNow(map);
            if (form == null)
            {
                form = DefDatabase<CreepJoinerFormKindDef>.AllDefsListForReading.RandomElementByWeight((CreepJoinerFormKindDef x) => x.Weight);
            }
            requires.AddRange(form.Requires);
            exclude.AddRange(form.Excludes);
            if (benefit == null)
            {
                benefit = CreepJoinerUtility.GetRandom(DefDatabase<CreepJoinerBenefitDef>.AllDefsListForReading, combatPoints, requires, exclude);
            }
            if (downside == null)
            {
                downside = CreepJoinerUtility.GetRandom(DefDatabase<CreepJoinerDownsideDef>.AllDefsListForReading, combatPoints, requires, exclude);
            }
            if (aggressive == null)
            {
                aggressive = CreepJoinerUtility.GetRandom(DefDatabase<CreepJoinerAggressiveDef>.AllDefsListForReading, combatPoints, requires, exclude);
            }
            if (rejection == null)
            {
                rejection = CreepJoinerUtility.GetRandom(DefDatabase<CreepJoinerRejectionDef>.AllDefsListForReading, combatPoints, requires, exclude);
            }
            exclude.Clear();
            requires.Clear();
        }
    }
}
