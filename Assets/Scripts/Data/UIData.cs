
using System;
using AtributeEditor;

namespace Data
{
    [Serializable]
    public class MainMenu
    {
        [LocalizationKey] public string Title;
        [LocalizationKey] public string StartButton;
        [LocalizationKey] public string SettingsButton;
        [LocalizationKey] public string QuitButton;
    }

    [Serializable]
    public class SettingsPanel
    {
        [LocalizationKey] public string Title;
        [LocalizationKey] public string MusicLabel;
        [LocalizationKey] public string SoundLabel;
        [LocalizationKey] public string ContinueButton;
        [LocalizationKey] public string MainMenuButton;
    }

    [Serializable]
    public class GameUI
    {
        [LocalizationKey] public string NextButton;
        [LocalizationKey] public string MenuButton;
        [LocalizationKey] public string SettingsButton;
        
        [LocalizationKey] public string FinishQuiz;
        [LocalizationKey] public string QuitButton;
    }
}