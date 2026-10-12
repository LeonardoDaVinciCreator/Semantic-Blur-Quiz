
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Json
{
    public class QuizJsonService : IQuizJsonService
    {
        private LevelList _gameData;
        private List<LevelData> _originalLevels;
        private List<LevelData> _shuffledLevels;

        private static readonly System.Random _rng = new System.Random();

        public void Initialize(TextAsset jsonFile)
        {
            if (jsonFile == null)
            {
                Debug.LogWarning("[QuizJsonService] JSON-файл не найден!");
                return;
            }

            _gameData = JsonUtility.FromJson<LevelList>(jsonFile.text);

            if (_gameData?.levels == null)
            {
                Debug.LogWarning("[QuizJsonService] JSON-файл не найден!");
                return;
            }

            _originalLevels = new List<LevelData>(_gameData.levels);
            _shuffledLevels = new List<LevelData>(_gameData.levels);
        }

        public LevelData GetLevelData(int index)
        {
            if (_shuffledLevels == null || index < 0 || index >= _shuffledLevels.Count)
                return null;

            return _shuffledLevels[index];
        }

        public int GetTotalLevels() => _shuffledLevels?.Count ?? 0;

        public void ShuffleLevels()
        {
            if (_shuffledLevels == null) return;

            int n = _shuffledLevels.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                var temp = _shuffledLevels[k];
                _shuffledLevels[k] = _shuffledLevels[n];
                _shuffledLevels[n] = temp;
            }
        }

        public IReadOnlyList<LevelData> GetAllLevels() => _originalLevels;
    }
}