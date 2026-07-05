using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Localization
{
    public class LanguageManager: ManagerBase<LanguageManager>
    {
        [Header("Доступные языки")]
        [SerializeField] private List<Locale> _supportedLocales;
        [SerializeField] private string _currentLanguage = "ru";
        public string CurrentLanguage => _currentLanguage;

        protected override void Awake()
        {
            base.Awake();

            if (PlayerPrefs.HasKey("Language"))
                _currentLanguage = PlayerPrefs.GetString("Language");
        }

        public string GetWordByKey(string tableName, string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
        }

        private void LoadLanguage()
        {
            if (PlayerPrefs.HasKey("Language"))
            {
                _currentLanguage = PlayerPrefs.GetString("Language");
                SetLanguageByCode(_currentLanguage);
            }
        }

        public void SetLanguageByCode(string langCode)
        {
            _currentLanguage = langCode;
            
            var locale = LocalizationSettings.AvailableLocales.GetLocale(langCode);
            if (locale != null)
            {
                LocalizationSettings.SelectedLocale = locale;
            }
            
            PlayerPrefs.SetString("Language", langCode);
            PlayerPrefs.Save();
        }
        
        public void SetLanguageByIndex(int index)
        {
            if (index >= 0 && index < LocalizationSettings.AvailableLocales.Locales.Count)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[index];
                SetLanguageByCode(locale.Identifier.Code);
            }
        }
    }
}