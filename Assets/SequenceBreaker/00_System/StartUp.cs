using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._01_Data.UnitClass;
using SequenceBreaker._04_Home.HomeListView;
using UnityEngine;

namespace SequenceBreaker._00_System
{
    public sealed class StartUp : MonoBehaviour
    {

        //For inventory management
        public ItemDataBase itemDataBase;
        //Item inventory
        public UnitClass inventory;
        //Ally inventory
//        public HomeContents homeContents;
        public HomeDataSourceMgr homeDataSourceMgr;

        
        //new Unit list
        public UnitMasterList unitMasterList;

        //wake up all main tab
        public GameObject a1;
        public GameObject a2;
        public GameObject a3;


        // Start is called before the first frame update
        void Start()
        {
            //inventory
            inventory = itemDataBase.LoadUnitInfo(inventory);
            
            // homeContentList 0 is ally list ( this is temp)
            List<UnitClass> unitList = homeDataSourceMgr.homeContents.homeContentList[0].unitClassList;
            foreach (var unit in unitList)
            {
                unit.experience = itemDataBase.LoadUnitInfo(unit).experience;
                unit.CalculateLevel();
            }
            
            // import additional enemy unit to home contents
            HomeContentData homeContentData = new HomeContentData();
            homeContentData.contentText = "new enemy list";
            homeContentData.description = "[DEBUG]";
            homeContentData.isInfinityInventoryMode = true;
            homeContentData.unitClassList = new List<UnitClass>();
            foreach (UnitMaster unitMaster in unitMasterList.unitList)
            {
                homeContentData.unitClassList.Add(unitMaster.GetUnitClass());
            }

            homeDataSourceMgr.homeContents.homeContentList.Add(homeContentData);
            homeDataSourceMgr.InsertData(homeDataSourceMgr.homeContents.homeContentList.Count -1,homeContentData);
            
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
