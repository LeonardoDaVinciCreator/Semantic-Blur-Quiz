using System;
using TMPro;
using UnityEngine;

namespace Timer
{
    public class TimerView: MonoBehaviour
    {        
        [SerializeField] private TMP_Text _text;
        
        public void SetText(string displayText)
        {                       
            _text.text = displayText;
        } 

        public void SetVisible(bool visible)
        {
            _text.gameObject.SetActive(visible);
        }
    }
}