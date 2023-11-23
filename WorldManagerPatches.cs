using HarmonyLib;
using System.Reflection;

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
                if (GetSaveRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper(typeof(WorldManager), "GetSaveRound", "WorldManager_GetSaveRound"));
                if (LoadSaveRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper(typeof(WorldManager), "LoadSaveRound", "WorldManager_LoadSaveRound"));
                if (StartNewRound?.GetInvocationList().Length > 0) list.Add(new PatchHelper(typeof(WorldManager), "StartNewRound", "WorldManager_StartNewRound"));
                if (Update?.GetInvocationList().Length > 0) list.Add(new PatchHelper(typeof(WorldManager), "Update", "WorldManager_Update"));
                if (Play?.GetInvocationList().Length > 0) list.Add(new PatchHelper(typeof(WorldManager), "Play", "WorldManager_Play"));

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

        internal class PatchHelper(Type type, string target, string patch)
        {
            public readonly Type targetType = type;
            public readonly string targetMethod = target;
            public readonly string patchMethod = patch;
            private bool patched = false;

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

        static void WorldManager_GetSaveRound(WorldManager __instance, ref SaveRound __result)
        {
            GetSaveRound?.Invoke(__instance, __result);
        }

        static void WorldManager_LoadSaveRound(WorldManager __instance, SaveRound saveRound)
        {
            LoadSaveRound?.Invoke(__instance, saveRound);
        }

        static void WorldManager_StartNewRound(WorldManager __instance)
        {
            StartNewRound?.Invoke(__instance);
        }

        static void WorldManager_Update(WorldManager __instance)
        {
            Update?.Invoke(__instance);
        }

        static void WorldManager_Play(WorldManager __instance)
        {
            Play?.Invoke(__instance);
        }
    }
}
