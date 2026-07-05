using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    /*
        ключи слов находящиеся в таблицах локализации
    */
    [Serializable]
    public class LevelData
    {
        public int id;
        public string imageName;
        public string correctWordKey;
        public List<string> synonymsKeys;
    }

    [Serializable]
    public class LevelList
    {
        public List<LevelData> levels = new List<LevelData>();
    }
}

