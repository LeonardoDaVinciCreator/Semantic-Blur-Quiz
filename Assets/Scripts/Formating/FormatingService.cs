
namespace Formating
{
    public static class FormatingService
    {
        public static string FormatTime(float seconds, string tableName, string key)
        {
            string text = Localization.LanguageManager.Instance.GetWordByKey(tableName, key);
            return string.Format(text, seconds);
        }

        public static string FormatScore(int score, string tableName, string key)
        {
            string text = Localization.LanguageManager.Instance.GetWordByKey(tableName, key);

            return string.Format(text, score);
        }
    }
}