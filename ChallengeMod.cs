using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CommonModNS
{
    public enum ChallengeStatus { ENABLED, DISABLED, BROKEN }
    public interface IChallengeMod
    {
        string Challenge_ModId { get; }
        string Challenge_SaveData_Immutable { get; }

        void Challenge_OnPlay(ChallengeStatus status);

        void Challenge_OnUI(Transform parent);
        void Challenge_OnUISave();
    }
}
