using System;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._01_Data.UnitClass;
using SequenceBreaker._03_Controller.Home;
using SequenceBreaker._03_Controller.Home.Character;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller.GUIController.Segue
{
    [Serializable]
    public sealed class TabController : MonoBehaviour
    {
        public Image timelineImage;
        public Image playImage;
        public Image homeImage;

        public GameObject timelineTab;
        public GameObject playTab;
        public GameObject homeTab;
        
        // current tab.
        public string currentTab;

        //For enemy unitClass load
        public ItemDataBase itemDataBase;
        public HomeContents homeContents;
        
        // For Home Character update 
        public CharacterStatusDisplay characterStatusDisplay;
        
        // Segue control
        public SegueController segueController;
        
        // Log list update
        private void Awake()
        {

            ActivateTab("TimelineTab");
            
        }



        public void ActivateTab(string toActivateTab)
        {
            
            switch (toActivateTab)
            {
                case "TimelineTab":
                    if (currentTab == toActivateTab)
                    {
                        segueController.InitTimeLineView();
                        
                    }
                    timelineImage.color = Color.white;
                    playImage.color = Color.gray;
                    homeImage.color = Color.gray;
                    
                    timelineTab.SetActive(true);
                    playTab.SetActive(false);
                    homeTab.SetActive(false);
                    break;
                case "PlayTab":
                    if (currentTab == toActivateTab)
                    {
                        segueController.InitPlayView();

                    }

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

                        }
                    }

                    playTab.SetActive(true);
                    homeTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                case "HomeTab":
                    if (currentTab == toActivateTab)
                    {
                        segueController.InitHomeView();
                    }
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
            segueController.dragAndClose.Init();
            
        }
    }
}