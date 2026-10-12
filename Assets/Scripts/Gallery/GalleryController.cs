using System;
using Json;
using Localization;
using Progression;
using Data;
using UnityEngine;
using UnityEngine.UI;


namespace Gallery
{
    public class GalleryController : MonoBehaviour
    {
        [SerializeField] private GalleryPanelView _view;
        [SerializeField] private UIConfigSO _maskConfig;

        private const int Columns = 6;

        private IGalleryPanel _model;
        private IQuizJsonService _jsonService;
        private IQuizProgressService _progressService;

        private int _selectedIndex;

        public event Action OnOpened;
        public event Action OnClosed;

        public void Initialize(IQuizJsonService jsonService, IQuizProgressService progressService)
        {
            _jsonService = jsonService;
            _progressService = progressService;

            _view.OnCellClicked += HandleCellClicked;
            _view.OnLeftClicked += () => Move(0, -1);
            _view.OnRightClicked += () => Move(0, +1);
            _view.OnUpClicked += () => Move(-1, 0);
            _view.OnDownClicked += () => Move(+1, 0);
            _view.OnSelectClicked += ToggleFullscreen;
            _view.OnCloseClicked += Close;
        }

        public void Open(string themeFolder)
        {
            _model = new GalleryPanelModel(_jsonService, _progressService, themeFolder);
            _selectedIndex = FindFirstOpened();

            RefreshLocalization();

            _view.Show(_model.BuildData());
            _view.SetSelected(_selectedIndex);

            OnOpened?.Invoke();
        }

        public void Close()
        {
            _view.Hide();
            _view.SetSelectButtonText(GetLocalizedString(_maskConfig.Gallery.SelectButtonKey));
            OnClosed?.Invoke();
        }

        private void HandleCellClicked(int index)
        {
            _selectedIndex = index;
            _view.SetSelected(_selectedIndex);

            // Если открыт fullscreen — обновить картинку под новый индекс
            if (_view.IsFullscreenActive)
                ShowFullscreenForSelected();
        }

        private void ToggleFullscreen()
        {
            if (_view.IsFullscreenActive)
            {
                _view.HideFullscreen();
                _view.SetSelectButtonText(GetLocalizedString(_maskConfig.Gallery.SelectButtonKey));
            }
            else
            {
                ShowFullscreenForSelected();
                _view.SetSelectButtonText(GetLocalizedString(_maskConfig.Gallery.CloseImageKey));
            }
        }

        private void ShowFullscreenForSelected()
        {
            Sprite sprite = _model.GetSprite(_selectedIndex);
            bool isOpened = _model.IsOpened(_selectedIndex);
            _view.ShowFullscreen(sprite, isOpened);
        }

        private void Move(int deltaRow, int deltaCol)
        {
            int total = _model.TotalCount;
            int rows = Mathf.CeilToInt((float)total / Columns);

            int row = _selectedIndex / Columns;
            int col = _selectedIndex % Columns;

            col = (col + deltaCol + Columns) % Columns;
            row = (row + deltaRow + rows) % rows;

            int newIndex = row * Columns + col;
            if (newIndex >= total) newIndex = total - 1;

            _selectedIndex = newIndex;
            _view.SetSelected(_selectedIndex);

            if (_view.IsFullscreenActive)
                ShowFullscreenForSelected();
        }

        private int FindFirstOpened()
        {
            for (int i = 0; i < _model.TotalCount; i++)
                if (_model.IsOpened(i)) return i;
            return 0;
        }

        public void RefreshLocalization()
        {
            if (_maskConfig == null) return;
            
            string select = GetLocalizedString(_maskConfig.Gallery.SelectButtonKey);
            string close  = GetLocalizedString(_maskConfig.Gallery.CloseButtonKey);

            _view.SetLocalizedTexts(select, close);
        }

        private string GetLocalizedString(string key)
        {
            if(string.IsNullOrEmpty(key)) return string.Empty;

            return LocalizationHelper.Get(_maskConfig.EndPanel.NameTable, key);
        }
    }
}