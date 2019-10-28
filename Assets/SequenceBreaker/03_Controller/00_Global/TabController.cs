using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._01_Home;
using SequenceBreaker._10_Global;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller._00_Global
{
    [System.Serializable]
    public sealed class TabController : MonoBehaviour
    {
        public Button timelineButton;
        public Button playButton;
        public Button homeButton;

        public GameObject timelineTab;
        public GameObject playTab;
        public GameObject homeTab;


        //For enemy unitClass load
        public ItemDataBase itemDataBase;
//        public MasterEnemyUnitList masterEnemyUnitList;
        public HomeContents homeContents;
        
        // Log list update

        public void ActivateTab(string toActivateTab)
        {
            switch (toActivateTab)
            {
                case "TimelineTab":
                    timelineTab.SetActive(true);
                    playTab.SetActive(false);
                    homeTab.SetActive(false);


                    //BattleLogEnhancedScrollController.GetComponent<BattleLogEnhancedScrollController>().DrawBattleLog();

                    break;
                case "PlayTab":

//                     unit load
                    foreach (var homeContentData in homeContents.homeContentList)
                    {
                        foreach (var unit in homeContentData.unitClassList)
                        {
                            unit.itemList = itemDataBase.LoadItemList("item-" + unit.affiliation + "-" + unit.uniqueId);
                        }
                    }

                    playTab.SetActive(true);
                    homeTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                case "HomeTab":
                    homeTab.SetActive(true);
                    playTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}