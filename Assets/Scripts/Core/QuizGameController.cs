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
using Gallery;

namespace Core
{
    public class QuizGameController : MonoBehaviour
    {
        [Header("Шаг")]
        [SerializeField] private float _blurStep = 10f;

        [Header("Таблица для вопросов и ответов пользов ателю")]
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
        [SerializeField] private Button _submitButton;
        [SerializeField] private TMP_Text _galleryButtonText;

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

        [SerializeField] private TMP_Text _restartButtonText;
        [SerializeField] private TMP_Text _returnMainMenuText;
        [SerializeField] private TMP_Text _nextButtonText;
        [SerializeField] private TMP_Text _submitButtonText;

        [Header("Контроллеры")]
        [SerializeField] private TimerController _timerController;
        [SerializeField] private EndPanelController _endPanelController;

        [SerializeField] private GalleryController _galleryController;
        [SerializeField] private Button _galleryButton;
               
        private float _startTime;
        private float _totalTime;
        
        private IQuizJsonService _jsonService;

        private IWordsCache _cache;
        private IVerificationService _verificationService;
        private IPixelizationService _pixelizationService;
        private IQuizProgressService _progressService;

        private string _namePixelSize = "_PixelSize";
        private PixelizationStat _pixelStat = new PixelizationStat();

        private bool _wasInputActive;
        private bool _wasSubmitActive;
        private bool _wasNextActive;
        private bool _wasStatusActive;

        private int _currentLevelIndex = 0;
        private int _correctCount = 0;
        private bool _isWinAnimation = false;
        private float _targetPixelSize;
        private List<string> _usedSynonymsKeys = new List<string>();    

        private string _currentReplicaKey = "";
        private bool _isGameFinished = false;
        private string _currentThemeId;

        private void Start()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StartQuizNoise();

            if (GameManager.Instance != null)
                Debug.Log(GameManager.Instance.TableName);

            _quizImage.material = Instantiate(_quizImage.material);
            GetPixelizationStat();

