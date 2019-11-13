using _00_Asset._08_Easy_Panel_Transitions.Scripts;
using SequenceBreaker._05_Play.Prepare;
using SequenceBreaker._06_Timeline.BattleLogView;
using UnityEngine;

namespace SequenceBreaker._06_Timeline
{
    public sealed class ShowDetailButton : MonoBehaviour
    {
        public GameObject battle;
        public BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value

        //active Battle log and inactive LogList
        public GameObject battleLog;
        public GameObject logList;


        public void ShowDetail()
        {
            // pass the value of the result of Battle
            battleLogEnhancedScrollController.runbattle = battle.GetComponent<RunBattle>();
            battleLogEnhancedScrollController.DrawBattleLog();
            
            battleLog.GetComponent<PanelAnimator>().StartAnimIn();
//            battleLog.SetActive(true);
//            logList.SetActive(false);       
            logList.transform.SetAsFirstSibling();
//            
//            Debug.Log("Show detail is pressed");
        }
    }
}
