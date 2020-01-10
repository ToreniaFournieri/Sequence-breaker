using System;
using SequenceBreaker.Environment;
using SequenceBreaker.Home.EquipListView;
using SequenceBreaker.Home.EquipView;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.GUIController.Segue
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
        //public ItemDataBase itemDataBase;
        //public EquipListContents equipListContents;

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
                    //foreach (var equipListContentData in equipListContents.equipListContentList)
                    //{
                    //    foreach (var unit in equipListContentData.unitWave.unitWave)
                    //    {
                    //        //Bug:This way to load info is not collect.
                    //        //UnitClass loadUnit = itemDataBase.LoadUnitInfo(unit);
                    //        if (unit != null && unit.affiliation == Affiliation.Ally)
                    //        {
                    //            UnitClass loadUnit = ItemDataBase.instance.LoadUnitInfo(unit);
                    //            unit.itemList = loadUnit.itemList;
                    //            //experience should only load in initial not this timing.
                    //            unit.experience = loadUnit.experience;

                    //        }
                    //        else
                    //        {
                    //            // enemy not load now. 201911.26 temp.
                    //        }


                    //    }
                    //}

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
                    //characterTreeViewDataSourceMgr.selectedCharacter
                    //if (characterStatusDisplay.unitWave.unitWave[characterStatusDisplay.selectedUnitNo].affiliation ==
                    //    Affiliation.Ally)
                    //{
                    //    characterStatusDisplay.unitWave.unitWave[characterStatusDisplay.selectedUnitNo].CalculateLevel();
                    //}
                    //if (characterStatusDisplay.characterTreeViewDataSourceMgr.selectedCharacter != null)
                    //{
                    //    if (characterStatusDisplay.characterTreeViewDataSourceMgr.selectedCharacter.affiliation ==
                    //    Affiliation.Ally)
                    //    {

                    //        characterStatusDisplay.characterTreeViewDataSourceMgr.selectedCharacter.CalculateLevel();


                    //    }
                    //    characterStatusDisplay.RefreshCharacterStatusAndItemList();

                    //}


                    homeTab.SetActive(true);
                    playTab.SetActive(false);
                    timelineTab.SetActive(false);
                    break;
                default:
                    Debug.LogError("unexpected tab value :" + toActivateTab);
                    break;
            }

            currentTab = toActivateTab;
            //            segueController.dragAndClose.Init();

        }
    }
}