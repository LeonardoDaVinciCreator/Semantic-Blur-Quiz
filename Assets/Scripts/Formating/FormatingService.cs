using Localization;

namespace Formating
{
    public static class FormatingService
    {
        public static string FormatTime(float seconds, string tableName, string key)
        {
            return string.Format(LocalizationHelper.Get(tableName, key), seconds);
        }

        public static string FormatScore(int score, string tableName, string key)
        {
            return string.Format(LocalizationHelper.Get(tableName, key), score);
        }
    }
}