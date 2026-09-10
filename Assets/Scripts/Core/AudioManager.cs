using Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _soundClickSource;
        [SerializeField] private AudioSource _soundSource;

        [Header("Music Sound")]
        [SerializeField] private AudioClip _soundMusic;

        [Header("Audio Sound")]
        [SerializeField] private AudioClip _soundClick;
        [Header("Sound answer")]
        [SerializeField] private AudioClip _soundIgnore;
        [SerializeField] private AudioClip _soundCloseMatch;
        [SerializeField] private AudioClip _soundWin;
        [SerializeField] private AudioClip _soundWrong;

        [Header("Default Volumes")]
        [SerializeField] private float _defaultMusicVolume = 0.5f;
        [SerializeField] private float _defaultSoundVolume = 0.5f;

        public static float DefaultMusicVolume => 0.5f;
        public static float DefaultSoundVolume => 0.5f;

        private float _musicVolume;
        private float _soundVolume;

        protected override void Awake()
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

        private void PlaySound(AudioClip clip, AudioSource audio)
        {
            if (audio != null && clip != null)
                audio.PlayOneShot(clip);
        }

        public void PlayMusic()
        {
            PlaySound(_soundMusic, _musicSource);
        }

        public void PlayClick()
        {
            PlaySound(_soundClick, _soundClickSource);
        }

        public void PlaySoundAnswer(VerificationResult result)
        {
            switch(result)
            {
                case VerificationResult.Ignore:
                    PlaySound(_soundIgnore, _soundSource);
                    break;

                case VerificationResult.Win:
                    PlaySound(_soundCloseMatch, _soundSource);
                    break;

                case VerificationResult.CloseMatch:
                    PlaySound(_soundWin, _soundSource);
                    break;

                case VerificationResult.Wrong:
                    PlaySound(_soundWrong, _soundSource);
                    break;
            }
                
        }
    }
}