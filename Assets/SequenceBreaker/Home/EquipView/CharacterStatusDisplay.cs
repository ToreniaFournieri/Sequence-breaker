using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.UnitClass;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Home.EquipView
{
    public sealed class CharacterStatusDisplay : MonoBehaviour
    {

        public ItemDataBase itemDataBase;
        [FormerlySerializedAs("UnitListScriptable")] public List<UnitClass> unitList;

        public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
        public CharacterTreeViewWithStickyHeadScript characterTreeViewWithStickyHeadScript;


        // character calculation
        //public CalculateUnitStatusMaster calculateUnitStatusMaster;
        public CalculateUnitStatus calculateUnitStatus;

        //Don't change these following values.
        public Text characterNameText;
        public Text itemAmountText;
        [FormerlySerializedAs("AbilityText")] public Text abilityText;

        //for data save
        public int itemCapacity;
        public int selectedUnitNo;
    

        public void Init()
        {
            RefreshCharacterStatusAndItemList();

        }

        // refresh character status display
        public void RefreshCharacterStatusAndItemList()
        {
            SetCharacterStatus(selectedUnitNo);
        }


        public void SetCharacterStatus(int selectedUnitNumber)
        {

            if (unitList != null && selectedUnitNumber < unitList.Count)
            {

                // updating
                this.selectedUnitNo = selectedUnitNumber;

                //Debug.Log(" unit:" + this.selectedUnitNo + " " + unitList.Count);
                //Debug.Log(" unit:" + unitList[this.selectedUnitNo].TrueName() );

                characterNameText.text = unitList[this.selectedUnitNo].TrueName();
                                    itemAmountText.text = unitList[this.selectedUnitNo].GetItemAmount() + "/" + unitList[this.selectedUnitNo].itemCapacity;

                //Debug.Log("id: " + unitList[this.selectedUnitNo].TrueName() + " ");


                calculateUnitStatus.Init(unitList[this.selectedUnitNo]);
                    abilityText.text = calculateUnitStatus.detailAbilityString;
                //calculateUnitStatus.Init(unitList[this.selectedUnitNo]);
                //abilityText.text = calculateUnitStatus.detailAbilityString;

                //for data save
                itemCapacity = unitList[this.selectedUnitNo].itemCapacity;

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
