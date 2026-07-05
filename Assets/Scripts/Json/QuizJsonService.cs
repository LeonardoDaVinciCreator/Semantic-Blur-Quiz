
using Data;
using UnityEngine;

namespace Json
{
    public class QuizJsonService
    {
        private LevelList _gameData;

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
    }
}