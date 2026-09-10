## Unity Project Analysis: Semantic-Blur-Quiz

### 📁 Структура проекта

**Assets/Scripts/ (основные категории):**
- **Core/** - основной игровой цикл и управление
  - `GameManager.cs` - глобальный менеджер игры
  - `QuizGameController.cs` - контроллер игрового процесса
  - `MainMenuController.cs` - меню главной страницы
  - `AudioManager.cs` / `AudioSettingsController.cs` - аудио система
  - `ThemeMenuButton.cs` - управление темами
  
- **Data/** - скрипableObject данные
  - `LevelData.cs` - данные уровней
  - `QuizReplicasSO.cs` / `Replicas.cs` - персонажи/реплики
  - `Theme.cs` / `ThemeConfigSO.cs` - темы оформления
  - `UIConfigSO.cs` / `UIData.cs` - конфигурация UI
  - `EndPanelSO.cs` - панель завершения
  
- **Progression/** - прогресс игрока
  - `QuizProgressionService.cs` - сохранение прогресса
  
- **Verification/** - проверка ответов
  - `QuizVerificationService.cs` - верификация ответов
  - `WordsCache.cs` - кэш слов
  
- **Json/** - работа с JSON
  - `QuizJsonService.cs` - парсинг JSON файлов
  
- **Localization/** - локализация
  - `LanguageManager.cs` - управление языками
  
- **Formatting/** - форматирование текста
  - `FormatingService.cs`
  
- **AtributeEditor/** - атрибуты для редактора
  - `LocalizationKeyAttribute.cs`
  - `TableNameAttribute.cs`

**Assets/Images/ (картинки):**
- `UI.png` (~600KB) - основной UI шаблон
- `Gemini_Generated_Image_*.png` - сгенерированные изображения

**Assets/Scenes/ (сцены):**
- `MenuScene.unity` (~82KB) - начальное меню
- `QuizScene.unity` (~152KB) - игровая сцена

### 🎮 Архитектура игры

**Тип:** Квиз-игра с выбором правильных/неправильных слов (Semantic Blur концепт)

**Основные системы:**
1. **Игровой цикл** - GameManager + QuizGameController
2. **Уровень/прогрессия** - LevelData + QuizProgressionService
3. **Валидация ответов** - QuizVerificationService + WordsCache
4. **Контент** - QuizReplicasSO (персонажи, реплики)
5. **Настройки** - Theme, Audio, UI конфигурации

### 📊 Ключевые файлы

| Файл | Назначение |
|------|-------------|
| `GameManager.cs` | Центральная точка управления |
| `QuizGameController.cs` | Логика игровой сессии |
| `QuizVerificationService.cs` | Проверка ответов игрока |
| `QuizProgressionService.cs` | Сохранение прогресса |
| `LevelData.cs` | Данные уровней квиза |
