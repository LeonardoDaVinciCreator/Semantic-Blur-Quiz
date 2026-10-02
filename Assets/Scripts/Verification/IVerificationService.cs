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

public class IVerificationService
{
    public VerificationResult VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys);
}