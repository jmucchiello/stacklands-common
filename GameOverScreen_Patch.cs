using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace CommonModNS
{
    [HarmonyPatch]
    public class GameOverScreen_Patch
    {
        public static void AddListener(Func<string> listener)
        {
            listeners.Add(listener);
        }

        private static List<Func<string>> listeners = new List<Func<string>>();

        /// <summary>
        /// Called from the Transpiler code to add text to the Game Over Screen.
        /// </summary>
        /// <returns></returns>
        private static string GetGameOverText()
        {
            string text = "";
            foreach (var listener in listeners) 
            {
                string next = listener.Invoke();
                if (String.IsNullOrEmpty(next)) continue;
                text += next;
                if (!text.EndsWith("\n")) text += "\n";
            }
            return text;
        }

        /// <summary>
        /// 
        /// 	IL_00b4: ldloc.0
        /// 	IL_00b5: call string CommonModNS.GameOverScreen_Patch::GetGameOverText()
        /// 	IL_00ba: call string[netstandard] System.String::Concat(string, string)
        /// 	IL_00bf: stloc.0
        /// 	
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>

        [HarmonyPatch(typeof(GameOverScreen),"Update")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                Type myClass = typeof(GameOverScreen_Patch);
                FieldInfo fiStatsText = AccessTools.Field(typeof(GameOverScreen), "StatsText");
                MethodInfo miGetGameOverText = AccessTools.Method(myClass, "GetGameOverText");
                MethodInfo miStringConcat = AccessTools.Method(typeof(System.String), "Concat", [typeof(string), typeof(string)]);

                List<CodeInstruction> result = new CodeMatcher(instructions)
                    .MatchStartForward(
                        new CodeMatch(OpCodes.Ldarg_0),
                        new CodeMatch(OpCodes.Ldfld, fiStatsText)
                    )
                    .Insert(
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Call, miGetGameOverText),
                        new CodeInstruction(OpCodes.Call, miStringConcat),
                        new CodeInstruction(OpCodes.Stloc_0)
                    )
                    .InstructionEnumeration()
                    .ToList();
                //result.ForEach(instruction => SpawnControlMod.Log($"{instruction}"));
                I.Log($"Exiting Instructions in {instructions.Count()}, instructions out {result.Count()}");
                return result;
            }
            catch (Exception e)
            {
                I.Log("Failed to Transpile GameOverScreen.Update" + e.ToString());
                return instructions;
            }
        }
    }
}
