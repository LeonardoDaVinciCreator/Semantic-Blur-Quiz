using System;
using System.Collections.Generic;
using AtributeEditor;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ThemeItemData
    {
        [Header("Ключ локализации названия темы")]
        [LocalizationKey]
        public string ThemeNameKey;

        [Header("Путь к папке темы в Resources")]
        public string FolderPath; // Например: "Themes/Fruits"
        #if UNITY_EDITOR
        public UnityEngine.Object FolderAsset;
        #endif
        [Header("Таблица в локализации")]
        [TableName]
        public string TableName;

        [TableName]
        public string GameTableName;

        [Header("Название сцены")]
        public string Scene;
    }    
}