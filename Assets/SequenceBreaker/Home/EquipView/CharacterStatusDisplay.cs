using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker.Home.EquipView
{
    public sealed class CharacterStatusDisplay : MonoBehaviour
    {

        public ItemDataBase itemDataBase;


        public UnitWave unitWave;
        //[FormerlySerializedAs("UnitListScriptable")] public List<UnitClass> unitList;

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

            if (unitWave.unitWave != null && selectedUnitNumber < unitWave.unitWave.Count)
            {

                // updating
                this.selectedUnitNo = selectedUnitNumber;


                characterNameText.text = unitWave.unitWave[this.selectedUnitNo].TrueName();

                unitWave.unitWave[this.selectedUnitNo].UpdateItemCapacity();

                int _currentItemCount = unitWave.unitWave[this.selectedUnitNo].GetItemAmount();
                if (unitWave.unitWave[this.selectedUnitNo].GetItemAmount() > unitWave.unitWave[this.selectedUnitNo].itemCapacity)
                {
                    _currentItemCount = unitWave.unitWave[this.selectedUnitNo].itemCapacity;
                }

                itemAmountText.text = _currentItemCount + "/" + unitWave.unitWave[this.selectedUnitNo].itemCapacity;


                //for data save
                itemCapacity = unitWave.unitWave[this.selectedUnitNo].itemCapacity;

                // load from saved data
                //Bug:This way to load info is not collect

                if (unitWave.unitWave[this.selectedUnitNo].affiliation == Environment.Affiliation.Enemy)
                {
                    // do not load
                }
                else
                {
                    UnitClass loadUnit = itemDataBase.LoadUnitInfo(unitWave.unitWave[this.selectedUnitNo]);
                    unitWave.unitWave[this.selectedUnitNo].itemList = loadUnit.itemList;

                }

                calculateUnitStatus.Init(unitWave.unitWave[this.selectedUnitNo]);
                abilityText.text = calculateUnitStatus.detailAbilityString;


                // not load experience point this.

                characterTreeViewDataSourceMgr.Show();
                characterTreeViewWithStickyHeadScript.Initialization();

            }

        }



        public bool AddAndSaveItem(Item addItem)
        {
            if (unitWave.unitWave[selectedUnitNo].GetItemAmount() >= unitWave.unitWave[selectedUnitNo].itemCapacity)
            {
                // failed. over.
                return true;
            }
            if (unitWave.unitWave[selectedUnitNo].itemList.Count == 0)
            {

                Item item = addItem.Copy();
                item.amount = 1;

                unitWave.unitWave[selectedUnitNo].itemList.Add(item);
            }
            else
            {
                bool onceHasBeenAdded = false;
                for (int i = unitWave.unitWave[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
                {
                    if (unitWave.unitWave[selectedUnitNo].itemList[i].GetId() == addItem.GetId())
                    {
                        onceHasBeenAdded = true;
                        if (unitWave.unitWave[selectedUnitNo].itemList[i].amount >= 99)
                        {
                            //nothing to do.
                        }
                        else
                        {
                            unitWave.unitWave[selectedUnitNo].itemList[i].amount += 1;
                        }

                    }
                }
                if (onceHasBeenAdded == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    unitWave.unitWave[selectedUnitNo].itemList.Add(item);
                }

            }

            itemDataBase.SaveUnitInfo(unitWave.unitWave[selectedUnitNo]);

            return false;

        }

        public void RemoveAndSaveItem(Item removedItem)
        {
            for (int i = unitWave.unitWave[selectedUnitNo].itemList.Count - 1; i >= 0; i--)
            {
                if (unitWave.unitWave[selectedUnitNo].itemList[i].GetId() == removedItem.GetId())
                {
                    if (unitWave.unitWave[selectedUnitNo].itemList[i].amount > 1)
                    {
                        unitWave.unitWave[selectedUnitNo].itemList[i].amount -= 1;
                    }
                    else
                    {
                        unitWave.unitWave[selectedUnitNo].itemList.RemoveAt(i);
                    }
                }
            }

            itemDataBase.SaveUnitInfo(unitWave.unitWave[selectedUnitNo]);


        }


        // for external use
        public List<Item> GetItemList()
        {
            return unitWave.unitWave[selectedUnitNo].itemList;

        }


    }
}
