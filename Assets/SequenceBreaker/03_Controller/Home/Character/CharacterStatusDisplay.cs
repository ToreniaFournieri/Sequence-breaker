using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._01_Data.UnitClass;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller.Home.Character
{
    public sealed class CharacterStatusDisplay : MonoBehaviour
    {

        public ItemDataBase itemDataBase;
        [FormerlySerializedAs("UnitListScriptable")] public List<UnitClass> unitList;

        public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
        public CharacterTreeViewWithStickyHeadScript characterTreeViewWithStickyHeadScript;


        // character calculation 
        public CalculateUnitStatus calculateUnitStatus;

        //Don't change these following values.
        public Text characterNameText;
        public Text itemAmountText;
        [FormerlySerializedAs("AbilityText")] public Text abilityText;

        //for data save
        public Affiliation affiliation;
        [FormerlySerializedAs("uniqueID")] public int uniqueId;
        public int itemCapacity;
        public int selectedUnitNo = 0;
    

        public void Init()
        {
            RefreshCharacterStatusAndItemList();

        }

        // refresh character status display
        public void RefreshCharacterStatusAndItemList()
        {
            SetCharacterStatus(selectedUnitNo);
        }


        public void SetCharacterStatus(int selectedUnitNo)
        {

            if (unitList != null && selectedUnitNo < unitList.Count)
            {

                // updating
                this.selectedUnitNo = selectedUnitNo;

                characterNameText.text = unitList[this.selectedUnitNo].TrueName();
                                    itemAmountText.text = unitList[this.selectedUnitNo].GetItemAmount() + "/" + unitList[this.selectedUnitNo].itemCapacity;
                                    
                calculateUnitStatus.Init(unitList[this.selectedUnitNo]);
                abilityText.text = calculateUnitStatus.detailAbilityString;

                //for data save
                itemCapacity = unitList[this.selectedUnitNo].itemCapacity;
                affiliation = unitList[this.selectedUnitNo].affiliation;
                uniqueId = unitList[this.selectedUnitNo].uniqueId;

                // load from saved data
                //Bug:This way to load info is not collect.
                UnitClass loadUnit = itemDataBase.LoadUnitInfo(unitList[this.selectedUnitNo]);
                unitList[this.selectedUnitNo].itemList = loadUnit.itemList;
                
                // not load experience point this.

                characterTreeViewDataSourceMgr.Show();
                characterTreeViewWithStickyHeadScript.Initialization();

            }

        }

     

        public bool AddAndSaveItem(Item addItem)
        {
            if (unitList[selectedUnitNo].GetItemAmount() >= unitList[selectedUnitNo].itemCapacity)
            {
                // failed. over.
                return true;
            }
            if (unitList[selectedUnitNo].itemList.Count == 0)
            {
                
                Item item = addItem.Copy();
                item.amount = 1;

                unitList[selectedUnitNo].itemList.Add(item);
            }
            else
            {
                bool onceHasBeenAdded = false;
                for (int i = unitList[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
                {
                    if (unitList[selectedUnitNo].itemList[i].GetId() == addItem.GetId())
                    {
                        onceHasBeenAdded = true;
                        if (unitList[selectedUnitNo].itemList[i].amount >= 99)
                        {
                            //nothing to do.
                        }
                        else
                        {
                            unitList[selectedUnitNo].itemList[i].amount += 1;
                        }

                    }
                }
                if (onceHasBeenAdded == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    unitList[selectedUnitNo].itemList.Add(item);
                }

            }

            itemDataBase.SaveUnitInfo(unitList[selectedUnitNo]);

            return false;

        }

        public void RemoveAndSaveItem(Item removedItem)
        {
            for (int i = unitList[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
            {
                if (unitList[selectedUnitNo].itemList[i].GetId() == removedItem.GetId())
                {
                    if (unitList[selectedUnitNo].itemList[i].amount > 1)
                    {
                        unitList[selectedUnitNo].itemList[i].amount -= 1;
                    }
                    else
                    {
                        unitList[selectedUnitNo].itemList.RemoveAt(i);
                    }
                }
            }

            itemDataBase.SaveUnitInfo(unitList[selectedUnitNo]);


        }


        // for external use
        public List<Item> GetItemList()
        {
            return unitList[selectedUnitNo].itemList;
            
        }


    }
}
