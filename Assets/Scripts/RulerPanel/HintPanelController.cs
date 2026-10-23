using System;
using System.Threading.Tasks;
using Data;
using Localization;
using UnityEngine;

namespace RulerPanel
{
    public class RulerPanelController : MonoBehaviour
    {       
        public event Action OnClosed;

        [SerializeField] private RulerPanelView _view;
        [SerializeField] private UIConfigSO _uiConfig;

        private IRulerPanel _model;

        private void Awake()
        {
            _model = new RulerPanelModel();
            _model.OnClosed += HandleModelClosed;
            _view.Hide();
        }

        public async Task Show()
        {
            _model.Open();
            _view.SetCloseListener(Close);
            
            if (_uiConfig != null)
            {
                string hintText = await GetLocalizedString(_uiConfig.Game.HintTextKey);
                string buttonText = await GetLocalizedString(_uiConfig.Game.HintButtonKey);
                _view.Show(hintText, buttonText);
            }
        }

        private void Close()
        {
            _model.Close();
        }

        private void HandleModelClosed()
        {
            _view.Hide();
            OnClosed?.Invoke();
        }

        public async Task RefreshLocalization()
        {
            if (_uiConfig == null) return;
            
            string hintText = await GetLocalizedString(_uiConfig.Game.HintTextKey);
            string buttonText = await GetLocalizedString(_uiConfig.Game.HintButtonKey);

            _view.SetLocalizedTexts(hintText, buttonText);
        }

        private async Task<string> GetLocalizedString(string key)
        {
            if(string.IsNullOrEmpty(key)) return string.Empty;
            return await LocalizationHelper.GetAsync(_uiConfig.EndPanel.NameTable, key);
        }
    }
}