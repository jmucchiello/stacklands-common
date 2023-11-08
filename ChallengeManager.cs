using System;
using System.Collections.Generic;
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

    }
}
