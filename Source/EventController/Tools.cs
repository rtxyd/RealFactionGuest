using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace EventController_rQP
{
    public static class Tools
    {
        public static MethodType methodType = new MethodType() { a = BindingFlags.NonPublic | BindingFlags.Instance, b = BindingFlags.NonPublic | BindingFlags.Static };

        public static MethodInfo MethodTool(ParamValue paramValue, Type type, string methodName)
        {
            BindingFlags Instance;
            if (paramValue == ParamValue.a)
            {
                Instance = methodType.a;
                return type.GetMethod(methodName, Instance);
            }
            else if (paramValue == ParamValue.b)
            {
                Instance = methodType.b;
                return type.GetMethod(methodName, Instance);
            }
            return null;
        }
        public static string ToUpperFirstLetter(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }
        public static void PatchTool(this MethodInfo methodInfo, Type type, ref Harmony harmony, string variableName, PatchType p)
        {
            MethodInfo prefix = null;
            MethodInfo postfix = null;
            switch (p)
            {
                case PatchType.Prefix:
                    prefix = type.GetMethod("Prefix_" + variableName);
                    break;
                case PatchType.Postfix:
                    postfix = type.GetMethod("Postfix_" + variableName);
                    break;
                case PatchType.Both:
                    prefix = type.GetMethod("Prefix_" + variableName);
                    postfix = type.GetMethod("Postfix_" + variableName);
                    break;
            }
            if (RealFactionGuestSettings.debugOption)
            {
                Log.Message("Patch: " + $"*{variableName}*".Colorize(UnityEngine.Color.blue));
            }
            if (postfix == null && prefix != null)
            {
                harmony.Patch(methodInfo, prefix);
                return;
            }
            if (postfix == null && prefix == null)
            {
                Log.Error("Patch failed: " + $"*{variableName}*".Colorize(UnityEngine.Color.blue));
                return;
            }
            if (prefix == null && postfix != null)
            {
                harmony.Patch(methodInfo, null, postfix);
                return;
            }
            harmony.Patch(methodInfo, prefix, postfix);
        }
        public static void TestTool_ForceCreepJoiner(ref PawnGenerationRequest request, ref bool iscreeped)
        {
            if (iscreeped)
            {
                iscreeped = false;
                return;
            }
            request.KindDef = DefDatabase<CreepJoinerFormKindDef>.AllDefs.RandomElement();
            request.IsCreepJoiner = true;
            iscreeped = true;
        }
        public static void TestTool_ForceRabbie(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
        public static void TestTool_ForceGenerateWithCertainFaction(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
        public static void TestTool_ForceGenerateWithCertainApparel(ref PawnGenerationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
