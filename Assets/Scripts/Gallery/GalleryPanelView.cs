using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gallery
{
    public class GalleryPanelView : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _root;

        [Header("Grid — клетки уже стоят в сцене")]
        [SerializeField] private RectTransform _gridParent;

        [Header("Colors")]
        [SerializeField] private Color _closedColor = Color.black;
        [SerializeField] private Color _openedColor = Color.white;
        [SerializeField] private Color _selectionTint = new Color(1f, 0.85f, 0.4f, 1f);

        [Header("Mark")]
        [SerializeField] private string _markString = "?";

        [Header("Bottom bar")]
        [SerializeField] private TMP_Text _counterText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _upButton;
        [SerializeField] private Button _downButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _closeButton;

        [Header("Fullscreen")]
        [SerializeField] private Image _fullscreenImage;
        [SerializeField] private TMP_Text _fullscreenQuestionMark;

        [Header("Text")]        
        [SerializeField] private TMP_Text _selectButtonText;
        [SerializeField] private TMP_Text _closeButtonText;

        public event Action<int> OnCellClicked;
        public event Action OnLeftClicked;
        public event Action OnRightClicked;
        public event Action OnUpClicked;
        public event Action OnDownClicked;
        public event Action OnSelectClicked;
        public event Action OnCloseClicked;

        private readonly List<Image> _images = new();
        private readonly List<TMP_Text> _questions = new();

        private void Awake()
        {
            CollectCells();

            _leftButton.onClick.AddListener(() => OnLeftClicked?.Invoke());
            _rightButton.onClick.AddListener(() => OnRightClicked?.Invoke());
            _upButton.onClick.AddListener(() => OnUpClicked?.Invoke());
            _downButton.onClick.AddListener(() => OnDownClicked?.Invoke());
            _selectButton.onClick.AddListener(() => OnSelectClicked?.Invoke());
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            _fullscreenImage.gameObject.SetActive(false);
            _fullscreenQuestionMark.gameObject.SetActive(false);

            Hide();
        }

        private void CollectCells()
        {
            int index = 0;

            foreach (Transform child in _gridParent)
            {
                var img = child.GetComponent<Image>();
                if (img == null) continue;

                var qm = child.GetComponentInChildren<TMP_Text>(true);//здесь true подразумевает использование как активного объекта, так и не активного(setActive(false))

                int capturedIndex = index;
                var btn = child.GetComponent<Button>();
                if (btn != null)
                    btn.onClick.AddListener(() => OnCellClicked?.Invoke(capturedIndex));

                _images.Add(img);
                _questions.Add(qm);
                index++;
            }
        }

        public void Show(GalleryPanelData data)
        {
            ApplyData(data);
            _counterText.text = data.CounterText;
            HideFullscreen();
            _root.SetActive(true);
        }

        public void Hide()
        {
            HideFullscreen();
            _root.SetActive(false);
        }

        public void SetSelected(int index)
        {
            for (int i = 0; i < _images.Count; i++)
            {
                bool isSelected = i == index;
                var baseColor = _questions[i].gameObject.activeSelf
                    ? _closedColor
                    : _openedColor;

                _images[i].color = isSelected ? _selectionTint : baseColor;
            }
        }

        public void ShowFullscreen(Sprite sprite, bool isOpened)
        {
            if (isOpened && sprite != null)
            {
                _fullscreenImage.sprite = sprite;
                _fullscreenImage.enabled = true;
                _fullscreenImage.gameObject.SetActive(true);
                _fullscreenQuestionMark.gameObject.SetActive(false);
            }
            else
            {
                _fullscreenImage.gameObject.SetActive(false);
                _fullscreenQuestionMark.gameObject.SetActive(true);
                _fullscreenQuestionMark.text = _markString;
            }
        }

        public void HideFullscreen()
        {
            _fullscreenImage.gameObject.SetActive(false);
            _fullscreenQuestionMark.gameObject.SetActive(false);
        }

        public bool IsFullscreenActive =>
            _fullscreenImage.gameObject.activeSelf ||
            _fullscreenQuestionMark.gameObject.activeSelf;

        private void ApplyData(GalleryPanelData data)
        {
            int total = _images.Count;

            for (int i = 0; i < total; i++)
            {
                if (i >= data.Items.Length)
                {
                    _images[i].gameObject.SetActive(false);
                    continue;
                }

                _images[i].gameObject.SetActive(true);

                var item = data.Items[i];

                if (item.IsOpened && item.Sprite != null)
                {
                    _images[i].sprite = item.Sprite;
                    _images[i].preserveAspect = true;
                    _images[i].color = _openedColor;
                    _questions[i].gameObject.SetActive(false);
                }
                else
                {
                    _images[i].sprite = null;
                    _images[i].color = _closedColor;
                    _questions[i].gameObject.SetActive(true);
                    _questions[i].text = _markString;
                }
            }
        }

        public void SetLocalizedTexts(string selectText, string closeText)
        {            
            if (_selectButtonText != null) _selectButtonText.text = selectText;
            if (_closeButtonText != null) _closeButtonText.text = closeText;
        }

        public void SetSelectButtonText(string text)
        {
            if (_selectButtonText != null) _selectButtonText.text = text;
        }
    }
}