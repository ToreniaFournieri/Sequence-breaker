using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._00_Global;
using SequenceBreaker._03_Controller._01_Home;
using SequenceBreaker._03_Controller._02_Play;
using SequenceBreaker._08_Battle._2_BeforeBattle;
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
        public HomeContents homeContents;


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
            List<UnitClass> unitList = homeContents.homeContentList[0].unitClassList;
            foreach (var unit in unitList)
            {
                unit.experience = itemDataBase.LoadUnitInfo(unit).experience;
                unit.CalculateLevel();
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
