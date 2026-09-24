using TMPro;
using UnityEngine;

namespace EndPanel
{
    public class EndPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _timeLabel, _timeValue;
        [SerializeField] private TMP_Text _scoreLabel, _scoreValue;
        [SerializeField] private TMP_Text _bestTimeLabel, _bestTimeValue;
        [SerializeField] private TMP_Text _bestScoreLabel, _bestScoreValue;

        public void Show(EndPanelData data)
        {            
            _title.text = data.Title;
            
            _timeLabel.text = data.TimeLabel;
            _timeValue.text = data.TimeValue;

            _scoreLabel.text = data.ScoreLabel;
            _scoreValue.text = data.ScoreValue;

            _bestTimeLabel.text = data.BestTimeLabel;
            _bestTimeValue.text = data.BestTimeValue;

            _bestScoreLabel.text = data.BestScoreLabel;
            _bestScoreValue.text = data.BestScoreValue;

            _root.SetActive(true);
        }
        public void Hide()
        {
            _root.SetActive(false);
        }
    }
}