            // === Тема ===
            if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.Folder))
            {
                _nameFolder = GameManager.Instance.Folder;
                _tableName = GameManager.Instance.TableName;
            }

            _currentThemeId = ThemeIdHelper.FromFolder(_nameFolder);

            // === Сервисы ===
            TextAsset textAsset = Resources.Load<TextAsset>($"{_nameFolder}/levels");

            var factory = new QuizServiceFactory();
            _jsonService = factory.CreateJsonService();
            _jsonService.Initialize(textAsset);
            _jsonService.ShuffleLevels();

            _cache = factory.CreateWordsCache();
            _verificationService = factory.CreateVerificationService(_cache);
            _pixelizationService = factory.CreateProgressionService();
            _progressService = new QuizProgressService();

            // === UI и подписки на кнопки ===
            InitUI();
            InitPanelEnd();
            InitGallery();

            Localization.LanguageManager.Instance.OnLanguageChanged += OnLanguageChanged;

            _inputField.onSubmit.AddListener(OnInputFieldSubmit);
            _nextButton.onClick.AddListener(NextLevel);
            _submitButton.onClick.AddListener(OnSubmitButtonClicked);

            // === Контроллеры ===
            InitializeControllers();

            // === Старт игры ===
            LoadLevel(_currentLevelIndex);
        }

        private void InitializeControllers()
        {
            InitializeTimer();
            InitializeEndPanel();
            InitializeGallery();
        }

        private void InitializeTimer()
        {
            if (_timerController == null)
            {
                Debug.LogError("[QuizGameController] TimerController не назначен!");
                return;
            }

            _timerController.UpdateTextMask(GetLocalizedString(_uiConfig.EndPanel.TimerTextKey));
            _timerController.OnTimerFinished += HandleTimerFinished;

            _startTime = Time.time;
            _timerController.StartTimer();
        }

        private void InitializeEndPanel()
        {
            if (_endPanelController == null)
            {
                Debug.LogError("[QuizGameController] EndPanelController не назначен!");
                return;
            }
        }

        private void InitializeGallery()
        {
            if (_galleryController == null)
            {
                Debug.LogError("[QuizGameController] GalleryController не назначен!");
                return;
            }

            _galleryController.Initialize(_jsonService, _progressService);
            _galleryController.OnOpened += HandleGalleryOpened;
            _galleryController.OnClosed += HandleGalleryClosed;
        }

        private void OnDestroy()
        {
            if (_timerController != null)
                _timerController.OnTimerFinished -= HandleTimerFinished;

            if (_galleryController != null)
            {
                _galleryController.OnOpened -= HandleGalleryOpened;
                _galleryController.OnClosed -= HandleGalleryClosed;
            }

            _cache?.Dispose();

            if (Localization.LanguageManager.Instance != null)
                Localization.LanguageManager.Instance.OnLanguageChanged -= OnLanguageChanged;

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopQuizNoise();
        }

        private void RefreshAllUI()
        {            
            _mainMenuButtonText.text = GetLocalizedString(_uiConfig.Game.MenuButton);

            _restartButtonText.text = GetLocalizedString(_uiConfig.EndPanel.RestartButtonKey);
            _returnMainMenuText.text = GetLocalizedString(_uiConfig.EndPanel.MenuButtonKey);

            _nextButtonText.text = GetLocalizedString(_uiConfig.Game.NextButton);
            _submitButtonText.text = GetLocalizedString(_uiConfig.Game.SubmitButton);
            _galleryButtonText.text = GetLocalizedString(_uiConfig.Game.GalleryButton);
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

        private void InitGallery()
        {
            _galleryButton.onClick.AddListener(() => _galleryController.Open(_nameFolder));
        }

        private void HandleGalleryOpened()
        {            
            _timerController.PauseTimer();
            
            _wasInputActive = _inputField.gameObject.activeSelf;
            _wasSubmitActive = _submitButton.gameObject.activeSelf;
            _wasNextActive = _nextButton.gameObject.activeSelf;
            _wasStatusActive = _statusText.gameObject.activeSelf;           
            
            _inputField.gameObject.SetActive(false);
            _submitButton.gameObject.SetActive(false);
            _nextButton.gameObject.SetActive(false);
            _statusText.gameObject.SetActive(false);

            _galleryButton.gameObject.SetActive(false);

            _quizImage.gameObject.SetActive(false);       

            if (_isGameFinished)            {
                
                _endPanelController.SetVisible(false);
            }     
        }

        private void HandleGalleryClosed()
        {
            _inputField.gameObject.SetActive(_wasInputActive);
            _submitButton.gameObject.SetActive(_wasSubmitActive);
            _nextButton.gameObject.SetActive(_wasNextActive);
            _statusText.gameObject.SetActive(_wasStatusActive);
            _galleryButton.gameObject.SetActive(true);
            _quizImage.gameObject.SetActive(true);

            if (_isGameFinished)            {
                
                _endPanelController.SetVisible(true);
            }
            else
            {                
                _timerController.SetVisible(true);
                _timerController.ResumeTimer();

                if (_wasInputActive)
                    _inputField.ActivateInputField();
            }
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

            if (_galleryController != null)
                _galleryController.RefreshLocalization(); 
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
            _isGameFinished = true;
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopQuizNoise();

            //_timerController.PauseTimer();
            _timerController.SetVisible(false);//теперь пауза такая
            _totalTime = Time.time - _startTime;

            _inputField.gameObject.SetActive(false);
            _submitButton.gameObject.SetActive(false);
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

            _targetPixelSize = _pixelizationService.CalculatePixelSize(result, _pixelStat, _blurStep);       

            _isWinAnimation = result == VerificationResult.Win;
            
            switch (result)
            {
                case VerificationResult.Win:
                    _statusText.fontStyle = FontStyles.Bold;
                    randomKey = _replicasSO.ReplicasData.Answers.Win.Keys.GetRandomItem()?.key ?? "";

                    _inputField.DeactivateInputField();
                    _inputField.gameObject.SetActive(false);
                    _submitButton.gameObject.SetActive(false);
                    _nextButton.gameObject.SetActive(true);
                    _nextButton.Select();

                    _correctCount++;
                    _progressService.MarkGuessed(_currentThemeId, level.id);

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

            if (result != VerificationResult.Win) 
            {
                _inputField.ActivateInputField();
                _submitButton.gameObject.SetActive(true);
            }
        }

        private void OnSubmitButtonClicked()
        {
            OnInputFieldSubmit(_inputField.text);
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
                _submitButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);                
                _statusText.fontStyle = FontStyles.Normal;
                randomKey = _replicasSO.ReplicasData.Finishing.Finish.Keys.GetRandomItem()?.key ?? "";
                _statusText.text = LocalizationHelper.Get(_tableNameReplicas, randomKey);
                _currentReplicaKey = randomKey;

                OnGameFinished(false);

                return;
            }

            _pixelizationService.ResetProgress();
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
            _submitButton.gameObject.SetActive(true);

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