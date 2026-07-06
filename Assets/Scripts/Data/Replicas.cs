
using System;
using System.Collections.Generic;
using AtributeEditor;
using UnityEngine;  

namespace Data
{    
    [Serializable]
    public class Key
    {
        [LocalizationKey]
        public string key;
    }
    [Serializable]
    public class Replic
    {
        public List<Key> Keys;
    }
    
    /// <summary>
    /// ответы которые показываются случайно исходя из ответа
    /// </summary>
    [Serializable]
    public class Answers
    {        
        public Replic Win;
        
        public Replic CloseMatch;
        
        public Replic Wrong;

        public Replic Ignore;
    }

    /// <summary>
    /// вопросы которые показываются случайно при переходе между картиинками
    /// </summary>
    [Serializable]
    public class Questions
    {
        public Replic Question;
    }

    [Serializable]
    public class Finishing
    {
        public Replic Finish;
    }

    [Serializable]
    public class Replicas
    {
        public Answers Answers;
        public Questions Questions;

        public Finishing Finishing;
    }

    public static class ListExtensions
    {
        public static T GetRandomItem<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
    }
}