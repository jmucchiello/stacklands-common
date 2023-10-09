using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CommonModNS
{
    [HarmonyPatch]
    public class LoadSave
    {
        private static Action<SaveRound> saveConfig;
        private static Action<SaveRound> loadConfig;

        public static void Set(Action<SaveRound> saveConfig_, Action<SaveRound> loadConfig_)
        {
            MethodBase mb = new StackFrame(1).GetMethod();
            I.Log($"LoadSave.Set called from {mb.FullDescription()}");
            saveConfig += saveConfig_;
            loadConfig += loadConfig_;
        }

        public static void Unset(Action<SaveRound> saveConfig_, Action<SaveRound> loadConfig_)
        {
            saveConfig -= saveConfig_;
            loadConfig -= loadConfig_;
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.GetSaveRound))]
        [HarmonyPostfix]
        static void WorldManager_GetSaveRound(WorldManager __instance, ref SaveRound __result)
        {
            saveConfig?.Invoke(__result);
        }

        [HarmonyPatch(typeof(WorldManager), nameof(WorldManager.LoadSaveRound))]
        [HarmonyPostfix]
        static void WorldManager_LoadSaveRound(WorldManager __instance, SaveRound saveRound)
        {
            loadConfig?.Invoke(saveRound);
        }
    }
}
