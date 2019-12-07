using System.Collections.Generic;
using SequenceBreaker.GUIController;
using SequenceBreaker.Home.HomeListView;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.MissionView;
using UnityEngine;

namespace SequenceBreaker.Environment
{
    public sealed class StartUp : MonoBehaviour
    {

        ////For inventory management
        //public ItemDataBase itemDataBase;
        //Item inventory
        public UnitClass inventory;

        //Ally set
        public UnitSet allyUnitSet;
        //Ally inventory
        public HomeDataSourceMgr homeDataSourceMgr;


        //new Unit list
        public UnitClassList unitClassList;

        //wake up all main tab
        public GameObject a1;
        public GameObject a2;
        public GameObject a3;

        public MissionController missionController;

        public TransparentMessageController transparentMessageController;

        // Start is called before the first frame update
        void Start()
        {
            //inventory
            //inventory = itemDataBase.LoadUnitInfo(inventory);
            inventory = ItemDataBase.Get.LoadUnitInfo(inventory);


            transparentMessageController.transparentMessage.SetActive(true);


            //Import ally info
            HomeContentData allyHomeContentData;

            if (allyUnitSet != null && allyUnitSet.unitSetList != null)
            {
                allyHomeContentData = new HomeContentData();
                allyHomeContentData.contentText = "Ally Equipment";
                allyHomeContentData.description = "Equip items for playable characters. ";
                allyHomeContentData.isInfinityInventoryMode = false;
                allyHomeContentData.unitClassList = new List<UnitClass>();
                foreach (UnitWave wave in allyUnitSet.unitSetList)
                {
                   foreach (UnitClass unit in wave.unitWave)
                    {
                        unit.itemList = ItemDataBase.Get.GetItemsFromUniqueId(unit.uniqueId);
                        unit.experience = ItemDataBase.Get.LoadUnitInfo(unit).experience;
                        unit.CalculateLevel();
                        allyHomeContentData.unitClassList.Add(unit);
                    }
                }

                homeDataSourceMgr.homeContents.homeContentList.Add(allyHomeContentData);
                homeDataSourceMgr.InsertData(homeDataSourceMgr.homeContents.homeContentList.Count - 1, allyHomeContentData);

            }


            //List<UnitClass> unitList = homeDataSourceMgr.homeContents.homeContentList[0].unitClassList;
            //foreach (var unit in unitList)
            //{
            //    //unit.experience = itemDataBase.LoadUnitInfo(unit).experience;
            //    unit.experience = ItemDataBase.Get.LoadUnitInfo(unit).experience;

            //    unit.CalculateLevel();
            //}


            // import additional enemy unit to home contents
            HomeContentData enemyHomeContentData;

            foreach (var mission in missionController.missionList.missionMasterList)
                //foreach (var mission in missionController.missionMasterList)
            {
                int waveInt = 1;

                foreach (var unitSet in mission.unitSet.unitSetList)
                {
                    enemyHomeContentData = new HomeContentData();
                    enemyHomeContentData.contentText = mission.missionName + " (wave: " + waveInt + ")";
                    enemyHomeContentData.description = "[Enemy info]";
                    enemyHomeContentData.isInfinityInventoryMode = true;
                    enemyHomeContentData.unitClassList = new List<UnitClass>();

                    foreach (var unit in unitSet.unitWave)
                    {
                        //List<ItemIdSet> itemIdSetList = ItemDataBase.Get.itemPresetList.ItemFromId(unit.uniqueId);

                        if (unit.uniqueId != 0 && unit != null)
                        {
                            List<Item> itemList = ItemDataBase.Get.GetItemsFromUniqueId(unit.uniqueId);
                            unit.itemList = itemList;
                        }
                    
                        //foreach (var set in itemIdSetList)
                        //{
                        //    Item item = ItemDataBase.Get.GetItemFromId(set.prefixId, set.baseId, set.suffixId, set.enhancedValue);
                        //    if (item != null)
                        //    {
                        //        unit.itemList.Add(item);
                        //    }
                        //}

                        enemyHomeContentData.unitClassList.Add(unit);
                    }

                    waveInt++;

                    homeDataSourceMgr.homeContents.homeContentList.Add(enemyHomeContentData);
                    homeDataSourceMgr.InsertData(homeDataSourceMgr.homeContents.homeContentList.Count - 1, enemyHomeContentData);
                }

            }


            a1.SetActive(true);
            a2.SetActive(true);
            a3.SetActive(true);

        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
#if UNITY_EDITOR

            // Only Unity Editor on debug
            Debug.unityLogger.logEnabled = true;

#else
                    // others debug off
                    Debug.unityLogger.logEnabled = false;

#endif



        }

        // Update is called once per frame
    }
}
