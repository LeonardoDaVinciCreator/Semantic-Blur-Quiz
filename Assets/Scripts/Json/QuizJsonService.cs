
using Data;
using UnityEngine;

namespace Json
{
    public class QuizJsonService : IQuizJsonService
    {
        private LevelList _gameData;

        private static readonly System.Random _rng = new System.Random();

        public void Initialize(TextAsset jsonFile)
        {
            if (jsonFile != null)
            {
                _gameData = JsonUtility.FromJson<LevelList>(jsonFile.text);
            }
            else
            {
                Debug.LogWarning("[QuizJsonService] JSON-файл не найден!");
            }
        }

        public LevelData GetLevelData(int index)
        {
            if(_gameData == null || _gameData.levels == null || index < 0 || index >= _gameData.levels.Count) return null;

            return _gameData.levels[index];
        }

        public int GetTotalLevels() => _gameData?.levels.Count ?? 0;

        public void ShuffleLevels()
        {
            if (_gameData?.levels == null) return;
            int n = _gameData.levels.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                var temp = _gameData.levels[k];
                _gameData.levels[k] = _gameData.levels[n];
                _gameData.levels[n] = temp;
            }
        }
    }
}