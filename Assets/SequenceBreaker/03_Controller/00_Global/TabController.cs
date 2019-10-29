using System;
using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._01_Home;
using SequenceBreaker._03_Controller._01_Home.Character;
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

        // default GameObject
        public GameObject defaultTimelineView;
        public GameObject defaultPlayView;
        public GameObject defaultHomeView;
        
        // current tab.
        private string _currentTab;

        //For enemy unitClass load
        public ItemDataBase itemDataBase;
        public HomeContents homeContents;
        
        // For Home Character update 
        public CharacterStatusDisplay characterStatusDisplay;
        
        // Log list update
        private void Start()
        {
            ActivateTab("PlayTab");
            
        }


        public void ActivateTab(string toActivateTab)
        {
            var colors = timelineButton.colors;
            var activateColors = colors;
            activateColors.normalColor = Color.white;
            var deactivateColors = colors;
            deactivateColors.normalColor = Color.gray;
            
            switch (toActivateTab)
            {
                case "TimelineTab":
                    if (_currentTab == toActivateTab)
                    {
                        defaultTimelineView.SetActive(true);
                        defaultTimelineView.transform.SetAsLastSibling();
                    }
                    timelineButton.colors = activateColors;
                    playButton.colors = deactivateColors;
                    homeButton.colors = deactivateColors;
                    timelineTab.SetActive(true);
                    playTab.SetActive(false);
                    homeTab.SetActive(false);


                    //BattleLogEnhancedScrollController.GetComponent<BattleLogEnhancedScrollController>().DrawBattleLog();

                    break;
                case "PlayTab":
                    if (_currentTab == toActivateTab)
                    {
                        defaultPlayView.SetActive(true);
                        defaultPlayView.transform.SetAsLastSibling();
                    }
                    timelineButton.colors = deactivateColors;
                    playButton.colors = activateColors;
                    homeButton.colors = deactivateColors;

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
                    if (_currentTab == toActivateTab)
                    {
                        defaultHomeView.SetActive(true);
                        defaultHomeView.transform.SetAsLastSibling();
                    }
                    timelineButton.colors = deactivateColors;
                    playButton.colors = deactivateColors;
                    homeButton.colors = activateColors;
                    
                    characterStatusDisplay.unitList[characterStatusDisplay.selectedUnitNo].CalculateLevel();
                    characterStatusDisplay.RefleshCharacterStatusAndItemList();
                    homeTab.SetActive(true);
                    playTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                default:
                    break;
            }

            _currentTab = toActivateTab;
        }
    }
}