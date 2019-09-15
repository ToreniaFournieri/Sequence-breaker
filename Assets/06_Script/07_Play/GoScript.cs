using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject Battle;
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value

    public void GoBattle()
    {
        Battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);
        battleLogEnhancedScrollController.Battle = Battle;
    }
}
