using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// build action is set to NONE

namespace CommonModNS
{
    internal class coolstuff
    {
    }

    [HarmonyPatch(typeof(WorldManager), "StartNewRound")]
    public class FixBoosterPackExploit
    {
        static void Postfix(WorldManager __instance)
        {
            foreach (BuyBoosterBox bbb in __instance.AllBoosterBoxes)
            {
                bbb.StoredCostAmount = 0;
            }
        }
    }

    public class StandardMod<T> : Mod where T : Mod
    {
        public static T instance { get; protected set; }
        public static void Log(string msg) => GetModLogger()?.Log(msg);
        public static void LogError(string msg) => GetModLogger()?.LogError(msg);

        private static ModLogger modLoggerCache = null;
        private static ModLogger GetModLogger()
        {
            if (instance == null) return null;
            if (modLoggerCache == null)
            {
                FieldInfo ModLoggerFI = AccessTools.Field(typeof(Mod), "Logger");
                modLoggerCache = ModLoggerFI.GetValue(instance) as ModLogger;
            }
            return modLoggerCache;
        }
    }
}
