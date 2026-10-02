using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using AtributeEditor;
using UnityEngine.SceneManagement;
using Data;
using Progression;
using Localization;
using EndPanel;
using Json;
using Verification;
using Services;

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

        [SerializeField] private TMP_Text _mainMenuButtonText;
        [SerializeField] private Button _mainMenuButton;        

        [Header("Сцена меню")]
        [SerializeField] private string _nameMenuScene;

        [Header("Локализация UI")]
        [SerializeField] private UIConfigSO _uiConfig;
        

        [Header("Кнопки на TV")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _returnMainMenu;

        [Header("Контроллеры")]
        [SerializeField] private TimerController _timerController;
        [SerializeField] private EndPanelController _endPanelController;
               
        private float _startTime;
        private float _totalTime;
        
        private IQuizJsonService _jsonService;

        private IWordsCache _cache;
        private IVerificationService _verificationService;
        private IProgressionService _progressionService;

        private string _namePixelSize = "_PixelSize";
        private PixelizationStat _pixelStat = new PixelizationStat();

        private int _currentLevelIndex = 0;
        private int _correctCount = 0;
        private bool _isWinAnimation = false;
        private float _targetPixelSize;
        private List<string> _usedSynonymsKeys = new List<string>();    

        private string _currentReplicaKey = "";

        private void Start()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StartQuizNoise();

            if (GameManager.Instance != null)
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

            var factory = new QuizServiceFactory();
            _jsonService = factory.CreateJsonService();
            _jsonService.Initialize(textAsset);
            _jsonService.ShuffleLevels();

            _cache = factory.CreateWordsCache();
            _verificationService = factory.CreateVerificationService(_cache);
            _progressionService = factory.CreateProgressionService();

            InitUI();            

            Localization.LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;
            InitPanelEnd();

            _inputField.onSubmit.AddListener(OnInputFieldSubmit);
            _nextButton.onClick.AddListener(NextLevel);
            
            InitializeControllers();

            LoadLevel(_currentLevelIndex);                      
        }

        private void InitializeControllers()
        {
            if(_timerController == null)
            {
                Debug.LogError("[QuizGameController] TimerController не назначен!");
                return;
            }
            else
            {
                _timerController.UpdateTextMask(GetLocalizedString(_uiConfig.EndPanel.TimerTextKey));
                _timerController.OnTimerFinished += HandleTimerFinished;

                _startTime = Time.time;
                _timerController.StartTimer(); 
            }             
        }

        private void OnDestroy()
        {
            if (_timerController != null)
                _timerController.OnTimerFinished -= HandleTimerFinished;

            _cache.Dispose();

            if (Localization.LanguageManager.Instance != null)
                Localization.LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopQuizNoise();
        }        

        private void RefreshAllUI()
        {
            
            _mainMenuButtonText.text = GetLocalizedString(_uiConfig.Settings.MainMenuButton);

            

            _restartButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.EndPanel.RestartButtonKey);
            _returnMainMenu.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.EndPanel.MenuButtonKey);

            _nextButton.GetComponentInChildren<TMP_Text>().text = GetLocalizedString(_uiConfig.Game.NextButton);
        }

        private void InitUI()
        {            
            RefreshAllUI();

            _mainMenuButton.onClick.AddListener(GoToMainMenu);

           
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
                _statusText.text = LocalizationHelper.Get(_tableNameReplicas, _currentReplicaKey);
            }            

            //обновление таймера
            if (_timerController != null)
            {
                string mask = GetLocalizedString(_uiConfig.EndPanel.TimerTextKey);
                _timerController.UpdateTextMask(mask);
            }

            if (_endPanelController != null)
            {                
                _endPanelController.RefreshDisplay();
            }

           
        }


        private void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(_nameMenuScene);
        }

        public string GetLocalizedString(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;
            return LocalizationHelper.Get(_tableNameUI, key);
        }

        private void Update()
        {
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

        private void OnGameFinished(bool timeOut)
        {           
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopQuizNoise();

            //_timerController.PauseTimer();
            _timerController.SetVisible(false);//теперь пауза такая
            _totalTime = Time.time - _startTime;

            _inputField.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);

            _quizImage.gameObject.SetActive(false);            
            
            int totalLevels = _jsonService.GetTotalLevels();

            _endPanelController.ShowResult(_totalTime, _correctCount, totalLevels, timeOut);            
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
                if (Core.AudioManager.Instance != null)
                    Core.AudioManager.Instance.PlaySoundAnswer(result);

                randomKey = _replicasSO.ReplicasData.Answers.Ignore.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = LocalizationHelper.Get(_tableNameReplicas, randomKey);
                _currentReplicaKey = randomKey;
                _inputField.text = "";
                _statusText.fontStyle = FontStyles.Normal;
                _inputField.ActivateInputField();
                return;
            }

            _targetPixelSize = _progressionService.CalculatePixelSize(result, _pixelStat, _blurStep);       

            _isWinAnimation = result == VerificationResult.Win;
            
            switch (result)
            {
                case VerificationResult.Win:
                    _statusText.fontStyle = FontStyles.Bold;
                    randomKey = _replicasSO.ReplicasData.Answers.Win.Keys.GetRandomItem()?.key ?? "";

                    _inputField.DeactivateInputField();
                    _inputField.gameObject.SetActive(false);
                    _nextButton.gameObject.SetActive(true);
                    _nextButton.Select();

                    _correctCount++;

                    break;

                case VerificationResult.CloseMatch:
                    _statusText.fontStyle = FontStyles.Italic;
                    randomKey = _replicasSO.ReplicasData.Answers.CloseMatch.Keys.GetRandomItem()?.key ?? "";
                    break;

                case VerificationResult.Wrong:
                    _statusText.fontStyle = FontStyles.Normal;
                    randomKey = _replicasSO.ReplicasData.Answers.Wrong.Keys.GetRandomItem()?.key ?? "";
                    break;
            }

            if (Core.AudioManager.Instance != null)
            {
                Core.AudioManager.Instance.PlaySoundAnswer(result);
            }

            _statusText.text = LocalizationHelper.Get(_tableNameReplicas, randomKey);
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
                _statusText.fontStyle = FontStyles.Normal;
                randomKey = _replicasSO.ReplicasData.Finishing.Finish.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = LocalizationHelper.Get(_tableNameReplicas, randomKey);
                _currentReplicaKey = randomKey;

                OnGameFinished(false);

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
            else
            {
                Debug.LogError("_quizImage.sprite не найден!!!");
            }

            _nextButton.gameObject.SetActive(false);
            _inputField.gameObject.SetActive(true);

            randomKey = _replicasSO.ReplicasData.Questions.Question.Keys.GetRandomItem()?.key ?? "";

            _statusText.text = LocalizationHelper.Get(_tableNameReplicas, randomKey);
            _currentReplicaKey = randomKey;

            _inputField.text = "";
            _inputField.ActivateInputField();
        }

        private void NextLevel()
        {
            if (Core.AudioManager.Instance != null)
            {
                Core.AudioManager.Instance.ResetNoiseForNewQuestion();
            }
            
            _currentLevelIndex++;
            LoadLevel(_currentLevelIndex);
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
    
        private void HandleTimerFinished()
        {
            OnGameFinished(true);
        }


    }
}