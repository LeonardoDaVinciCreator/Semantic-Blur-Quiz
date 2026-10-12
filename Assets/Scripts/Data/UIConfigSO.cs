
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "UILocalization", menuName = "UI/UI Config")]
    public class UIConfigSO: ScriptableObject
    {
        [Header("Игровые UI (кнопки, подсказки)")]
        public GameUI Game = new GameUI();        

        [Header("Панель завершения")]
        public EndPanelSO EndPanel = new EndPanelSO();

        [Header("Панель галереи")]
        public GalleryConfig Gallery = new GalleryConfig();
    }
}