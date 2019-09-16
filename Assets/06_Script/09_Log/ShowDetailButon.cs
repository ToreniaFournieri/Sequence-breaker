using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowDetailButton : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject Battle;
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value
    public GameObject transparentMessage;

    public void ShowDetail()
    {
        Battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);
        battleLogEnhancedScrollController.Battle = Battle;

        //Not works well
        RunBattle _runbattle = Battle.GetComponent<RunBattle>();
        //get Mission name
        string missionName = _runbattle.missionText;
        string missionLevel = " (lv:" + _runbattle.missionLevel.ToString() + ")";


        transparentMessage.GetComponentInChildren<Text>().text += "\n " + "Mission start: " + missionName + missionLevel;
        transparentMessage.SetActive(true);
    }
}
