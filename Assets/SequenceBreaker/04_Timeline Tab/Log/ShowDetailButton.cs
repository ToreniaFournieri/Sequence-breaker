using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class ShowDetailButton : MonoBehaviour
{
    public GameObject battle;
    public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value

    //active Battle log and inactive loglist
    public GameObject battleLog;
    public GameObject logList;


    public void ShowDetail()
    {
        // pass the value of the result of Battle
        battleLogEnhancedScrollController.Battle = battle;
        battleLogEnhancedScrollController.DrawBattleLog();

        battleLog.SetActive(true);
        logList.SetActive(false);       

    }
}
