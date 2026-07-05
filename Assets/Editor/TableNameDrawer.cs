using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using System;
using AtributeEditor;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TableNameAttribute))]
    public class TableNameDrawer : PropertyDrawer
    {
        private static GUIContent[] _tableOptions;
        private static int _selectedIndex = -1;

        private void UpdateTableOptions()
        {
            // Не пересобираем список каждый кадр, если он уже есть
            if (_tableOptions != null) return;

            var tables = LocalizationEditorSettings.GetStringTableCollections();
            _tableOptions = new GUIContent[tables.Count + 1];
            _tableOptions[0] = new GUIContent("None");
            
            for (int i = 0; i < tables.Count; i++)
            {
                _tableOptions[i + 1] = new GUIContent(tables[i].TableCollectionName);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use TableName with string.");
                return;
            }

            UpdateTableOptions();

            string currentTable = property.stringValue;
            
            // Корректно вычисляем индекс текущего сохраненного значения
            _selectedIndex = Array.FindIndex(_tableOptions, opt => opt.text == currentTable);
            if (_selectedIndex == -1) _selectedIndex = 0;

            EditorGUI.BeginProperty(position, label, property);
            
            // Отрисовываем Popup
            int newIndex = EditorGUI.Popup(position, label, _selectedIndex, _tableOptions);
            
            // Если игрок выбрал другое значение, записываем его напрямую в свойство
            if (newIndex != _selectedIndex)
            {
                property.stringValue = (newIndex == 0) ? "" : _tableOptions[newIndex].text;
                
                // Критически важно: принудительно говорим Unity сохранить изменения на диске!
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            
            EditorGUI.EndProperty();
        }
    }
}