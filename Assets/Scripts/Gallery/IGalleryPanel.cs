using UnityEngine;

namespace Gallery
{
    // Данные одной клетки
    public readonly struct GalleryItemData
    {
        public readonly int Index;
        public readonly Sprite Sprite;// null если закрыта
        public readonly bool IsOpened;

        public GalleryItemData(int index, Sprite sprite, bool isOpened)
        {
            Index = index;
            Sprite = sprite;
            IsOpened = isOpened;
        }
    }

    // Данные для всей панели
    public class GalleryPanelData
    {
        public string CounterText;// "11/18"
        public GalleryItemData[] Items;
    }

    public interface IGalleryPanel
    {
        int TotalCount { get; }
        int OpenedCount { get; }
        bool IsOpened(int index);
        Sprite GetSprite(int index);// null если закрыт
        GalleryPanelData BuildData();
    }
}