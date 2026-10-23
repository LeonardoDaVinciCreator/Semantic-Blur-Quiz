using UnityEngine;
using System.Threading.Tasks;

namespace Localization
{
    public static class LocalizationHelper
    {
        public static async Task<string> GetAsync(string tableName, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (LanguageManager.Instance == null) return key;

            return await LanguageManager.Instance.GetWordByKeyAsync(tableName, key);
        }
    }
}
