using System;
using SequenceBreaker._00_System;
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
//        public Button timelineButton;
//        public Button playButton;
//        public Button homeButton;

        public Image timelineImage;
        public Image playImage;
        public Image homeImage;

        public GameObject timelineTab;
        public GameObject playTab;
        public GameObject homeTab;

        // default GameObject
        public GameObject defaultTimelineView;
        public GameObject defaultPlayView;
        public GameObject defaultHomeView;
        
        // current tab.
        public string currentTab;

        //For enemy unitClass load
        public ItemDataBase itemDataBase;
        public HomeContents homeContents;
        
        // For Home Character update 
        public CharacterStatusDisplay characterStatusDisplay;
        
        // Log list update
        private void Start()
        {

            ActivateTab("TimelineTab");
            
        }


        public void ActivateTab(string toActivateTab)
        {
//            var colors = timelineButton.colors;
//            var activateColors = colors;
//            activateColors.normalColor = Color.white;
//            var deactivateColors = colors;
//            deactivateColors.normalColor = Color.gray;
            
            switch (toActivateTab)
            {
                case "TimelineTab":
                    if (currentTab == toActivateTab)
                    {
                        defaultTimelineView.SetActive(true);
                        defaultTimelineView.transform.SetAsLastSibling();
                    }
//                    timelineButton.colors = activateColors;
//                    playButton.colors = deactivateColors;
//                    homeButton.colors = deactivateColors;
                    
                    timelineImage.color = Color.white;
                    playImage.color = Color.gray;
                    homeImage.color = Color.gray;
                    
                    timelineTab.SetActive(true);
                    playTab.SetActive(false);
                    homeTab.SetActive(false);


                    //BattleLogEnhancedScrollController.GetComponent<BattleLogEnhancedScrollController>().DrawBattleLog();

                    break;
                case "PlayTab":
                    if (currentTab == toActivateTab)
                    {
                        defaultPlayView.SetActive(true);
                        defaultPlayView.transform.SetAsLastSibling();
                    }
//                    timelineButton.colors = deactivateColors;
//                    playButton.colors = activateColors;
//                    homeButton.colors = deactivateColors;

                    timelineImage.color = Color.gray;
                    playImage.color = Color.white;
                    homeImage.color = Color.gray;

                    
//                     unit load
                    foreach (var homeContentData in homeContents.homeContentList)
                    {
                        foreach (var unit in homeContentData.unitClassList)
                        {
                            //Bug:This way to load info is not collect.
                            UnitClass loadUnit = itemDataBase.LoadUnitInfo(unit);
                            unit.itemList = loadUnit.itemList;
                            //experience should only load in initial not this timing.
                            unit.experience = loadUnit.experience;

//                            unit.itemList = itemDataBase.LoadUnitInfo("item-" + unit.affiliation + "-" + unit.uniqueId);
                        }
                    }

                    playTab.SetActive(true);
                    homeTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                case "HomeTab":
                    if (currentTab == toActivateTab)
                    {
                        defaultHomeView.SetActive(true);
                        defaultHomeView.transform.SetAsLastSibling();
                    }
//                    timelineButton.colors = deactivateColors;
//                    playButton.colors = deactivateColors;
//                    homeButton.colors = activateColors;
                    
                    timelineImage.color = Color.gray;
                    playImage.color = Color.gray;
                    homeImage.color = Color.white;
                    
                    //only ally should calculate level
                    if (characterStatusDisplay.unitList[characterStatusDisplay.selectedUnitNo].affiliation ==
                        Affiliation.Ally)
                    {
                        characterStatusDisplay.unitList[characterStatusDisplay.selectedUnitNo].CalculateLevel();
                    }
                    characterStatusDisplay.RefreshCharacterStatusAndItemList();

                    homeTab.SetActive(true);
                    playTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                default:
                    Debug.LogError("unexpected tab value :" + toActivateTab);
                    break;
            }

            currentTab = toActivateTab;
        }
    }
}