using Localization;
using System.Threading.Tasks;

namespace Formating
{
    public static class FormatingService
    {
        public static async Task<string> FormatTime(float seconds, string tableName, string key)
        {
            string formatString = await LocalizationHelper.GetAsync(tableName, key);
            return string.Format(formatString, seconds);
        }

        public static async Task<string> FormatScore(int score, string tableName, string key)
        {
            string formatString = await LocalizationHelper.GetAsync(tableName, key);
            return string.Format(formatString, score);
        }
    }
}