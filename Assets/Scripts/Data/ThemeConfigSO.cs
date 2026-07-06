using System;
using System.Collections.Generic;
using AtributeEditor;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "MainMenuThemes", menuName = "Quiz/MainMenu Themes Config")]
    public class ThemeConfigSO : ScriptableObject
    {
        public List<ThemeItemData> Themes;

        #if UNITY_EDITOR
        // метод автоматически менеят путь папки, тоесть теперь можно просто перемещять папки
        private void OnValidate()
        {
            if (Themes == null) return;

            foreach (var theme in Themes)
            {
                if (theme.FolderAsset != null)
                {
                    string fullPath = UnityEditor.AssetDatabase.GetAssetPath(theme.FolderAsset);
                    
                    string resourcesMarker = "Resources/";
                    int resourcesIndex = fullPath.IndexOf(resourcesMarker);

                    if (resourcesIndex != -1)
                    {
                        theme.FolderPath = fullPath.Substring(resourcesIndex + resourcesMarker.Length);
                    }
                    else
                    {
                        Debug.LogError($"[ThemeConfigSO] Папка '{theme.FolderAsset.name}' должна находиться внутри папки Resources!");
                        theme.FolderPath = string.Empty;
                    }
                }
                else
                {
                    theme.FolderPath = string.Empty;
                } 
            }
        }
        #endif
    }
}