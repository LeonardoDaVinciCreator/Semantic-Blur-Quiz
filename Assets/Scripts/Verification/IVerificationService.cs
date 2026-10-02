using System.Collections.Generic;
using Data;
using Localization;
using Unity.VisualScripting;
using UnityEngine;

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