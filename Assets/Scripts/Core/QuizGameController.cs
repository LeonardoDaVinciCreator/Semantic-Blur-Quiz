using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Json;
using Data;
using Progression;
using System.Collections.Generic;
using AtributeEditor;
using System;
using UnityEngine.SceneManagement;

namespace Core
{
    public class QuizGameController : MonoBehaviour
    {
        [Header("Шаг")]
        [SerializeField] private float _blurStep = 10f;

        [Header("Таблица для вопросов и ответов пользователю")]
        [TableName]
        [SerializeField] private string _tableNameReplicas = "Replicas";

        [Header("Таблица для UI")]
        [TableName]
        [SerializeField] private string _tableNameUI = "UITable";

        [Header("Таблица для квиза")]
        [TableName]
        [SerializeField] private string _tableName = "GameWords";

        [Header("папка в ресурсах")]
        [SerializeField] private string _nameFolder = "Themes/Fruits";
        [SerializeField] private QuizReplicasSO _replicasSO;

        [Header("UI")]
        [SerializeField] private Image _quizImage;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _menuButton;

        [SerializeField] private TMP_Text _statusText;

        [Header("Пауза/Настройки")]
        [SerializeField] private GameObject _settingsPanel;
        
        [SerializeField] private TMP_Text _musicText;
        [SerializeField] private Slider _musicSlider;

        [SerializeField] private TMP_Text _soundText;
        [SerializeField] private Slider _soundSlider;
       
        [SerializeField] private TMP_Text _mainMenuButtonText;
        [SerializeField] private Button _mainMenuButton;        

        [Header("Сцена меню")]
        [SerializeField] private string _nameMenuScene;

        [Header("Локализация UI")]
        [SerializeField] private UIConfigSO _uiConfig;
        
        private QuizJsonService _jsonService;
        private QuizVerificationService _verificationService;
        private QuizProgressionService _progressionService;

        private string _namePixelSize = "_PixelSize";
        private PixelizationStat _pixelStat = new PixelizationStat();

        private int _currentLevelIndex = 0;
        private bool _isWinAnimation = false;
        private float _targetPixelSize;
        private List<string> _usedSynonymsKeys = new List<string>();    

        private bool _isPaused = false;    

        private void Start()
        {
            Debug.Log(GameManager.Instance.TableName);

            _quizImage.material = Instantiate(_quizImage.material); // чтобы разблокировать изменения данных для материала
            GetPixelizationStat();

            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.Folder))
            {
                _nameFolder = GameManager.Instance.Folder;
                _tableName = GameManager.Instance.TableName;
            }

            // пример пути Resources/Themes/Fruits/levels.json
            TextAsset textAsset = Resources.Load<TextAsset>($"{_nameFolder}/levels");

            _jsonService = new QuizJsonService();
            _jsonService.Initialize(textAsset);

            _verificationService = new QuizVerificationService();
            _progressionService = new QuizProgressionService();

            InitUI();

            _inputField.onSubmit.AddListener(OnInputFieldSubmit);
            _nextButton.onClick.AddListener(NextLevel);

            _menuButton.onClick.AddListener(GoToMainMenu);

            

