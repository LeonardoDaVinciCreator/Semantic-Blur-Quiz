
using System.Collections.Generic;
using System.Threading.Tasks;
using Localization;

namespace Verification
{
    public class WordsCache : IWordsCache
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

        public async Task<string[]> GetCacheAsync(string tableName, string key)
        {
            string cacheKey = $"{tableName}_{key}";

            if (!_cache.ContainsKey(cacheKey))
            {
                _cache[cacheKey] = await LanguageManager.Instance.GetWordsByKeyAsync(tableName, key);
            }
            
            return _cache[cacheKey];
        }

        public void Clear() => _cache.Clear();

        private void OnLanguageChanged(string langCode)
        {
            Clear();
        }
    }
}