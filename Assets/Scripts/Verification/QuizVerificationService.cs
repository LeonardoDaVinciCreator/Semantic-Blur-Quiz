using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Localization;
using Services;
using Unity.VisualScripting;
using UnityEngine;

namespace Verification
{
    public class QuizVerificationService : IVerificationService
    {
        private IWordsCache _formsCache;
        
        public QuizVerificationService(IWordsCache formsCache)
        {
            _formsCache = formsCache;
        }

        public async Task<VerificationResult> VerifyAnswer(string playerInput, LevelData currentLevel, string tableName, List<string> usedSynonymsKeys)
        {
            if(string.IsNullOrEmpty(playerInput)) return VerificationResult.Wrong;
            
            string lowerInput = playerInput.ToLower().Trim();

            string[] correctWords = await _formsCache.GetCacheAsync(tableName, currentLevel.correctWordKey);
            
            if(Array.Exists(correctWords, w => w.ToLower().Trim() == lowerInput)) 
                return VerificationResult.Win;        

            foreach(var synKey in currentLevel.synonymsKeys)
            {
                if(currentLevel.synonymsKeys == null)
                {
                    Debug.LogError("currentLevel.synonymsKeys -  пустой!!!");
                    return VerificationResult.Wrong;
                }

                if (usedSynonymsKeys != null && usedSynonymsKeys.Contains(synKey))
                    continue;

                string[] words = await _formsCache.GetCacheAsync(tableName, synKey);
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

                    string[] forms = await _formsCache.GetCacheAsync(tableName, synKey);
                    if (Array.Exists(forms, w => w.ToLower().Trim() == lowerInput))
                    {
                        return VerificationResult.Ignore;
                    }
                }
            }

            return VerificationResult.Wrong;
        }
    }
}