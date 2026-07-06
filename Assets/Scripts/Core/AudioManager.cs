using Manager;
using UnityEngine;

namespace Core
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundSource;

        [Header("Default Volumes")]
        [SerializeField] private float _defaultMusicVolume = 0.5f;
        [SerializeField] private float _defaultSoundVolume = 0.5f;

        public static float DefaultMusicVolume => 0.5f;
        public static float DefaultSoundVolume => 0.5f;

        private float _musicVolume;
        private float _soundVolume;

        private void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            // Загружаем сохранённые настройки
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", _defaultMusicVolume);
            _soundVolume = PlayerPrefs.GetFloat("SoundVolume", _defaultSoundVolume);

            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (_musicSource != null) _musicSource.volume = _musicVolume;
            if (_soundSource != null) _soundSource.volume = _soundVolume;
        }
        
        public float GetMusicVolume() => _musicVolume;
        public float GetSoundVolume() => _soundVolume;

        public void SetMusicVolume(float value)
        {
            _musicVolume = Mathf.Clamp01(value);
            if (_musicSource != null) _musicSource.volume = _musicVolume;
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            PlayerPrefs.Save();
        }

        public void SetSoundVolume(float value)
        {
            _soundVolume = Mathf.Clamp01(value);
            if (_soundSource != null) _soundSource.volume = _soundVolume;
            PlayerPrefs.SetFloat("SoundVolume", _soundVolume);
            PlayerPrefs.Save();
        }

        public void PlaySound(AudioClip clip)
        {
            if (_soundSource != null && clip != null)
                _soundSource.PlayOneShot(clip);
        }
    }
}