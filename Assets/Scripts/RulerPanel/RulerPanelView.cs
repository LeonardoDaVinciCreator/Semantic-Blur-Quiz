using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RulerPanel
{
    public class RulerPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _rulerText;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Button _closeButton;

        public void Show(string hintText, string buttonText)
        {
            _rulerText.text = hintText;
            _buttonText.text = buttonText;
            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        public void SetCloseListener(System.Action onClose)
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(() => onClose.Invoke());
        }

        public void SetLocalizedTexts(string selectText, string closeText)
        {            
            if (_rulerText != null) _rulerText.text = selectText;
            if (_buttonText != null) _buttonText.text = closeText;
        }
    }
}