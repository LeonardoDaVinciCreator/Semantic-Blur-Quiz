using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;
using System;

using Localization;

namespace Core
{
    public class ThemeMenuButton : MonoBehaviour
    {        
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;

        private ThemeItemData _data;
        private Action<ThemeItemData> _onThemeSelected;

        public void Initialize(ThemeItemData data, Action<ThemeItemData> onThemeSelected)
        {
            _data = data;
            _onThemeSelected = onThemeSelected;

            _nameText.text = LocalizationHelper.Get(_data.TableName, _data.ThemeNameKey);
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(SelectTheme);
        }

        private void SelectTheme()
        {
            _onThemeSelected?.Invoke(_data);
        }

        public void RefreshText()
        {
            if (_data != null)
            {
                _nameText.text = LocalizationHelper.Get(_data.TableName, _data.ThemeNameKey);
            }
        }
    }
}