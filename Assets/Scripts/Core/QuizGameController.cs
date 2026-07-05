using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Json;
using Data;
using Progression;
using System.Collections.Generic;
using AtributeEditor;

namespace Core
{
    public class QuizGameController : MonoBehaviour
    {
        [Header("Шаг")]
        [SerializeField] private float _blurStep = 10f;

        [Header("Таблица для вопросов и ответов пользователю")]
        [TableName]
        [SerializeField] private string _tableNameReplicas = "Replicas";

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
        
        private QuizJsonService _jsonService;
        private QuizVerificationService _verificationService;
        private QuizProgressionService _progressionService;

        private string _namePixelSize = "_PixelSize";
        private PixelizationStat _pixelStat = new PixelizationStat();

        private int _currentLevelIndex = 0;
        private float _targetPixelSize;
        private List<string> _usedSynonymsKeys = new List<string>();

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
            _jsonService.Initialize(textAsset); // тут поидее нужно папку пихать

            _verificationService = new QuizVerificationService();
            _progressionService = new QuizProgressionService();

            _inputField.onSubmit.AddListener(OnInputFieldSubmit);
            _nextButton.onClick.AddListener(NextLevel);

            LoadLevel(_currentLevelIndex);


        }

        private void Update()
        {
            // Плавное изменение пикселизации на экране
            float currentMatValue = _quizImage.material.GetFloat(_namePixelSize);
            if (Mathf.Abs(currentMatValue - _targetPixelSize) > 0.1f)
            {
                float nextValue = Mathf.Lerp(currentMatValue, _targetPixelSize, Time.deltaTime * 5f);
                _quizImage.material.SetFloat(_namePixelSize, nextValue);
                _pixelStat.CurrentPixelSize = nextValue;
            }
        }

        private void OnInputFieldSubmit(string playerInput)
        {
            if (string.IsNullOrEmpty(playerInput)) return;

            LevelData level = _jsonService.GetLevelData(_currentLevelIndex);
            if (level == null) return;

            VerificationResult result = _verificationService.VerifyAnswer(playerInput, level, _tableName, _usedSynonymsKeys);

            _targetPixelSize = _progressionService.CalculatePixelSize(result, _pixelStat, _blurStep);

            string randomKey = "";

            // Обрабатываем результат
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

            if (level == null)
            {
                Debug.Log("Все уровни пройдены, показ результатов и переход на главное меню");

                _inputField.text = "";
                _inputField.DeactivateInputField();
                _inputField.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);

                _statusText.color = Color.green;
                _statusText.text = "Тема пройдена!";

                return;
            }

            _progressionService.ResetProgress();
            _usedSynonymsKeys.Clear();

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

            string randomKey = _replicasSO.ReplicasData.Questions.Question.Keys.GetRandomItem()?.key ?? "";

            _statusText.text = Localization.LanguageManager.Instance.GetWordByKey(_tableNameReplicas, randomKey);

            _inputField.text = "";
            _inputField.ActivateInputField();
        }

        private void NextLevel()
        {
            _currentLevelIndex++;
            LoadLevel(_currentLevelIndex);
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