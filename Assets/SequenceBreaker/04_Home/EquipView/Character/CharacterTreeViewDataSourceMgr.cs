using System.Collections.Generic;
using SequenceBreaker._01_Data.Items.Item;
using SequenceBreaker._04_Home.EquipView.Inventory;
using UnityEngine;

namespace SequenceBreaker._04_Home.EquipView.Character
{
    public sealed class CharacterTreeViewItemData
    {
        public string MName;
        public string MIcon;

        readonly List<Item> _mChildItemDataList = new List<Item>();
        
        public int ChildCount => _mChildItemDataList.Count;

        public void AddChild(Item data)
        {
            _mChildItemDataList.Add(data);
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

        public CharacterStatusDisplay characterStatusDisplay;

        // other inventory
        public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

        readonly List<CharacterTreeViewItemData> _mItemDataList = new List<CharacterTreeViewItemData>();

        static CharacterTreeViewDataSourceMgr _instance;
        int _mTreeViewItemCount = 1;

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

        public void Show()
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
//            otherInventoryTreeViewDataSourceMgr.allyInventoryItemList.AddItemAndSave(item, 1);
            otherInventoryTreeViewDataSourceMgr.AddItemAndSave(item);

            //remove from other inventory
            characterStatusDisplay.RemoveAndSaveItem(item);
            

            characterStatusDisplay.RefreshCharacterStatusAndItemList();
            otherInventoryTreeViewDataSourceMgr.DoRefreshDataSource();

        }


        // this method is called from CharacterStatus Display . SetCharacterStatus
        private void DoRefreshDataSource()
        {
            var itemList = characterStatusDisplay.GetItemList();
            if (itemList != null && itemList.Count != 0 && itemList[0] != null)
            {
                itemList.Sort((x, y) => (x.baseItem.itemId - y.baseItem.itemId));
            }

            _mItemDataList.Clear();
            for (int i = 0; i < _mTreeViewItemCount; ++i)
            {
                CharacterTreeViewItemData tData = new CharacterTreeViewItemData {MName = "Character", MIcon = "1"};
                _mItemDataList.Add(tData);


                if (itemList != null)
                    foreach (Item item in itemList)
                    {
                        if (item != null)
                        {
                            tData.AddChild(item);
                        }
                    }
            }


        }



    }
}