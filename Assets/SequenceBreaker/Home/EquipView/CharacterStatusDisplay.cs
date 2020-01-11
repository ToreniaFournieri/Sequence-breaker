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



        public UnitClass selectedCharacter;
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
        //public Text characterNameText;
        //public Text itemAmountText;
        //[FormerlySerializedAs("AbilityText")] public Text abilityText;

        //for data save
        //public int itemCapacity;
        //public int selectedUnitNo;


        //public void Init()
        //{
        //    RefreshCharacterStatusAndItemList();

        //}

        // refresh character status display
        public void RefreshCharacterStatusAndItemList()
        {
            //SetCharacterStatus(characterTreeViewDataSourceMgr.selectedCharacter);
            SetCharacterStatus(selectedCharacter);
        }



        public void SetCharacterStatus(UnitClass targetCharacter)
        {

            //1. set selected character.
            selectedCharacter = targetCharacter;

            //characterTreeViewWithStickyHeadScript.mLoopListView.RefreshAllShownItem();
            Debug.Log("SetCharacter: " + targetCharacter);

            //2. Set Character infomation and each item into the  List<CharacterTreeViewItemData> _mItemDataList.
            characterTreeViewDataSourceMgr.Init();

            //3. Update sticky header infomation.
            //characterTreeViewWithStickyHeadScript.mLoopListView.SetListItemCount(20,false);
            characterTreeViewWithStickyHeadScript.Initialization();
            Debug.Log("SetCharacter after sticky init: " + targetCharacter);

            //4. need to update cells.

            //characterTreeViewWithStickyHeadScript.mLoopListView.RefreshAllShownItemWithFirstIndex(0);




        }


        public bool AddAndSaveItem(Item addItem)
        {
            if (selectedCharacter.GetItemAmount() >= selectedCharacter.itemCapacity)
            {
                // failed. over.
                return true;
            }
            if (selectedCharacter.itemList.Count == 0)
            {

                Item item = addItem.Copy();
                item.amount = 1;

                selectedCharacter.itemList.Add(item);
            }
            else
            {
                bool onceHasBeenAdded = false;
                for (int i = selectedCharacter.itemList.Count - 1; i >= 0; i--)
                {
                    if (selectedCharacter.itemList[i].GetId() == addItem.GetId())
                    {
                        onceHasBeenAdded = true;
                        if (selectedCharacter.itemList[i].amount >= 99)
                        {
                            //nothing to do.
                        }
                        else
                        {
                            selectedCharacter.itemList[i].amount += 1;
                        }

                    }
                }
                if (onceHasBeenAdded == false)
                {
                    Item item = addItem.Copy();
                    item.amount = 1;
                    selectedCharacter.itemList.Add(item);
                }

            }

            itemDataBase.SaveUnitInfo(selectedCharacter);

            return false;

        }

        public void RemoveAndSaveItem(Item removedItem)
        {
            for (int i = selectedCharacter.itemList.Count - 1; i >= 0; i--)
            {
                if (selectedCharacter.itemList[i].GetId() == removedItem.GetId())
                {
                    if (selectedCharacter.itemList[i].amount > 1)
                    {
                        selectedCharacter.itemList[i].amount -= 1;
                    }
                    else
                    {
                        selectedCharacter.itemList.RemoveAt(i);
                    }
                }
            }

            itemDataBase.SaveUnitInfo(selectedCharacter);


        }


        // for external use

        public List<Item> GetItemList()
        {
            return selectedCharacter.itemList;

        }



    }
}
