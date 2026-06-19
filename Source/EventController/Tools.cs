using HarmonyLib;
using System;
using System.Reflection;
using Verse;
using System.Linq;
using System.Collections.Generic;

#if DEBUG
using System.Diagnostics;
#endif
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
        public static void PatchTool(this MethodInfo methodInfo, Type type, ref Harmony harmony, string variableName, PatchType p)
        {
            MethodInfo prefix = null;
            MethodInfo postfix = null;
            MethodInfo transpiler = null;
            switch (p)
            {
                case PatchType.Prefix:
                    prefix = type.GetMethod("Prefix_" + variableName);
                    harmony.Patch(methodInfo, prefix);
                    return;
                case PatchType.Postfix:
                    postfix = type.GetMethod("Postfix_" + variableName);
                    harmony.Patch(methodInfo, null, postfix);
                    return;
                case PatchType.Transpiler:
                    transpiler = type.GetMethod("Transpiler_" + variableName);
                    harmony.Patch(methodInfo, null, null, transpiler);
                    return;
                case PatchType.Both:
                    prefix = type.GetMethod("Prefix_" + variableName);
                    postfix = type.GetMethod("Postfix_" + variableName);
                    break;
            }
            if (RealFactionGuestSettings.debugOption)
            {
                Log.Message("Patch: " + $"*{variableName}*".Colorize(UnityEngine.Color.blue));
            }
            if (p == PatchType.Both)
            {
                if (postfix == null && prefix != null)
                {
                    harmony.Patch(methodInfo, prefix);
                    return;
                }
                if (prefix == null && postfix != null)
                {
                    harmony.Patch(methodInfo, null, postfix);
                    return;
                }
                harmony.Patch(methodInfo, prefix, postfix);
            }
        }
#if DEBUG
        public static void GetStackTraceInfo(StackTrace stack)
        {
            var frames = stack.GetFrames();
            Log.Message(string.Join(Environment.NewLine,(IEnumerable<StackFrame>) frames));
        }
#endif
        public static string GetShortExceptionString(Exception ex)
        {
            var messages = new List<string>();
            var current = ex;
            while (current != null)
            {
                messages.Add($"{current.GetType().FullName}: {current.Message}");
                current = current.InnerException;
            }
            string messageChain = string.Join(" ---> ", messages);

            var baseEx = ex.GetBaseException();
            var topStackLine = baseEx.StackTrace
                ?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?.Trim();

            if (string.IsNullOrEmpty(topStackLine))
            {
                return messageChain;
            }
            return $"{messageChain}{Environment.NewLine}{topStackLine}";
        }
        public static void HandleEventControllerError(Exception ex, OngoingEvent ongoingEvent)
        {
            Log.Error(OrganizeErrorMsg(ex));
            EventController_Work.ongoingEvents &= ~ongoingEvent;
        }
        public static string OrganizeErrorMsg(Exception ex)
        {
            return "Real Faction Guest: " + EventController_Work.GetOngoingEvent() + " Failed.\n" + GetShortExceptionString(ex);
        }
    }
}
