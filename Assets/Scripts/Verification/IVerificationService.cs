using System.Collections.Generic;
using Data;
using Localization;
using Services;
using Unity.VisualScripting;
using UnityEngine;

namespace Verification
{
    public enum VerificationResult
    {
        Wrong,
        CloseMatch,
        Win,
        Ignore
    }

    public interface IVerificationService
    {
        VerificationResult VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys);
    }
}