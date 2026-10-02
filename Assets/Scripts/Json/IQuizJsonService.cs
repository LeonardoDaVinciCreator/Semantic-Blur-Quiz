using Data;
using UnityEngine;

namespace Json
{
    public interface IQuizJsonService
    {
        void Initialize(TextAsset jsonFile);
        LevelData GetLevelData(int index);
        int GetTotalLevels();
        void ShuffleLevels();
    }
}