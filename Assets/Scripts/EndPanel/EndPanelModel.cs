
using UnityEngine;

namespace EndPanel
{
    public class EndPanelData
    {
        public string Title;
        public string TimeLabel, TimeValue;
        public string ScoreLabel, ScoreValue;
        public string BestTimeLabel, BestTimeValue;
        public string BestScoreLabel, BestScoreValue;
    }

    public class EndPanelModel : IEndPanel
    {
        private const string BestTimeKey = "BestTime";
        private const string BestScoreKey = "BestScore";

        public void Save(float time, int score)
        {
            if (!TryGetBestTime(out var bestTime) || time < bestTime)
                PlayerPrefs.SetFloat(BestTimeKey, time);

            if (!TryGetBestScore(out var bestScore) || score > bestScore)
                PlayerPrefs.SetInt(BestScoreKey, score);

            PlayerPrefs.Save();
        }

        public bool TryGetBestTime(out float time)
        {
            if (!PlayerPrefs.HasKey(BestTimeKey)) { time = 0f; return false; }
            time = PlayerPrefs.GetFloat(BestTimeKey);
            return true;
        }

        public bool TryGetBestScore(out int score)
        {
            if (!PlayerPrefs.HasKey(BestScoreKey)) { score = 0; return false; }
            score = PlayerPrefs.GetInt(BestScoreKey);
            return true;
        }
    }
}