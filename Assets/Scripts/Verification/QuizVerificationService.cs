using System;
using System.Collections.Generic;
using Data;
using Localization;
using Unity.VisualScripting;

public enum VerificationResult
{
    Wrong,       // Красный
    CloseMatch,  // Желтый
    Win,          // Зеленый

    Ignore       // Ничего не делать
}

public class QuizVerificationService
{
    private WordsCache _formsCache;
    
    public QuizVerificationService(WordsCache formsCache)
    {
        _formsCache = formsCache;
    }

    public VerificationResult VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys)
    {
        if(string.IsNullOrEmpty(playerInput)) return VerificationResult.Wrong;
        
        string lowerInput = playerInput.ToLower().Trim();

        string[] correctWords = _formsCache.GetCache(tableName, currentLevel.correctWordKey);
        
        if(Array.Exists(correctWords, w => w.ToLower().Trim() == lowerInput)) 
            return VerificationResult.Win;        

        foreach(var synKey in currentLevel.synonymsKeys)
        {
            if (usedSynonymsKeys != null && usedSynonymsKeys.Contains(synKey))
                continue;

            string[] words = _formsCache.GetCache(tableName, synKey);
            if(Array.Exists(words, w => w.ToLower().Trim() == lowerInput))
            {
                usedSynonymsKeys?.Add(synKey); 
                return VerificationResult.CloseMatch;
            }
        }

        //проверка на повтор слов
        if (usedSynonymsKeys != null)
        {
            foreach (var synKey in currentLevel.synonymsKeys)
            {
                if (!usedSynonymsKeys.Contains(synKey))
                    continue;

                string[] forms = _formsCache.GetCache(tableName, synKey);
                if (Array.Exists(forms, w => w.ToLower().Trim() == lowerInput))
                {
                    return VerificationResult.Ignore;
                }
            }
        }

        return VerificationResult.Wrong;
    }
}