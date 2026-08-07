
using System.Collections.Generic;
using Localization;

public class WordsCache
{
    private Dictionary<string, string[]> _cache = new ();

    public WordsCache()
    {
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    public void Dispose()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    public string[] GetCache(string tableName, string key)
    {
        string cacheKey = $"{tableName}_{key}";

        if (!_cache.ContainsKey(cacheKey))
        {
            _cache[cacheKey] = LanguageManager.Instance.GetWordsByKey(tableName, key);
        }
        
        return _cache[cacheKey];
    }

    public void Clear() => _cache.Clear();

    private void OnLanguageChanged(string langCode)
    {
        Clear();
    }
}