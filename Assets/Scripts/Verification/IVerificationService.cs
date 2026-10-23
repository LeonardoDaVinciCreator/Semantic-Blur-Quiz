using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<VerificationResult> VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys);
    }
}