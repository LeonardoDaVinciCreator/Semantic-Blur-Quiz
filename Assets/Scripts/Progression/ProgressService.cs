using System.Collections.Generic;
using UnityEngine;

namespace Progression
{
    public class QuizProgressService : IQuizProgressService
    {
        private const string KeyPrefix = "Guessed_";

        public bool IsGuessed(string themeId, int levelIndex)
        {
            return GetGuessed(themeId).Contains(levelIndex);
        }

        public void MarkGuessed(string themeId, int levelIndex)
        {
            var set = GetGuessed(themeId);
            
            if (!set.Add(levelIndex)) return;
            Save(themeId, set);
        }

        public HashSet<int> GetGuessed(string themeId)
        {
            var result = new HashSet<int>();
            if (string.IsNullOrEmpty(themeId)) return result;

            string raw = PlayerPrefs.GetString(KeyPrefix + themeId, "");
            if (string.IsNullOrEmpty(raw)) return result;

            foreach (var part in raw.Split(','))
            {
                if (int.TryParse(part, out int index))
                    result.Add(index);
            }
            return result;
        }

        public int GetGuessedCount(string themeId)
        {
            return GetGuessed(themeId).Count;
        }

        public void ResetTheme(string themeId)
        {
            if (string.IsNullOrEmpty(themeId)) return;
            PlayerPrefs.DeleteKey(KeyPrefix + themeId);
            PlayerPrefs.Save();
        }

        public void ResetAll(string[] themeIds)
        {
            if (themeIds == null) return;

            foreach (var themeId in themeIds)
            {
                if (!string.IsNullOrEmpty(themeId))
                    PlayerPrefs.DeleteKey(KeyPrefix + themeId);
            }
            PlayerPrefs.Save();
        }

        private void Save(string themeId, HashSet<int> set)
        {
            PlayerPrefs.SetString(KeyPrefix + themeId, string.Join(",", set));
            PlayerPrefs.Save();
        }
    }
}