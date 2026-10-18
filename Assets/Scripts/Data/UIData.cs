
using System;
using AtributeEditor;

namespace Data
{
    [Serializable]
    public class GameUI
    {
        [LocalizationKey] public string NextButton;
        [LocalizationKey] public string SubmitButton;
        [LocalizationKey] public string GalleryButton;
        [LocalizationKey] public string MenuButton;
        [LocalizationKey] public string SettingsButton;
        
        [LocalizationKey] public string FinishQuiz;
        [LocalizationKey] public string QuitButton;
        
        [LocalizationKey] public string HintTextKey = "hint_text";
        [LocalizationKey] public string HintButtonKey = "hint_button";
    }
}