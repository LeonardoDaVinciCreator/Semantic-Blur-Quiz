using UnityEngine;
using Data;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Core
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("настройки тем")]
        [SerializeField] private ThemeConfigSO _themeConfigSO;

        [Header("UI элементы списка")]
        [SerializeField] private Transform _container; // родитель кнопок
        [SerializeField] private ThemeMenuButton _buttonPrefab;

        private List<ThemeMenuButton> _spawnedButtons = new ();

        private void Start()
        {
            BuildMenu();

            if (Localization.LanguageManager.Instance != null)
                Localization.LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
        }

        private void OnDestroy()
        {
            if (Localization.LanguageManager.Instance != null)
                Localization.LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged(string langCode)
        {
            RefreshAllButtonsText();            
        }

        private void RefreshAllButtonsText()
        {
            foreach (var button in _spawnedButtons)
            {
                button.RefreshText();
            }
        }

        public void BuildMenu()
        {
            if (_themeConfigSO == null || _themeConfigSO.Themes == null) return;

            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.GetChild(i).gameObject);
            }
            _spawnedButtons.Clear();

            foreach (var themeData in _themeConfigSO.Themes)
            {
                ThemeMenuButton newButton = Instantiate(_buttonPrefab, _container);
                newButton.Initialize(themeData, OnThemeClicked);
                _spawnedButtons.Add(newButton);
            }

            if (_container is RectTransform rect)
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        private void OnThemeClicked(ThemeItemData selectedTheme)
        {
            if (string.IsNullOrEmpty(selectedTheme.FolderPath) || string.IsNullOrEmpty(selectedTheme.Scene))
            {
                Debug.LogError("[MainMenu] У темы пустой FolderPath или Scene, переход отменён.");
                return;
            }

            Debug.Log($"Выбрана тема: {selectedTheme.FolderPath}");

            GameManager.Instance.Folder = selectedTheme.FolderPath;
            GameManager.Instance.TableName = selectedTheme.GameTableName;

            SceneManager.LoadScene(selectedTheme.Scene);
        }
    }
}