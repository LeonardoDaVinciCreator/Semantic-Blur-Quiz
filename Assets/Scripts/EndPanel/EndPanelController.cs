using System.Threading.Tasks;
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

        public async void RefreshDisplay()
        {
            if(!_hasResult) return;

            var data = await BuildData();
            _view.Show(data);
        }

        public async Task SetVisible(bool visible)
        {
            if (visible)
            {
                if (_hasResult) _view.Show(await BuildData());
            }
            else
            {
                _view.Hide();
            }
        }

        public async Task<EndPanelData> BuildData()
        {
            string bestTimeText = _model.TryGetBestTime(out var bestTime)
                ? string.Format(
                    await GetLocalizedString(_maskConfig.EndPanel.BestTimeTextKey),
                    bestTime
                )
                : "-";
            
            string bestScoreText = _model.TryGetBestScore(out var bestScore)
                ? string.Format(
                    await GetLocalizedString(_maskConfig.EndPanel.BestScoreTextKey),
                    bestScore
                )
                : "-";

            return new EndPanelData
            {
                Title = await GetLocalizedString(_isTimeOut ? _maskConfig.EndPanel.TitlegGameOverKey : _maskConfig.EndPanel.TitleCompleteKey),

                TimeLabel = await GetLocalizedString(_maskConfig.EndPanel.TimeLabelKey),
                TimeValue = string.Format(await GetLocalizedString(_maskConfig.EndPanel.TimeTextKey), _time),

                ScoreLabel = await GetLocalizedString(_maskConfig.EndPanel.ScoreLabelKey),
                ScoreValue = string.Format(await GetLocalizedString(_maskConfig.EndPanel.ScoreTextKey), _correct, _total),

                BestTimeLabel = await GetLocalizedString(_maskConfig.EndPanel.BestTimeLabelKey),
                BestTimeValue = bestTimeText,

                BestScoreLabel = await GetLocalizedString(_maskConfig.EndPanel.BestScoreLabelKey),
                BestScoreValue = bestScoreText,
            };
        }

        private async Task<string> GetLocalizedString(string key)
        {
            if(string.IsNullOrEmpty(key)) return string.Empty;

            return await LocalizationHelper.GetAsync(_maskConfig.EndPanel.NameTable, key);
        }
    }
}