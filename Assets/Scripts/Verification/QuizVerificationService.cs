using Data;
using Localization;

public enum VerificationResult
{
    Wrong,       // Красный
    CloseMatch,  // Желтый
    Win          // Зеленый
}

public class QuizVerificationService
{
    public VerificationResult VerifyAnswer(string playerInput, LevelData currentLevel, string tableName)
    {
        if(string.IsNullOrEmpty(playerInput)) return VerificationResult.Wrong;
        
        string lowerInput = playerInput.ToLower().Trim();

        string correctWord = LanguageManager.Instance.GetWordByKey(tableName, currentLevel.correctWordKey).ToLower().Trim();
        if(correctWord == lowerInput) return VerificationResult.Win;

        foreach(var synKey in currentLevel.synonymsKeys)
        {
            string word = LanguageManager.Instance.GetWordByKey(tableName, synKey).ToLower().Trim();
            if(word == lowerInput) return VerificationResult.CloseMatch;
        }

        return VerificationResult.Wrong;
    }
}