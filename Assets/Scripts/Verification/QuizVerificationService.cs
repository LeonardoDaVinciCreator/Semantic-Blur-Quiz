using System.Collections.Generic;
using Data;
using Localization;

public enum VerificationResult
{
    Wrong,       // Красный
    CloseMatch,  // Желтый
    Win,          // Зеленый

    Ignore       // Ничего не делать
}

public class QuizVerificationService
{
    public VerificationResult VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys)
    {
        if(string.IsNullOrEmpty(playerInput)) return VerificationResult.Wrong;
        
        string lowerInput = playerInput.ToLower().Trim();

        string correctWord = LanguageManager.Instance.GetWordByKey(tableName, currentLevel.correctWordKey).ToLower().Trim();
        if(correctWord == lowerInput) return VerificationResult.Win;

        foreach(var synKey in currentLevel.synonymsKeys)
        {
            if (usedSynonymsKeys != null && usedSynonymsKeys.Contains(synKey))
                continue;

            string word = LanguageManager.Instance.GetWordByKey(tableName, synKey).ToLower().Trim();
            if(word == lowerInput)
            {
                usedSynonymsKeys?.Add(synKey); 
                return VerificationResult.CloseMatch;
            }
        }

        //проверка на повтор слов
        foreach(var synKey in currentLevel.synonymsKeys)
        {
            string word = LanguageManager.Instance.GetWordByKey(tableName, synKey).ToLower().Trim();
            if(word == lowerInput && usedSynonymsKeys.Contains(synKey))
            {
                return VerificationResult.Ignore;
            }
        }

        return VerificationResult.Wrong;
    }
}