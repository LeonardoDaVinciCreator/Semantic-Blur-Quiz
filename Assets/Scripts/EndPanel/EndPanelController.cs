using Data;
using Localization;
using UnityEngine;

namespace EndPanel
{
    public class EndPanelController : MonoBehaviour
    {
        [SerializeField] private EndPanelView _view;
        [SerializeField] private UIConfigSO _maskConfig;

        private IEndPanel _model;

        private float _time;
        private int _correct;
        private int _total;
        private bool _isTimeOut;
        private bool _hasResult;

        private void Awake()
        {
            _model = new EndPanelModel();
            _view.Hide();
        }

        public void ShowResult(float time, int correct, int total, bool isTimeOut)
        {
            if (_model == null) _model = new EndPanelModel();

            _time = time;
            _correct = correct;
            _total = total;
            _isTimeOut = isTimeOut;
            _hasResult = true;

            _model.Save(time, correct);

            RefreshDisplay();
        }

        public void HideResult()
        {
            _hasResult = false;
            _view.Hide();
        }

        public void RefreshDisplay()
        {
            if(!_hasResult) return;

            var data = BuildData();
            _view.Show(data);
        }

        public void SetVisible(bool visible)
        {
            if (visible)
            {
                if (_hasResult) _view.Show(BuildData());
            }
            else
            {
                _view.Hide();
            }
        }

        public EndPanelData BuildData()
        {
            string bestTimeText = _model.TryGetBestTime(out var bestTime)
                ? string.Format(
                    GetLocalizedString(_maskConfig.EndPanel.BestTimeTextKey),
                    bestTime
                )
                : "-";
            
            string bestScoreText = _model.TryGetBestScore(out var bestScore)
                ? string.Format(
                    GetLocalizedString(_maskConfig.EndPanel.BestScoreTextKey),
                    bestScore
                )
                : "-";

            return new EndPanelData
            {
                Title = GetLocalizedString(_isTimeOut ? _maskConfig.EndPanel.TitlegGameOverKey : _maskConfig.EndPanel.TitleCompleteKey),

                TimeLabel = GetLocalizedString(_maskConfig.EndPanel.TimeLabelKey),
                TimeValue = string.Format(GetLocalizedString(_maskConfig.EndPanel.TimeTextKey), _time),

                ScoreLabel = GetLocalizedString(_maskConfig.EndPanel.ScoreLabelKey),
                ScoreValue = string.Format(GetLocalizedString(_maskConfig.EndPanel.ScoreTextKey), _correct, _total),

                BestTimeLabel = GetLocalizedString(_maskConfig.EndPanel.BestTimeLabelKey),
                BestTimeValue = bestTimeText,

                BestScoreLabel = GetLocalizedString(_maskConfig.EndPanel.BestScoreLabelKey),
                BestScoreValue = bestScoreText,
            };
        }

        private string GetLocalizedString(string key)
        {
            if(string.IsNullOrEmpty(key)) return string.Empty;

            return LocalizationHelper.Get(_maskConfig.EndPanel.NameTable, key);
        }
    }
}