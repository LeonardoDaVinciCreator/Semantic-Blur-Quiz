using System.Collections.Generic;

namespace Progression
{
    public interface IQuizProgressService
    {
        // Узнать, угадан ли конкретный уровень темы
        bool IsGuessed(string themeId, int levelIndex);

        // Отметить уровень угаданным
        void MarkGuessed(string themeId, int levelIndex);

        // Получить весь набор угаданных индексов темы
        HashSet<int> GetGuessed(string themeId);

        // Сколько угадано (для "5 из 18")
        int GetGuessedCount(string themeId);

        // Сбросить прогресс одной темы
        void ResetTheme(string themeId);

        // Сбросить всё (по всем темам)
        void ResetAll(string[] themeIds);
    }
}