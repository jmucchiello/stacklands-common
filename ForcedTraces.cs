using System;
using System.Collections.Generic;
using System.Text;

namespace CommonModNS
{
    public static class ForcedTraces
    {
        private static List<string> strings = new List<string>();
        static ForcedTraces()
        {
            WorldManagerPatches.Update += delegate { Export(); };
        }

        public static void Export()
        {
            foreach (string s in strings)
            {
                I.Log(s);
            }
            strings.Clear();
        }

        public static void Trace(string s)
        { 
            strings.Add(s); 
        }
    }
}
