using UnityEngine;

namespace Localization
{
    public static class LocalizationHelper
    {
        public static string Get(string tableName, string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            if (LanguageManager.Instance == null) return key;
            return LanguageManager.Instance.GetWordByKey(tableName, key);
        }
    }
}
