using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class EndPanelSO
    {
        [Header("название таблицы")]
        public string NameTable;

        [Header("Ключи локалиции для элементов")]
        public string TitlegGameOverKey = "ui_finish_title";
        public string TitleCompleteKey = "ui_finish_title_complete";
        public string TimeLabelKey = "time_label";
        public string TimeTextKey = "time_text";
        public string ScoreLabelKey = "score_label";
        public string ScoreTextKey = "score_text";
        public string BestTimeLabelKey = "best_time_label";
        public string BestTimeTextKey = "best_time_text";
        public string BestScoreLabelKey = "best_score_label";
        public string BestScoreTextKey = "best_score_text";
        public string RestartButtonKey = "restart_button";
        public string MenuButtonKey = "menu_button";

        public string TimerTextKey = "timer_text";
    }
}