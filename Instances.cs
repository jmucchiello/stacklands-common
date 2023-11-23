using System.Reflection;
using System.Diagnostics;

namespace CommonModNS
{
    public static class I
    {
        public static WorldManager WM => WorldManager.instance;
        public static WorldManager.GameState GameState => WM.CurrentGameState;
        public static RunVariables CRV => WM.CurrentRunVariables;
        public static GameDataLoader GDL => WM.GameDataLoader;
        public static PrefabManager PFM => PrefabManager.instance;
        public static GameScreen GS => GameScreen.instance;
        public static ModOptionsScreen MOS => ModOptionsScreen.instance;
        public static ModalScreen Modal => ModalScreen.instance;
        public static void Log(string msg)
        {
            try
            {
                log?.Invoke(null, new object[] { msg });
            }
            catch (Exception) { }
        }
        public static string Xlat(string termId, params LocParam[] terms)
        {
            if (termId == null)
            {
                Log("XLAT TermId is null");
            }
            else
            {
                string xlat = terms != null && terms.Length > 0 ? SokLoc.Translate(termId, terms) : SokLoc.Translate(termId);
                if (xlat != "---MISSING---")
                {
                    return xlat; // success
                }
                Log($"XLAT {termId} {xlat}");
            }
            StackTrace trace = new StackTrace();
            if (trace != null) Log($"{trace.GetFrame(1)?.GetMethod()}/{trace.GetFrame(2)?.GetMethod()}/{trace.GetFrame(3)?.GetMethod()}");
            return null;
        }

        public static LocParam Param(string termId, string value)
        {
            return LocParam.Create(termId, value);
        }

        private static MethodInfo log;

        /**
         * If you declare this in your Mod class:
         *      public static void Log(string msg) => Instance?.Logger.Log(msg);
         * This code will find it and make I.Log call your Log function. And then you can copy this file
         * into any mod and the I.Log function will automatically work without you having to rename how you find the instance to the mod.
         */
        static I()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type myMod = assembly.ExportedTypes.First(x => typeof(Mod).IsAssignableFrom(x));
            log = myMod.GetMethod("Log");
            if (!log.IsStatic) log = null;
        }
    }
}
