using System.Collections.Generic;
using Data;
using Json;
using Progression;
using UnityEngine;

namespace Gallery
{
    public class GalleryPanelModel : IGalleryPanel
    {
        private readonly IQuizJsonService _jsonService;
        private readonly IQuizProgressService _progressService;
        private readonly string _themeFolder;
        private readonly string _themeId;
        private readonly IReadOnlyList<LevelData> _levels;

        public GalleryPanelModel(
            IQuizJsonService jsonService,
            IQuizProgressService progressService,
            string themeFolder)
        {
            _jsonService = jsonService;
            _progressService = progressService;
            _themeFolder = themeFolder;
            _themeId = ThemeIdHelper.FromFolder(themeFolder);
            _levels = jsonService.GetAllLevels();
        }

        public int TotalCount => _jsonService.GetTotalLevels();

        public int OpenedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < _levels.Count; i++)
                    if (IsOpened(i)) count++;
                return count;
            }
        }

        public bool IsOpened(int index)
        {
            if (index < 0 || index >= _levels.Count) return false;
            return _progressService.IsGuessed(_themeId, _levels[index].id);
        }

        public Sprite GetSprite(int index)
        {
            if (!IsOpened(index)) return null;

            LevelData level = _levels[index];
            
            if (level == null) return null;

            return Resources.Load<Sprite>($"{_themeFolder}/{level.imageName}");
        }

        public GalleryPanelData BuildData()
        {
            int total = TotalCount;
            var items = new GalleryItemData[total];

            for (int i = 0; i < total; i++)
                items[i] = new GalleryItemData(i, GetSprite(i), IsOpened(i));

            return new GalleryPanelData
            {
                CounterText = $"{OpenedCount}/{total}",
                Items = items
            };
        }
    }
}