using System.Collections.Generic;
using SequenceBreaker.Home.HomeListView;
using SequenceBreaker.Master.Items;
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
        //Ally inventory
        public HomeDataSourceMgr homeDataSourceMgr;


        //new Unit list
        public UnitClassList unitClassList;

        //wake up all main tab
        public GameObject a1;
        public GameObject a2;
        public GameObject a3;

        public MissionController missionController;

        // Start is called before the first frame update
        void Start()
        {
            //inventory
            //inventory = itemDataBase.LoadUnitInfo(inventory);
            inventory = ItemDataBase.Get.LoadUnitInfo(inventory);

            // homeContentList 0 is ally list ( this is temp)
            List<UnitClass> unitList = homeDataSourceMgr.homeContents.homeContentList[0].unitClassList;
            foreach (var unit in unitList)
            {
                //unit.experience = itemDataBase.LoadUnitInfo(unit).experience;
                unit.experience = ItemDataBase.Get.LoadUnitInfo(unit).experience;

                unit.CalculateLevel();
            }

            // import additional enemy unit to home contents
            HomeContentData homeContentData;

            foreach (var mission in missionController.missionMasterList)
            {
                int waveInt = 1;

                foreach (var unitSet in mission.unitSet.unitSetList)
                {
                    homeContentData = new HomeContentData();
                    homeContentData.contentText = mission.missionName + " (wave: " + waveInt + ")";
                    homeContentData.description = "[Enemy info]";
                    homeContentData.isInfinityInventoryMode = true;
                    homeContentData.unitClassList = new List<UnitClass>();

                    foreach (var unit in unitSet.unitWave)
                    {
                        //List<ItemIdSet> itemIdSetList = ItemDataBase.Get.itemPresetList.ItemFromId(unit.uniqueId);

                        List<Item> itemList = ItemDataBase.Get.GetItemsFromUniqueId(unit.uniqueId);
                        unit.itemList = itemList;
                        //foreach (var set in itemIdSetList)
                        //{
                        //    Item item = ItemDataBase.Get.GetItemFromId(set.prefixId, set.baseId, set.suffixId, set.enhancedValue);
                        //    if (item != null)
                        //    {
                        //        unit.itemList.Add(item);
                        //    }
                        //}

                        homeContentData.unitClassList.Add(unit);
                    }

                    waveInt++;

                    homeDataSourceMgr.homeContents.homeContentList.Add(homeContentData);
                    homeDataSourceMgr.InsertData(homeDataSourceMgr.homeContents.homeContentList.Count - 1, homeContentData);
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
