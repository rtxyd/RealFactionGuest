using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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
        }
        public static IEnumerable<CodeInstruction> MethodReplacer(this List<CodeInstruction> codes, MethodInfo method, OpCode start, OpCode end, List<CodeInstruction> replacer)
        {
            var codelineMethod = 0;
            for (int i = 1; i < codes.Count; i++)
            {
                if (codes[i].Calls(method))
                {
                    codelineMethod = i;
                    break;
                }
            }
            if (codelineMethod == 0)
            {
                return codes.AsEnumerable();
            }
            var startLine = 0;
            var endLine = 0;
            for (int i = codelineMethod; i >= 0; i--)
            {
                if (codes[i].opcode == start)
                {
                    startLine = i;
                    break;
                }
            }
            for (int i = codelineMethod; i < codes.Count; i++)
            {
                if (codes[i].opcode == end)
                {
                    endLine = i;
                    break;
                }
            }
            //var codes1 = new List<CodeInstruction>();
            //for (int i = 0; i < codes.Count; i++)
            //{
            //    if (startLine <= i && i < endLine)
            //    {
            //        continue;
            //    }
            //    if (i == endLine)
            //    {
            //        codes1.AddRange(replacer);
            //        continue;
            //    }
            //    codes1.Add(codes[i]);
            //}
            var methodinfo = AccessTools.Method(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow), new System.Type[] { typeof(Map) });
            DynamicMethod d = new DynamicMethod("temp", typeof(void), new Type[] { typeof(Map) });
            var il = d.GetILGenerator();
            //Label[] labels1 = new Label[]
            //    {il.DefineLabel(), il.DefineLabel()};
            replacer.Parse(ref il ,out Label next, out bool flagNext);
            if (flagNext)
            {
                endLine++;
                codes[endLine].operand = next;
            }
            //var replacer = new List<CodeInstruction>()
            //{
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Brfalse_S, labels1[0]),
            //    new CodeInstruction(OpCodes.Ldarg_0),
            //    new CodeInstruction(OpCodes.Call, methodinfo),
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Br_S, labels1[1]),
            //    new CodeInstruction(OpCodes.Ldc_R4, 100f){ labels = new List<Label> { labels1[0] } },
            //    new CodeInstruction(OpCodes.Stloc_0),
            //    new CodeInstruction(OpCodes.Nop){ labels = new List<Label> { labels1[1] } },
            //};
            var codes1 = codes.GetOpcodesLabelDictionary(replacer, ref il, startLine, endLine);
            return codes1;
            //codes1.GetOpcodesLabelDictionary(out Dictionary<Label, int> codeOprandLable, out Dictionary<Label, int> labelCode, startLine, endLine);
            //codes1.RefreshAllLabels(codeOprandLable, labelCode);
            //return codes1.AsEnumerable();
        }
        static void Parse(this List<CodeInstruction> replacer, ref ILGenerator il, out Label next, out bool flagNext)
        {
            Dictionary<TmpLabel, Label> record = new Dictionary<TmpLabel, Label>();
            flagNext = false;
            next = default;
            for (int i = 0; i < replacer.Count - 1; i++)
            {
                if (replacer[i].operand is TmpLabel j)
                {
                    //1
                    if (replacer[i].opcode == OpCodes.Nop)
                    {
                        if (!record.ContainsKey(j))
                        {
                            var label = il.DefineLabel();
                            record.Add(j, label);
                            if (replacer[i + 1].labels != null)
                            {
                                replacer[i + 1].labels.Add(label);
                            }
                            else
                            {
                                replacer[i + 1].labels = new List<Label>() { label };
                            }
                            replacer.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            if (replacer[i + 1].labels != null)
                            {
                                replacer[i + 1].labels.Add(record[j]);
                            }
                            else
                            {
                                replacer[i + 1].labels = new List<Label>() { record[j] };
                            }
                            replacer.RemoveAt(i);
                            i--;
                            continue;
                        }
                    }
                    // 2
                    else
                    {
                        if (!record.ContainsKey(j))
                        {
                            var label = il.DefineLabel();
                            record.Add(j, label);
                            replacer[i].operand = label;
                        }
                        else
                        {
                            replacer[i].operand = record[j];
                        }
                    }
                }

                //replacer[i].opcode == OpCodes.Nop ?
                //    !record.ContainsKey(j) ?
                //    replacer[i + 1].labels != null ?
                //    replacer[i + 1].labels.Add(label)  
                //    : ((Func<int>)(() => { replacer[i + 1].labels = new List<Label>() { label }; replacer.RemoveAt(i); return i--; }))()
                //    : replacer[i + 1].labels != null ? 
                //    replacer[i + 1].labels.Add(record[j]) 
                //    : ((Func<int>)(() => { replacer[i + 1].labels = new List<Label>() { record[j] }; replacer.RemoveAt(i); return i--; }))()
                //    : (Func<>)(() = > { return })
            }
            if (replacer.Last().operand is TmpLabel k)
            {
                if(replacer.Last().opcode == OpCodes.Nop)
                {
                    replacer.Last().operand = il.DefineLabel();
                }
                else
                {
                    next = record[k];
                    flagNext = true;
                }
            }
        }
        static void Parse2()
        {

        }

        public static void RefreshAllLabels(this List<CodeInstruction> oringinal, Dictionary<Label, int> codeOprandLable, Dictionary<Label, int> labelCode)
        {
            foreach (var label in codeOprandLable.Keys)
            {
                var labelNew = new Label();
                oringinal[codeOprandLable[label]].operand = labelNew;
                var labels = oringinal[labelCode[label]].labels;
                for (int i = 0; i < labels.Count; i++)
                {
                    labels.Remove(label);
                    labels.Add(labelNew);
                    oringinal[labelCode[label]].labels = labels;
                }
            }
        }
        public static void GetOpcodesLabelDictionaryInner(this List<CodeInstruction> codes, out Dictionary<Label, List<int>> codesLabelsIndex, out Dictionary<Label, List<int>> labelsCodesIndex, int start = -1)
        {
            codesLabelsIndex = new Dictionary<Label, List<int>>();
            labelsCodesIndex = new Dictionary<Label, List<int>>();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand is Label l)
                {
                    if (codesLabelsIndex.Keys.Contains(l))
                    {
                        codesLabelsIndex[l].Add(i);
                    }
                    else
                    {
                        codesLabelsIndex.Add(l, new List<int>() { i });
                    }
                    continue;
                }
                if (codes[i].labels != null)
                {
                    for (int j = 0; j < codes[i].labels.Count; j++)
                    {
                        var label = codes[i].labels[j];
                        if (labelsCodesIndex.Keys.Contains(label))
                        {
                            labelsCodesIndex[label].Add(i);
                        }
                        else
                        {
                            labelsCodesIndex.Add(label, new List<int>() { i });
                        }
                    }
                }
            }
        }
        public static IEnumerable<CodeInstruction> GetOpcodesLabelDictionary(this List<CodeInstruction> codes, List<CodeInstruction> replacer, ref ILGenerator il, int start = -1, int end = -1)
        {
            //List<CodeInstruction> codes1 = new List<CodeInstruction>();
            replacer.GetOpcodesLabelDictionaryInner(out Dictionary<Label, List<int>> replacerLabelsIndex, out Dictionary<Label, List<int>> labelsReplacerIndex);
            codes.GetOpcodesLabelDictionaryInner(out Dictionary<Label, List<int>> codesLabelsIndex, out Dictionary<Label, List<int>> labelsCodesIndex);
            List<CodeInstruction> head = new List<CodeInstruction>();
            List<CodeInstruction> tail = new List<CodeInstruction>();
            for (int i = 0; i < start + replacer.Count; i++)
            {
                il.Emit(OpCodes.Nop);
                head.Add(codes[i]);
            }

            //for(int i = end++; i < codes.Count; i++)
            //{
            //    if (codes[i].operand is Label)
            //    {
            //        il.Emit(OpCodes.Nop, il.DefineLabel());
            //    }
            //    if (codes[i].labels != null)
            //    {
            //        for (int j = 0; j < codes[i].labels.Count; j++)
            //        {
            //            var label = codes[i].labels[j];
            //            if (codesLabelsIndex.Keys.Contains(label))
            //            {
            //                var locs = codesLabelsIndex[label];
            //                for (int k = 0; k < locs.Count;k++)
            //                {
            //                    var loc = locs[k];
            //                    codes[loc].operand = 
            //                }
            //            }
            //        }
            //    }
            //    il.Emit(OpCodes.Nop);
            //}
            List<Label> record = new List<Label>();
            for (int i = end++; i < codes.Count - 1; i++)
            {
                if (codes[i + 1].operand is Label l)
                {
                    var label = il.DefineLabel();
                    il.MarkLabel(label);
                    codes[i + 1].operand = l;
                    if (!record.Contains(l))
                    {
                        for (int j = 0; j < codesLabelsIndex[l].Count; j++)
                        {
                            var labels = codes[labelsCodesIndex[l][j]].labels;
                            for (int k = 0; k < labels.Count; k++)
                            {
                                labels[k] = label;
                                codes[labelsCodesIndex[l][j]].labels = labels;
                            }
                        }
                    }
                }
                tail.Add(codes[i]);
                il.Emit(OpCodes.Nop);
            }
            tail.Add(codes.Last());
            il.Emit(OpCodes.Ret);
            //foreach (var item in replacerLabelsIndex.Keys)
            //{

            //}
            //codeOprandLable = new Dictionary<Label, int>();
            //labelCode = new Dictionary<Label, int>();
            //for (int i = 0; i < codes.Count; i++)
            //{
            //    if (codes[i].operand is Label j)
            //    {

            //        codeOprandLable.Add(j, i);
            //    }
            //    if (codes[i].labels != null)
            //    {
            //        var labels = codes[i].labels;
            //        for (int k = 0; k < labels.Count; k++)
            //        {
            //            var label2 = new Label();
            //            if (codeOprandLable.Keys.Contains(labels[k]))
            //            {
            //                labels[k] = new Label();
            //                codes[i].labels = labels;
            //            }
            //            labelCode.Add(labels[k], i);
            //        }
            //    }
            //}
            head.AddRange(replacer);
            head.AddRange(tail);
            return head.AsEnumerable();
        }
        public static void TransTypeEmit(this CodeInstruction code, ILGenerator il)
        {
            if (code.operand is byte)
            {
                il.Emit(code.opcode, (byte)code.operand);
            }
            if (code.operand is sbyte)
            {
                il.Emit(code.opcode, (sbyte)code.operand);
            }
            if (code.operand is short)
            {
                il.Emit(code.opcode, (short)code.operand);
            }
            if (code.operand is int)
            {
                il.Emit(code.opcode, (int)code.operand);
            }
            if (code.operand is MethodInfo)
            {
                il.Emit(code.opcode, (MethodInfo)code.operand);
            }
            if (code.operand is SignatureHelper)
            {
                il.Emit(code.opcode, (SignatureHelper)code.operand);
            }
            if (code.operand is ConstructorInfo)
            {
                il.Emit(code.opcode, (ConstructorInfo)code.operand);
            }
            if (code.operand is Type)
            {
                il.Emit(code.opcode, (Type)code.operand);
            }
            if (code.operand is long)
            {
                il.Emit(code.opcode, (long)code.operand);
            }
            if (code.operand is float)
            {
                il.Emit(code.opcode, (float)code.operand);
            }
            if ((code.operand is double))
            {
                il.Emit(code.opcode, (double)code.operand);
            }
            if (code.operand is Label)
            {
                il.Emit(code.opcode, (Label)code.operand);
            }
            if (code.operand is Label[])
            {
                il.Emit(code.opcode, (Label[])code.operand);
            }
            if (code.operand is FieldInfo)
            {
                il.Emit(code.opcode, (FieldInfo)code.operand);
            }
            if (code.operand is LocalBuilder)
            {
                il.Emit(code.opcode, (LocalBuilder)code.operand);
            }
        }
        public static void MethodTranspiler()
        {
        }
    }
}
