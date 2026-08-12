
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UILocalization", menuName = "UI/UI Config")]
    public class UIConfigSO: ScriptableObject
    {
        [Header("Главное меню (если нужно)")]
        public MainMenu Main;

        [Header("Игровые UI (кнопки, подсказки)")]
        public GameUI Game;        
        
        [Header("Панель настроек")]
        public SettingsPanel Settings;     

        [Header("Панель завершения")]
        public EndPanelSO EndPanel;   
    }
}