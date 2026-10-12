using System;
using AtributeEditor;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class GalleryConfig
    {
        [Header("Ключи локализации")]
        [LocalizationKey] public string SelectButtonKey = "gallery_open";
        [LocalizationKey] public string CloseImageKey = "gallery_close_image";
        [LocalizationKey] public string CloseButtonKey = "gallery_back";
    }
}