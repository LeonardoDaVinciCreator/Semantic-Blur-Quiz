using UnityEngine;
using UnityEngine.UI;
using Core;

namespace Components
{
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        public enum VolumeType { Music, Sound }

        [Header("Что регулирует этот слайдер")]
        [SerializeField] private VolumeType _type = VolumeType.Sound;

        [Header("Пауза перед предпросмотром звука (сек)")]
        [SerializeField] private float _previewDelay = 0.5f;

        private Slider _slider;
        private float _lastChangeTime;
        private bool _previewPending;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            
            _slider.minValue = 0f;
            _slider.maxValue = 1f;

            _slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnEnable()
        {            
            if (AudioManager.Instance == null) return;

            float current = _type == VolumeType.Music
                ? AudioManager.Instance.GetMusicVolume()
                : AudioManager.Instance.GetSoundVolume();

            _slider.SetValueWithoutNotify(current);
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnSliderChanged(float value)
        {
            if (AudioManager.Instance == null) return;

            if (_type == VolumeType.Music)
                AudioManager.Instance.SetMusicVolume(value);
            else
                AudioManager.Instance.SetSoundVolume(value);

            _lastChangeTime = Time.unscaledTime;
            _previewPending = true;
        }

        private void Update()
        {
            if (!_previewPending) return;
            if (Time.unscaledTime - _lastChangeTime < _previewDelay) return;

            _previewPending = false;
            
            if (_type == VolumeType.Sound && AudioManager.Instance != null)
                AudioManager.Instance.PlayClick();
        }
    }
}