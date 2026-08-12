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
        public event Action<string> OnLanguageChanged;

        [Header("Доступные языки")]
        [SerializeField] private List<Locale> _supportedLocales;
        [SerializeField] private string _currentLanguage = "ru";

        [Header("Разделитель")] 
        [SerializeField] private string _separator = "|";
        public string CurrentLanguage => _currentLanguage;

        protected override void Awake()
        {
            base.Awake();

            if (PlayerPrefs.HasKey("Language"))
                _currentLanguage = PlayerPrefs.GetString("Language");

            SetLanguageByCode(_currentLanguage);
        }

        private void OnEnable()
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        private void OnDisable()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        private void OnSelectedLocaleChanged(Locale newLocale)
        {
            string code = newLocale.Identifier.Code;
            _currentLanguage = code;
            PlayerPrefs.SetString("Language", code);
            PlayerPrefs.Save();
            OnLanguageChanged?.Invoke(code);
        }

        public string GetWordByKey(string tableName, string key)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
        }

        public string[] GetWordsByKey(string tableName, string key)
        {
            string rowWords = GetWordByKey(tableName, key);

            if (string.IsNullOrEmpty(rowWords)) 
                return new string[] {""};
            
            return rowWords.Split(_separator);
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

            OnLanguageChanged?.Invoke(langCode);
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