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
using UnityEngine.InputSystem.Interactions;

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

        [SerializeField] private TMP_Text _statusText;

        [Header("Пауза/Настройки")]
        [SerializeField] private GameObject _settingsPanel;        
       
        [SerializeField] private TMP_Text _mainMenuButtonText;
        [SerializeField] private Button _mainMenuButton;        

        [Header("Сцена меню")]
        [SerializeField] private string _nameMenuScene;

        [Header("Локализация UI")]
        [SerializeField] private UIConfigSO _uiConfig;

        [Header("Таймер")]
        [SerializeField] private float _timeLimit = 60f;
        [Header("ui таймера")]
        [SerializeField] private TMP_Text _timerText;

        [Header("Объекты завершения")]
        [SerializeField] private TMP_Text _titlePanelEnd;
        [SerializeField] private TMP_Text _timeLabel;
        [SerializeField] private TMP_Text _timeValue;
        [SerializeField] private TMP_Text _scoreLabel;
        [SerializeField] private TMP_Text _scoreValue;
        [SerializeField] private TMP_Text _bestTimeLabel;
        [SerializeField] private TMP_Text _bestTimeValue;
        [SerializeField] private TMP_Text _bestScoreLabel;
        [SerializeField] private TMP_Text _bestScoreValue;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _returnMainMenu;

        private float _currentTime;
        private bool _isTimerRunning = false;
        private float _startTime;
        
        
        private QuizJsonService _jsonService;

        private WordsCache _cache;
        private QuizVerificationService _verificationService;
        private QuizProgressionService _progressionService;

        private string _namePixelSize = "_PixelSize";
        private PixelizationStat _pixelStat = new PixelizationStat();

        private int _currentLevelIndex = 0;
        private int _correctCount = 0;
        private bool _isWinAnimation = false;
        private float _targetPixelSize;
        private List<string> _usedSynonymsKeys = new List<string>();    

        private bool _isPaused = false;    

        private string _currentReplicaKey = "";

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
            _jsonService.ShuffleLevels();

            _cache = new WordsCache();
            _verificationService = new QuizVerificationService(_cache);

            _progressionService = new QuizProgressionService();

            InitUI();
            Localization.LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
            InitPanelEnd();

            _inputField.onSubmit.AddListener(OnInputFieldSubmit);
            _nextButton.onClick.AddListener(NextLevel);


            

            LoadLevel(_currentLevelIndex);

            _currentTime = _timeLimit;
            _isTimerRunning = true;
            _startTime = Time.time;
        }

        private void OnDestroy()
        {
            _cache.Dispose();

            if (Localization.LanguageManager.Instance != null)
                Localization.LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;
        }

        

        private void RefreshAllUI()
        {
            
            _mainMenuButtonText.text = GetLocalizedString(_uiConfig.Settings.MainMenuButton);

            _titlePanelEnd.text = GetLocalizedString(_uiConfig.EndPanel.TitlegGameOverKey);
            _timeLabel.text = GetLocalizedString(_uiConfig.EndPanel.TimeLabelKey);
            _scoreLabel.text = GetLocalizedString(_uiConfig.EndPanel.ScoreLabelKey);
            _bestTimeLabel.text = GetLocalizedString(_uiConfig.EndPanel.BestTimeLabelKey);
            _bestScoreLabel.text = GetLocalizedString(_uiConfig.EndPanel.BestScoreLabelKey);

            _restartButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.EndPanel.RestartButtonKey);
            _returnMainMenu.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.EndPanel.MenuButtonKey);

            _nextButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.Game.NextButton);
        }

        private void InitUI()
        {            
            RefreshAllUI();

            _mainMenuButton.onClick.AddListener(GoToMainMenu);

            //_settingsPanel.SetActive(false);

            _titlePanelEnd.gameObject.SetActive(false);
            _timeLabel.gameObject.SetActive(false);
            _timeValue.gameObject.SetActive(false);
            _scoreLabel.gameObject.SetActive(false);
            _scoreValue.gameObject.SetActive(false);
            _bestTimeLabel.gameObject.SetActive(false);
            _bestTimeValue.gameObject.SetActive(false);
            _bestScoreLabel.gameObject.SetActive(false);
            _bestScoreValue.gameObject.SetActive(false);
        }

        private void InitPanelEnd()
        {
            _restartButton.onClick.AddListener(ReloadGame);
            _returnMainMenu.onClick.AddListener(LoadMainMenu);
        }

        private void OnLanguageChanged(string obj)
        {
            RefreshAllUI();

            if (!string.IsNullOrEmpty(_currentReplicaKey))
            {
                _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, _currentReplicaKey);
            }

            if (_titlePanelEnd.gameObject.activeSelf)
            {
                UpdateResultDisplay();
            }
        }

        private void UpdateResultDisplay()
        {
            float totalTime = Time.time - _startTime;
            int totalLevels = _jsonService.GetTotalLevels();

            _timeValue.text = string.Format(GetLocalizedString(_uiConfig.EndPanel.TimeTextKey), totalTime);
            _scoreValue.text = string.Format(GetLocalizedString(_uiConfig.EndPanel.ScoreTextKey), _correctCount, totalLevels);

            UpdateBestResultsUI();
        }

        private void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(_nameMenuScene);
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

            Timer();
        }

        private void Timer()
        {
            if (_isTimerRunning && !_isPaused)
            {
                _currentTime -= Time.deltaTime;
                if (_currentTime <= 0f)
                {
                    _currentTime = 0f;
                    _isTimerRunning = false;
                    OnTimeEnd(true);
                }
            }

            _timerText.text = string.Format(
                GetLocalizedString(_uiConfig.EndPanel.TimerTextKey),
                Mathf.Ceil(_currentTime)
            );
        }

        private void OnTimeEnd(bool timeOut)
        {
            _isTimerRunning = false;

            _inputField.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);

            _quizImage.gameObject.SetActive(false);
            _statusText.gameObject.SetActive(false);
            _timerText.gameObject.SetActive(false);

            _titlePanelEnd.gameObject.SetActive(true);
            _timeLabel.gameObject.SetActive(true);
            _timeValue.gameObject.SetActive(true);
            _scoreLabel.gameObject.SetActive(true);
            _scoreValue.gameObject.SetActive(true);
            _bestTimeLabel.gameObject.SetActive(true);
            _bestTimeValue.gameObject.SetActive(true);
            _bestScoreLabel.gameObject.SetActive(true);
            _bestScoreValue.gameObject.SetActive(true);

            string titleKey = timeOut ? _uiConfig.EndPanel.TitlegGameOverKey : _uiConfig.EndPanel.TitleCompleteKey;
            _titlePanelEnd.text = GetLocalizedString(titleKey);

            float totalTime = Time.time - _startTime;
            int totalLevels = _jsonService.GetTotalLevels();

            _timeValue.text = string.Format(
                GetLocalizedString(_uiConfig.EndPanel.TimeTextKey),
                totalTime
            );

             _scoreValue.text = string.Format(
                GetLocalizedString(_uiConfig.EndPanel.ScoreTextKey),
                _correctCount,
                totalLevels
            );

            UpdateBestResultsUI();

            SaveBestResult(totalTime, _correctCount);
        }

        private void UpdateBestResultsUI()
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime", -1f);
            int bestScore = PlayerPrefs.GetInt("BestScore", -1);
            int totalLevels = _jsonService.GetTotalLevels();

            if (bestTime > 0)
            {
                _bestTimeValue.text = string.Format(
                    GetLocalizedString(_uiConfig.EndPanel.BestTimeTextKey),
                    bestTime
                );
            }
            else
            {
                _bestTimeValue.text = "-";
            }

            if (bestScore > 0)
            {
                _bestScoreValue.text = string.Format(
                    GetLocalizedString(_uiConfig.EndPanel.BestScoreTextKey),
                    bestScore,
                    totalLevels
                );
            }
            else
            {
                _bestScoreValue.text = "-";
            }
        }

        private void SaveBestResult(float time, int score)
        {
            float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
            if(time<bestTime) PlayerPrefs.SetFloat("BestTime", time);

            float bestScore = PlayerPrefs.GetFloat("BestScore", 0);
            if(score > bestScore) PlayerPrefs.SetFloat("BestScore", score);

            PlayerPrefs.Save();
            Debug.Log(time + " " + score);
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
                _currentReplicaKey = randomKey;
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

                    _correctCount++;

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
            _currentReplicaKey = randomKey;

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

                _statusText.color = Color.green;
                randomKey = _replicasSO.ReplicasData.Finishing.Finish.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);
                _currentReplicaKey = randomKey;

                OnTimeEnd(false);

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
            _currentReplicaKey = randomKey;

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
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(_nameMenuScene);
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