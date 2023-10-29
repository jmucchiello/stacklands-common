using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CommonModNS
{
    public static class WorldManagerPatches
    {
        public static Action<WorldManager, SaveRound> GetSaveRound;     // called from SaveManager.Save before a game is saved
        public static Action<WorldManager, SaveRound> LoadSaveRound;    // called from MainMenu.ContinueButton.Clicked after a game is loaded
        public static Action<WorldManager> StartNewRound;               // called from RunOptionsScreen.PlayButton.Clicked after a new game is started
        public static Action<WorldManager> Update;                      // called by UnityEngine.MonoBehaviour every "clock" tick
        public static Action<WorldManager> Play;                        // called from (ContinueButton or PlayButton).Clicked when a game is started the menu

        public static void ApplyPatches(Harmony Harmony)
        {
            try
            {
                List<PatchHelper> list = new List<PatchHelper>();
                if (GetSaveRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper() { targetType = typeof(WorldManager), targetMethod = "GetSaveRound", patchMethod = "WorldManager_GetSaveRound" });
                if (LoadSaveRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper() { targetType = typeof(WorldManager), targetMethod = "LoadSaveRound", patchMethod = "WorldManager_LoadSaveRound" });
                if (StartNewRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper() { targetType = typeof(WorldManager), targetMethod = "StartNewRound", patchMethod = "WorldManager_StartNewRound" });
                if (Update?.GetInvocationList().Length > 0) list.Add(new PatchHelper() { targetType = typeof(WorldManager), targetMethod = "Update", patchMethod = "WorldManager_Update" });
                if (Play?.GetInvocationList().Length > 0) list.Add(new PatchHelper() { targetType = typeof(WorldManager), targetMethod = "Play", patchMethod = "WorldManager_Play" });

                foreach (PatchHelper patch in list)
                {
                    patch.Patch(Harmony);
                }
            }
            catch (Exception ex)
            {
                I.Log("Exception caught in WorldManagerPatches.ApplyPatches: " + ex.Message);
            }
        }

        internal class PatchHelper
        {
            public Type targetType;
            public string targetMethod;
            public string patchMethod;
            private bool patched;

            public void Patch(Harmony harmony)
            {
                if (!patched)
                {
                    MethodInfo miMine = AccessTools.Method(typeof(WorldManagerPatches), patchMethod);
                    MethodInfo miWM = AccessTools.Method(targetType, targetMethod);
                    var x = harmony.Patch(original: miWM, postfix: new HarmonyMethod(miMine));
                    I.Log($"Patching {targetType.Name}.{targetMethod} {(x != null ? "successed" : "failed")}");
                    patched = true;
                }
            }
        }

        //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.GetSaveRound))]
        //[HarmonyPostfix]
        static void WorldManager_GetSaveRound(WorldManager __instance, ref SaveRound __result)
        {
            GetSaveRound?.Invoke(__instance, __result);
        }

        //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.LoadSaveRound))]
        //[HarmonyPostfix]
        static void WorldManager_LoadSaveRound(WorldManager __instance, SaveRound saveRound)
        {
            LoadSaveRound?.Invoke(__instance, saveRound);
        }

        //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.LoadSaveRound))]
        //[HarmonyPostfix]
        static void WorldManager_StartNewRound(WorldManager __instance)
        {
            StartNewRound?.Invoke(__instance);
        }

        //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Update))]
        //[HarmonyPostfix]
        static void WorldManager_Update(WorldManager __instance)
        {
            Update?.Invoke(__instance);
        }

        //[HarmonyPatch(typeof(WorldManager), nameof(WorldManager.Play))]
        //[HarmonyPostfix]
        static void WorldManager_Play(WorldManager __instance)
        {
            Play?.Invoke(__instance);
        }
    }
}
