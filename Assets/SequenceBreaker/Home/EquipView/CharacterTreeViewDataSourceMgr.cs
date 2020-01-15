using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Home.EquipView
{
    public sealed class CharacterTreeViewItemData
    {
        public string MName;
        public string RightText;
        public string MIcon;

        readonly List<Item> _mChildItemDataList = new List<Item>();

        //public UnitClass selectedCharacter;


        public int ChildCount => _mChildItemDataList.Count;



        public void AddChild(Item data)
        {
            _mChildItemDataList.Add(data);
        }
        public void SortItemList()
        {
            try
            {
                _mChildItemDataList.Sort((x, y) => (x.baseItem.itemId - y.baseItem.itemId));
            }
            catch
            {
                Debug.LogError("Failed Sort item list: " + MName + " target list count:" + _mChildItemDataList.Count);

            }
        }

        public Item GetChild(int index)
        {
            if (index < 0 || index >= _mChildItemDataList.Count)
            {
                return null;
            }
            return _mChildItemDataList[index];
        }
    }

    public sealed class CharacterTreeViewDataSourceMgr : MonoBehaviour
    {
        //public UnitClass selectedCharacter;

        public CharacterStatusDisplay characterStatusDisplay;

        public ItemDataBase itemDataBase;

        // other inventory
        public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

        readonly List<CharacterTreeViewItemData> _mItemDataList = new List<CharacterTreeViewItemData>();

        static CharacterTreeViewDataSourceMgr _instance;
        int _mTreeViewItemCount = 2;

        public static CharacterTreeViewDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterTreeViewDataSourceMgr>();
                }
                return _instance;
            }

        }

        void Awake()
        {
            Init();
        }


        public void Init()
        {
            DoRefreshDataSource();

        }


        public CharacterTreeViewItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public Item GetItemChildDataByIndex(int itemIndex, int childIndex)
        {
            CharacterTreeViewItemData data = GetItemDataByIndex(itemIndex);
            return data?.GetChild(childIndex);
        }

        public int TreeViewItemCount => _mItemDataList.Count;

        public int TotalTreeViewItemAndChildCount
        {
            get
            {
                int count = _mItemDataList.Count;
                int totalCount = 0;
                for (int i = 0; i < count; ++i)
                {
                    totalCount = totalCount + _mItemDataList[i].ChildCount;
                }
                return totalCount;
            }
        }

        public void TryTransferItemToOtherInventory(Item item)
        {
            //inventory is infinity

            //add item
            otherInventoryTreeViewDataSourceMgr.AddItemAndSave(item);

            //remove from other inventory
            characterStatusDisplay.RemoveAndSaveItem(item);

            //characterStatusDisplay.characterTreeViewWithStickyHeadScript.Initialization();


            characterStatusDisplay.RefreshCharacterStatusAndItemList();

            otherInventoryTreeViewDataSourceMgr.DoRefreshDataSource();

        }


        // this method is called from CharacterStatus Display . SetCharacterStatus
        private void DoRefreshDataSource()
        {

            //Debug.Log("in DoRefresh ");
            characterStatusDisplay.selectedCharacter.UpdateItemCapacity();

            if (characterStatusDisplay.selectedCharacter.affiliation == Environment.Affiliation.Enemy)
            {
                // do not load
            }
            else
            {
                UnitClass loadUnit = itemDataBase.LoadUnitInfo(characterStatusDisplay.selectedCharacter);
                characterStatusDisplay.selectedCharacter.itemList = loadUnit.itemList;
            }

            List<Item> itemList = characterStatusDisplay.selectedCharacter.itemList;

            CharacterTreeViewItemData tData;

            _mItemDataList.Clear();

            int currentAmount = characterStatusDisplay.selectedCharacter.GetItemAmount();
            if (characterStatusDisplay.selectedCharacter.GetItemAmount() > characterStatusDisplay.selectedCharacter.itemCapacity)
            {
                currentAmount = characterStatusDisplay.selectedCharacter.itemCapacity;
            }

            tData = new CharacterTreeViewItemData
            {
                MName = Word.Get("Status") + "  " + characterStatusDisplay.selectedCharacter.shortName,
                RightText = currentAmount + "/" + characterStatusDisplay.selectedCharacter.itemCapacity,
                MIcon = "0"
            };
            _mItemDataList.Add(tData);

            Item headStatusWithIcon = ScriptableObject.CreateInstance<Item>();
            headStatusWithIcon.amount = -1;
            tData.AddChild(headStatusWithIcon);

            tData = null;
            tData = new CharacterTreeViewItemData { MName = Word.Get("Equiped Item List"), MIcon = "1" };
            _mItemDataList.Add(tData);


            if (itemList != null)
            {
                int _itemCapacityCount = 0;
                foreach (Item item in itemList)
                {
                    if (_itemCapacityCount >= characterStatusDisplay.selectedCharacter.itemCapacity)
                    {
                        continue;
                    }
                    _itemCapacityCount++;

                    if (item != null)
                    {
                        tData.AddChild(item);
                    }
                    
                    tData.SortItemList();
                }
            }



        }



    }
}
