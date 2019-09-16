using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject Battle;
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value
    public GameObject transparentMessageController;

    public void GoBattle()
    {
        Battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);
        battleLogEnhancedScrollController.Battle = Battle;

        //get Mission name
        string missionName = null;

        //Not works well
        Text[] texts = Battle.GetComponents<Text>();
        foreach (Text text in texts)
        {
            if (text.name == "MissionText")
            {
                missionName = text.text;
            }
        }

        

        transparentMessageController.GetComponentInChildren<Text>().text = "Mission start. " + missionName;
        transparentMessageController.SetActive(true);
    }
}