            LoadLevel(_currentLevelIndex);
        }        

        private void InitUI()
        {            
            _musicText.text = GetLocalizedString(_uiConfig.Settings.MusicLabel);
            _soundText.text = GetLocalizedString(_uiConfig.Settings.SoundLabel);
            _mainMenuButtonText.text = GetLocalizedString(_uiConfig.Settings.MainMenuButton);

            _nextButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.Game.NextButton);
            _menuButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.Game.MenuButton);

            _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
            _soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);

            LoadVolumeSettings();

            _mainMenuButton.onClick.AddListener(GoToMainMenu);

            //_settingsPanel.SetActive(false);
        }

        private string GetLocalizedString(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return Localization.LanguageManager.Instance.GetWordByKey(_tableNameUI, key);
        }

        private void Update()
        {
            if (_isPaused) return;

            // Плавное изменение пикселизации на экране
            float currentMatValue = _quizImage.material.GetFloat(_namePixelSize);
            if (Mathf.Abs(currentMatValue - _targetPixelSize) > 0.1f)
            {
                float speed;
                if (_isWinAnimation)
                {
                    float totalChange = Mathf.Abs(_targetPixelSize - _pixelStat.CurrentPixelSize);
                    // Чем больше изменение, тем медленнее скорость
                    speed = Mathf.Clamp(27.5f / (totalChange / _blurStep + 0.5f), 0.33f, 10f);
                }
                else
                {
                    speed = _blurStep; // обычная скорость
                }

                float nextValue = Mathf.Lerp(currentMatValue, _targetPixelSize, Time.deltaTime * speed);
                _quizImage.material.SetFloat(_namePixelSize, nextValue);
                _pixelStat.CurrentPixelSize = nextValue;
            }
        }

        private void OnInputFieldSubmit(string playerInput)
        {
            if (string.IsNullOrEmpty(playerInput)) return;

            _pixelStat.CurrentPixelSize = _quizImage.material.GetFloat(_namePixelSize);

            LevelData level = _jsonService.GetLevelData(_currentLevelIndex);
            if (level == null) return;

            VerificationResult result = _verificationService.VerifyAnswer(playerInput, level, _tableName, _usedSynonymsKeys);
            
            string randomKey = "";

            if (result == VerificationResult.Ignore)
            {
                randomKey = _replicasSO.ReplicasData.Answers.Ignore.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);
                _statusText.color = Color.gray;
                _inputField.text = "";
                _inputField.ActivateInputField();
                return;
            }

            _targetPixelSize = _progressionService.CalculatePixelSize(result, _pixelStat, _blurStep);       

            _isWinAnimation = result == VerificationResult.Win;
            
            switch (result)
            {
                case VerificationResult.Win:
                    _statusText.color = Color.green;
                    randomKey = _replicasSO.ReplicasData.Answers.Win.Keys.GetRandomItem()?.key ?? "";

                    _inputField.DeactivateInputField();
                    _inputField.gameObject.SetActive(false);
                    _nextButton.gameObject.SetActive(true);
                    _nextButton.Select();
                    break;

                case VerificationResult.CloseMatch:
                    _statusText.color = Color.yellow;
                    randomKey = _replicasSO.ReplicasData.Answers.CloseMatch.Keys.GetRandomItem()?.key ?? "";
                    break;

                case VerificationResult.Wrong:
                    _statusText.color = Color.red;
                    randomKey = _replicasSO.ReplicasData.Answers.Wrong.Keys.GetRandomItem()?.key ?? "";
                    break;
            }

            _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);

            _inputField.text = "";
            if (result != VerificationResult.Win) _inputField.ActivateInputField();
        }

        private void LoadLevel(int index)
        {
            LevelData level = _jsonService.GetLevelData(index);

            string randomKey = "";

            if (level == null)
            {
                Debug.Log("Все уровни пройдены, показ результатов и переход на главное меню");

                _inputField.text = "";
                _inputField.DeactivateInputField();
                _inputField.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);

                _menuButton.gameObject.SetActive(true);
                _menuButton.Select();

                _statusText.color = Color.green;
                randomKey = _replicasSO.ReplicasData.Finishing.Finish.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);

                return;
            }

            _progressionService.ResetProgress();
            _usedSynonymsKeys.Clear();
            _isWinAnimation = false;

            _pixelStat.CurrentPixelSize = _pixelStat.MinPixelSize;
            _targetPixelSize = _pixelStat.MinPixelSize;
            _quizImage.material.SetFloat(_namePixelSize, _pixelStat.CurrentPixelSize);

            // пример пути Resources/Themes/Fruits/apple.png
            Sprite levelSprite = Resources.Load<Sprite>($"{_nameFolder}/{level.imageName}");
            if (levelSprite != null)
            {
                _quizImage.sprite = levelSprite;
            }

            _nextButton.gameObject.SetActive(false);
            _inputField.gameObject.SetActive(true);

            randomKey = _replicasSO.ReplicasData.Questions.Question.Keys.GetRandomItem()?.key ?? "";

            _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);

            _inputField.text = "";
            _inputField.ActivateInputField();
        }

        private void NextLevel()
        {
            _currentLevelIndex++;
            LoadLevel(_currentLevelIndex);
        }

        private void OpenSettingsPanel()
        {
            _settingsPanel.SetActive(true);
            _isPaused = true;
            Time.timeScale = 0f;

            _inputField.DeactivateInputField();
            _inputField.interactable = false;
            _nextButton.interactable = false;

            // отображение текущих значений громкости
            UpdateVolumeDisplay();
        }

        private void CloseSettingsPanel()
        {
            _settingsPanel.SetActive(false);
            _isPaused = false;
            Time.timeScale = 1f;

            _inputField.interactable = true;
            _nextButton.interactable = true;

            // Если поле ввода активно (не скрыто после победы), возврат фокуса
            if (_inputField.gameObject.activeSelf)
            {
                _inputField.ActivateInputField();
            }

            SaveVolumeSettings();
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_nameMenuScene);
        }

        private void OnMusicSliderChanged(float value)
        {                      
            AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnSoundSliderChanged(float value)
        {          
            AudioManager.Instance?.SetSoundVolume(value);
        }

        private void LoadVolumeSettings()
        {
            float music = AudioManager.Instance != null ? AudioManager.Instance.GetMusicVolume() : AudioManager.DefaultMusicVolume;
            float sound = AudioManager.Instance != null ? AudioManager.Instance.GetSoundVolume() : AudioManager.DefaultSoundVolume;

            _musicSlider.value = music;
            _soundSlider.value = sound;

            UpdateVolumeDisplay();
        }

        private void SaveVolumeSettings()
        {
            AudioManager.Instance?.SetMusicVolume(_musicSlider.value);
            AudioManager.Instance?.SetSoundVolume(_soundSlider.value);
        }

        private void UpdateVolumeDisplay()
        {
            OnMusicSliderChanged(_musicSlider.value);
            OnSoundSliderChanged(_soundSlider.value);
        }

        private void GetPixelizationStat()
        {
            Material mat = _quizImage.material;
            Shader shader = mat.shader;

            int propertyIndex = shader.FindPropertyIndex(_namePixelSize);

            if(propertyIndex != -1)
            {
                var limits = shader.GetPropertyRangeLimits(propertyIndex);

                _pixelStat.MinPixelSize = limits.x;
                _pixelStat.MaxPixelSize = limits.y;
                _pixelStat.CurrentPixelSize = mat.GetFloat(_namePixelSize);
                _targetPixelSize = _pixelStat.CurrentPixelSize;
            }
        }
    }
}