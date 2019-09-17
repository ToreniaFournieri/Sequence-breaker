using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject battle;
    //public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value
    public GameObject transparentMessage;

    //for adding battle log to log list
    public LogListSRIA logList;

    private List<GameObject> battleForLogList;

    public void GoBattle()
    {
        battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);
        //battleLogEnhancedScrollController.Battle = Battle;

        //Not works well
        RunBattle _runbattle = battle.GetComponent<RunBattle>();
        //get Mission name
        string missionName = _runbattle.missionText;
        string missionLevel = " (lv:" + _runbattle.missionLevel.ToString() + ")";

        battleForLogList = new List<GameObject>();
        battleForLogList.Add(battle);
        logList.battleList = battleForLogList; 

        transparentMessage.GetComponentInChildren<Text>().text += "\n " + "Mission start: " + missionName + missionLevel;
        transparentMessage.SetActive(true);
    }
}
