using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CommonModNS
{
    public class ChallengeManager
    {
        readonly List<IChallengeMod> mods = new();

        private static readonly string saveRoundKey = "challengemanager";
        private static readonly string oldSaveRoundKey = null;

        private static readonly string salt = Environment.MachineName + "?challengemanager";

        void save(SaveRound saveRound)
        {
            Dictionary<string, string> dict = new();
            foreach (var mod in mods)
            {
                dict.Add(mod.Challenge_ModId, mod.Challenge_SaveData_Immutable);
            }
        }


        private static List<Mod> FindMods()
        {
            List<Mod> mods = new();
            foreach (Mod mod in ModManager.LoadedMods)
            {
                Type type = mod.GetType();
                MethodInfo mi = AccessTools.Method(type, "ChallengeInit");
                if (mi == null)
                {
                    I.Log($"{mod.name} does not implement ChallengeInit");
                    continue;
                }
                try
                {
                    object ret = mi.Invoke(null, new object[0]);
                    if (ret == null) continue;
                    if (!(bool)ret) continue;

                    mods.Add(mod);
                }
                catch (Exception ex)
                {
                    I.Log($"Invoking {mod.name}.ChallengeInit() threw {ex.Message}");
                }
            }
            return mods;
        }

        private static void Invoke(string function, params object[] args)
        {
            foreach (Mod mod in FindMods())
            {
                Type type = mod.GetType();
                MethodInfo mi = AccessTools.Method(type, function);
                if (mi == null)
                {
                    I.Log($"{mod.name} does not implement {function}");
                    continue;
                }
                try
                {
                    mi.Invoke(null, args);
                }
                catch (Exception ex)
                {
                    I.Log($"Invoking {mod.name}.{function}() threw {ex.Message}\n{ex.StackTrace}");
                }
            }
        }
    }
}
