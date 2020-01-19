using System.Collections.Generic;
using SequenceBreaker.GUIController;
using SequenceBreaker.Home.EquipListView;
using SequenceBreaker.Home.WandObsolateEquipListView;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Play.MissionView;
using UnityEngine;
using UnityEngine.UI;

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
        public EquipListDataSourceMgr equipListDataSourceMgr;


        //new Unit list
        public UnitClassList unitClassList;

        //default inventory list
        public ItemPreset initialInventoryItemPreset;

        //wake up all main tab
        public GameObject a1;
        public GameObject a2;
        public GameObject a3;

        public MissionController missionController;

        public TransparentMessageController transparentMessageController;

        // Start is called before the first frame update
        void Start()
        {
            InitializeInventory();
            transparentMessageController.transparentMessage.SetActive(true);

//gameObject.GetComponent<CanvasScaler>().referenceResolution.y / gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight
            //Debug.Log( gameObject.GetComponent<RectTransform>().rect.width + " rect.width");


            //Import ally info
            EquipListContentData allyEquipListContentData;

            if (allyUnitSet != null && allyUnitSet.unitSetList != null)
            {
                allyEquipListContentData = new EquipListContentData();
                allyEquipListContentData.contentText = "Ally Equipment";
                allyEquipListContentData.description = "Equip items for playable characters. ";
                allyEquipListContentData.isInfinityInventoryMode = false;

                //allyEquipListContentData.unitClassList = new List<UnitClass>();
                foreach (UnitWave wave in allyUnitSet.unitSetList)
                {
                    foreach (UnitClass unit in wave.unitWave)
                    {
                        if (unit.affiliation == Affiliation.Ally)
                        {
                            unit.itemList = ItemDataBase.instance.GetItemsFromUniqueId(unit.uniqueId);
                            unit.experience = ItemDataBase.instance.LoadUnitInfo(unit).experience;
                            unit.CalculateLevel();
                        }
                        //allyEquipListContentData.unitClassList.Add(unit);
                    }

                    allyEquipListContentData.unitWave = wave;

                }

                equipListDataSourceMgr.equipListContents.equipListContentList.Add(allyEquipListContentData);
                equipListDataSourceMgr.InsertData(equipListDataSourceMgr.equipListContents.equipListContentList.Count - 1, allyEquipListContentData);

            }


            //// import additional enemy unit to home contents
            EquipListContentData enemyHomeContentData;

            foreach (var mission in missionController.missionList.missionMasterList)
            {
                int waveInt = 1;
                enemyHomeContentData = new EquipListContentData();

                // Get and set only last wave enemy.
                foreach (UnitWave unitWave in mission.unitSet.unitSetList)
                {
                    enemyHomeContentData.contentText = mission.missionName + " (wave: " + waveInt + ")";
                    enemyHomeContentData.description = "[Enemy info]";
                    enemyHomeContentData.isInfinityInventoryMode = true;
                    unitWave.level = mission.levelInitial;

                    foreach (var unit in unitWave.unitWave)
                    {
                        if (unit.uniqueId != 0 && unit != null)
                        {
                            List<Item> itemList = ItemDataBase.instance.GetItemsFromUniqueId(unit.uniqueId);
                            unit.itemList = itemList;
                        }

                    }
                    enemyHomeContentData.unitWave = unitWave;


                    waveInt++;


                }
                equipListDataSourceMgr.equipListContents.equipListContentList.Add(enemyHomeContentData);
                equipListDataSourceMgr.InsertData(equipListDataSourceMgr.equipListContents.equipListContentList.Count - 1, enemyHomeContentData);
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

        public void InitializeInventory()
        {
            inventory = ItemDataBase.instance.LoadUnitInfo(inventory);

            if (inventory.itemList.Count == 0)
            {

                foreach (ItemIdSet itemIdSet in initialInventoryItemPreset.itemIdList)
                {
                    inventory.itemList.Add(ItemDataBase.instance.GetItemFromItemIdSet(itemIdSet));
                }
                ItemDataBase.instance.SaveUnitInfo(inventory);


            }

        }

        // Update is called once per frame
    }
}
