using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using System;
using System.Collections.Generic;
using AtributeEditor;

namespace Editor
{
    [CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
    public class LocalizationKeyDrawer : PropertyDrawer
    {
        private GUIContent[] _keyOptions;
        private int _selectedIndex = -1;

        private void LoadAllLocalizationKeys()
        {
            if (_keyOptions != null) return;

            List<GUIContent> options = new List<GUIContent> { new GUIContent("None") };
            var tables = LocalizationEditorSettings.GetStringTableCollections();

            foreach (var table in tables)
            {
                string tableName = table.TableCollectionName;
                foreach (var sharedKey in table.SharedData.Entries)
                {
                    options.Add(new GUIContent($"{tableName}/{sharedKey.Key}"));
                }
            }

            _keyOptions = options.ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use LocalizationKey with string.");
                return;
            }

            LoadAllLocalizationKeys();

            string currentRawValue = property.stringValue;
            
            // ИСПРАВЛЕНО: Теперь поиск корректно находит индекс по чистому ключу
            _selectedIndex = Array.FindIndex(_keyOptions, opt => opt.text.EndsWith("/" + currentRawValue) || opt.text == currentRawValue);
            if (_selectedIndex == -1) _selectedIndex = 0;

            EditorGUI.BeginProperty(position, label, property);

            int prevIndex = _selectedIndex;
            _selectedIndex = EditorGUI.Popup(position, label, _selectedIndex, _keyOptions);

            if (_selectedIndex != prevIndex)
            {
                if (_selectedIndex == 0)
                {
                    property.stringValue = "";
                }
                else
                {
                    string selectedFullText = _keyOptions[_selectedIndex].text;
                    int slashIndex = selectedFullText.IndexOf('/');
                    property.stringValue = selectedFullText.Substring(slashIndex + 1);
                }
                
                // Принудительное сохранение изменений
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }
    }
}
