using SequenceBreaker._03_Controller._03_Log;
using UnityEngine;

namespace SequenceBreaker._04_Timeline_Tab.Log
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
            battleLogEnhancedScrollController.battle = battle;
            battleLogEnhancedScrollController.DrawBattleLog();

            battleLog.SetActive(true);
            logList.SetActive(false);       

        }
    }
}
