using System.Collections;
using Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class AudioManager : ManagerBase<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;

        [SerializeField] private AudioSource _staticNoiseSource;

        [SerializeField] private AudioSource _soundClickSource;
        [SerializeField] private AudioSource _soundSource;

        [Header("Music Sound")]
        [SerializeField] private AudioClip _soundMusic;

        [SerializeField] private AudioClip _staticNoiseLoop;

        [Header("Audio Sound")]
        [SerializeField] private AudioClip _soundClick;
        [Header("Sound answer(опционально)")]
        [SerializeField] private AudioClip _soundIgnore;
        [SerializeField] private AudioClip _soundCloseMatch;
        [SerializeField] private AudioClip _soundWin;
        [SerializeField] private AudioClip _soundWrong;

        [Header("Default Volumes")]
        [SerializeField] private float _defaultMusicVolume = 0.5f;
        [SerializeField] private float _defaultSoundVolume = 0.5f;

        [SerializeField] private float _currentNoiseIntensity = 0.5f;
        [SerializeField] private float _maxNoiseCap = 1.0f;
        [SerializeField] private float _noiseStep = 0.2f;

        private float _targetNoiseIntensity = 0.5f;

        public static float DefaultMusicVolume => Instance != null ? Instance._defaultMusicVolume : 0.5f;
        public static float DefaultSoundVolume => Instance != null ? Instance._defaultSoundVolume : 0.5f;

        private const string Music_Volume_Key = "MusicVolume";
        private const string Sound_Volume_Key = "SoundVolume";

        private float _musicVolume;
        private float _soundVolume;

        private Coroutine _noiseFadeCoroutine;        

        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            // Загружаем сохранённые настройки
            _musicVolume = PlayerPrefs.GetFloat(Music_Volume_Key, _defaultMusicVolume);
            _soundVolume = PlayerPrefs.GetFloat(Sound_Volume_Key, _defaultSoundVolume);

            ApplyVolumes();
            //StartStaticNoise();

            _targetNoiseIntensity = _currentNoiseIntensity;
        }

        public void StartQuizNoise()
        {
            StartStaticNoise();
        }

        public void StopQuizNoise()
        {
            if (_staticNoiseSource != null && _staticNoiseSource.isPlaying)
                _staticNoiseSource.Stop();
        }

        private void ApplyVolumes()
        {
            if (_musicSource != null) _musicSource.volume = _musicVolume;
            if (_soundSource != null) _soundSource.volume = _soundVolume;
            if (_soundClickSource != null) _soundClickSource.volume = _soundVolume;

            UpdateNoiseVolume();
        }
        
        public float GetMusicVolume() => _musicVolume;
        public float GetSoundVolume() => _soundVolume;

        public void SetMusicVolume(float value)
        {
            _musicVolume = Mathf.Clamp01(value);
            if (_musicSource != null) _musicSource.volume = _musicVolume;            
            
            UpdateNoiseVolume();

            PlayerPrefs.SetFloat(Music_Volume_Key, _musicVolume);
            PlayerPrefs.Save();
        }

        public void SetSoundVolume(float value)
        {
            _soundVolume = Mathf.Clamp01(value);
            if (_soundSource != null) _soundSource.volume = _soundVolume;
            if (_soundClickSource != null) _soundClickSource.volume = _soundVolume;
            
            PlayerPrefs.SetFloat(Sound_Volume_Key, _soundVolume);
            PlayerPrefs.Save();
        }

        public void SetNoiseIntensityNormalized(float normalizedValue)
        {
            _currentNoiseIntensity = Mathf.Clamp(normalizedValue, 0f, _maxNoiseCap);
            UpdateNoiseVolume();
        }

        private void Update()
        {            
            if (!Mathf.Approximately(_currentNoiseIntensity, _targetNoiseIntensity))
            {
                _currentNoiseIntensity = Mathf.MoveTowards(_currentNoiseIntensity, _targetNoiseIntensity, Time.deltaTime * 1.5f);
                UpdateNoiseVolume();
            }
        }

        private void UpdateNoiseVolume()
        {
            if (_staticNoiseSource != null)
            {                
                _staticNoiseSource.volume = _musicVolume * _currentNoiseIntensity;
                Debug.Log($"[Audio DEBUG] Текущая громкость шума: {_staticNoiseSource.volume}, Интенсивность: {_currentNoiseIntensity}");
            }
        }

        private void StartStaticNoise()
        {
            if (_staticNoiseSource == null || _staticNoiseLoop == null) return;
            _staticNoiseSource.clip = _staticNoiseLoop;
            _staticNoiseSource.loop = true;
            UpdateNoiseVolume();
            _staticNoiseSource.Play();
        }

        public void ResetNoiseForNewQuestion()
        {
            _targetNoiseIntensity = 0.5f;
        }

        private void PlaySound(AudioClip clip, AudioSource audio)
        {
            if (audio != null && clip != null)
                audio.PlayOneShot(clip);
        }        

        public void PlayMusic()
        {
            if (_musicSource == null || _soundMusic == null) return;
            _musicSource.clip = _soundMusic;
            _musicSource.loop = true;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
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
                    PlaySound(_soundWin, _soundSource);
                    _targetNoiseIntensity = 0f;
                    break;

                case VerificationResult.CloseMatch:
                    PlaySound(_soundCloseMatch, _soundSource);
                    _targetNoiseIntensity = Mathf.Max(0f, _targetNoiseIntensity - _noiseStep);
                    break;

                case VerificationResult.Wrong:
                    PlaySound(_soundWrong, _soundSource);
                    _targetNoiseIntensity = Mathf.Min(_maxNoiseCap, _targetNoiseIntensity + _noiseStep);
                    break;
            }
                
        }
    }
}