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


        // Unit is get using
        // characterTreeViewDataSourceMgr.selectedCharacter



        //public UnitWave unitWave;
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
        //public int selectedUnitNo;


        //public void Init()
        //{
        //    RefreshCharacterStatusAndItemList();

        //}

        // refresh character status display
        public void RefreshCharacterStatusAndItemList()
        {
            SetCharacterStatus(characterTreeViewDataSourceMgr.selectedCharacter);
        }


        public void SetCharacterStatus(UnitClass targetCharacter)
        {
            characterTreeViewDataSourceMgr.selectedCharacter = targetCharacter;

            characterNameText.text = targetCharacter.TrueName();
            targetCharacter.UpdateItemCapacity();


            int _currentItemCount = targetCharacter.GetItemAmount();


            if (targetCharacter.GetItemAmount() > targetCharacter.itemCapacity)
            {
                _currentItemCount = targetCharacter.itemCapacity;
            }

            itemAmountText.text = _currentItemCount + "/" + targetCharacter.itemCapacity;


            //for data save
            itemCapacity = targetCharacter.itemCapacity;

            // load from saved data
            //Bug:This way to load info is not collect

            if (targetCharacter.affiliation == Environment.Affiliation.Enemy)
            {
                // do not load
            }
            else
            {
                UnitClass loadUnit = itemDataBase.LoadUnitInfo(targetCharacter);
                targetCharacter.itemList = loadUnit.itemList;

            }

            //calculateUnitStatus.Init(targetCharacter);
            //abilityText.text = calculateUnitStatus.detailAbilityString;


            // not load experience point this.
            //characterTreeViewDataSourceMgr.selectedCharacter = targetCharacter;
            //characterTreeViewWithStickyHeadScript.Initialization();

            characterTreeViewDataSourceMgr.Init();
            characterTreeViewWithStickyHeadScript.Initialization();


            //characterTreeViewWithStickyHeadScript.mLoopListView.RefreshAllShownItemWithFirstIndex(0);
            //}

        }


        public bool AddAndSaveItem(Item addItem)
        {
            if (characterTreeViewDataSourceMgr.selectedCharacter.GetItemAmount() >= characterTreeViewDataSourceMgr.selectedCharacter.itemCapacity)
            {
                // failed. over.
                return true;
            }
            if (characterTreeViewDataSourceMgr.selectedCharacter.itemList.Count == 0)
            {

                Item item = addItem.Copy();
                item.amount = 1;

                characterTreeViewDataSourceMgr.selectedCharacter.itemList.Add(item);
            }
            else
            {
                bool onceHasBeenAdded = false;
                for (int i = characterTreeViewDataSourceMgr.selectedCharacter.itemList.Count - 1; i >= 0; i--)
                {
                    if (characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].GetId() == addItem.GetId())
                    {
                        onceHasBeenAdded = true;
                        if (characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].amount >= 99)
                        {
                            //nothing to do.
                        }
                        else
                        {
                            characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].amount += 1;
                        }

                    }
                }
                if (onceHasBeenAdded == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    characterTreeViewDataSourceMgr.selectedCharacter.itemList.Add(item);
                }

            }

            itemDataBase.SaveUnitInfo(characterTreeViewDataSourceMgr.selectedCharacter);

            return false;

        }

        public void RemoveAndSaveItem(Item removedItem)
        {
            for (int i = characterTreeViewDataSourceMgr.selectedCharacter.itemList.Count - 1; i >= 0; i--)
            {
                if (characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].GetId() == removedItem.GetId())
                {
                    if (characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].amount > 1)
                    {
                        characterTreeViewDataSourceMgr.selectedCharacter.itemList[i].amount -= 1;
                    }
                    else
                    {
                        characterTreeViewDataSourceMgr.selectedCharacter.itemList.RemoveAt(i);
                    }
                }
            }

            itemDataBase.SaveUnitInfo(characterTreeViewDataSourceMgr.selectedCharacter);


        }


        // for external use

        public List<Item> GetItemList()
        {
            return characterTreeViewDataSourceMgr.selectedCharacter.itemList;

        }
        //public List<Item> GetItemList()
        //{
        //    return unitWave.unitWave[selectedUnitNo].itemList;

        //}


    }
}
