using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Localization;
using Core;
using System.Threading.Tasks;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Слайдеры")]
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;

    [Header("Тексты (для локализации)")]
    [SerializeField] private TMP_Text _musicLabel;
    [SerializeField] private TMP_Text _soundLabel;

    [Header("Ключи локализации (если используется UIConfig)")]
    [SerializeField] private string _tableNameUI = "UITable";
    [SerializeField] private string _musicKey = "settings_music";
    [SerializeField] private string _soundKey = "settings_sound";

    private void Start()
    {
        // Загружаем сохранённые значения
        LoadVolumeSettings();

        // Подписываемся на события слайдеров
        _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        _soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);

        // Подписываемся на смену языка (если тексты локализуются)
        if (LanguageManager.Instance != null)
        {
            LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
            // Обновляем тексты при старте
            RefreshLabels();
        }
    }

    private void OnDestroy()
    {
        if (LanguageManager.Instance != null)
            LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    private void LoadVolumeSettings()
    {
        float music = AudioManager.Instance != null ? AudioManager.Instance.GetMusicVolume() : AudioManager.DefaultMusicVolume;
        float sound = AudioManager.Instance != null ? AudioManager.Instance.GetSoundVolume() : AudioManager.DefaultSoundVolume;

        _musicSlider.value = music;
        _soundSlider.value = sound;
    }

    private void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        // При необходимости обновить отображение (например, процент)
    }

    private void OnSoundSliderChanged(float value)
    {
        AudioManager.Instance?.SetSoundVolume(value);
    }

    private void OnLanguageChanged(string langCode)
    {
        RefreshLabels();
    }

    private async void RefreshLabels()
    {
        if (_musicLabel != null)
            _musicLabel.text = await GetLocalizedString(_musicKey);

        if (_soundLabel != null)
            _soundLabel.text = await GetLocalizedString(_soundKey);
    }

    private async Task<string> GetLocalizedString(string key)
    {
        return await LocalizationHelper.GetAsync(_tableNameUI, key);
    }
}