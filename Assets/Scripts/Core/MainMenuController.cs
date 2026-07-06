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
        }

        public void BuildMenu()
        {
            foreach(var btn in _spawnedButtons)
            {
                Destroy(btn);
            }
            _spawnedButtons.Clear();

            if (_themeConfigSO == null || _themeConfigSO.Themes == null) return;

            foreach (var themeData in _themeConfigSO.Themes)
            {
                ThemeMenuButton newButton = Instantiate(_buttonPrefab, _container);
                newButton.Initialize(themeData, OnThemeClicked);
                _spawnedButtons.Add(newButton);
            }
        }

        private void OnThemeClicked(ThemeItemData selectedTheme)
        {
            Debug.Log($"Выбрана тема: {selectedTheme.FolderPath}");

            GameManager.Instance.Folder = selectedTheme.FolderPath;
            GameManager.Instance.TableName = selectedTheme.GameTableName;

            SceneManager.LoadScene(selectedTheme.Scene);
        }
    }
}