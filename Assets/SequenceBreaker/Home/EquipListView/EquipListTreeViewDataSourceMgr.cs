using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Home.EquipView;
using SequenceBreaker.Home.WandObsolateEquipListView;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Translate;
using UnityEngine;

namespace SequenceBreaker.Home.EquipListView
{
    public sealed class EquipListTreeViewItemData
    {
        public string MName;
        public string RightText;
        public string MIcon;

        List<UnitClass> _mChildUnitClassList = new List<UnitClass>();
        //readonly List<Item> _mChildItemDataList = new List<Item>();

        //public UnitClass selectedCharacter;


        public int ChildCount => _mChildUnitClassList.Count;



        public void AddChild(UnitClass data)
        {
            _mChildUnitClassList.Add(data);
        }

        //public void SortItemList()
        //{
        //    try
        //    {
        //        _mChildUnitClassList.Sort((x, y) => (x.uniqueId - y.uniqueId));
        //    }
        //    catch
        //    {
        //        Debug.LogError("Failed Sort item list: " + MName + " target list count:" + _mChildUnitClassList.Count);

        //    }
        //}

        public UnitClass GetChild(int index)
        {
            if (index < 0 || index >= _mChildUnitClassList.Count)
            {
                return null;
            }
            return _mChildUnitClassList[index];
        }
    }

    public sealed class EquipListTreeViewDataSourceMgr : MonoBehaviour
    {
        public UnitClass selectedCharacter;

        public EquipListContents equipListContents;
        //public CharacterStatusDisplay characterStatusDisplay;

        // other inventory
        //public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

        readonly List<EquipListTreeViewItemData> _mItemDataList = new List<EquipListTreeViewItemData>();

        static EquipListTreeViewDataSourceMgr _instance;
        int _mTreeViewItemCount = 2;

        public static EquipListTreeViewDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<EquipListTreeViewDataSourceMgr>();
                }
                return _instance;
            }

        }

        //void Awake()
        //{
        //    Init();
        //}

        private void Start()
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


        public EquipListTreeViewItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public UnitClass GetItemChildDataByIndex(int itemIndex, int childIndex)
        {
            EquipListTreeViewItemData data = GetItemDataByIndex(itemIndex);
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

        //public void TryTransferItemToOtherInventory(Item item)
        //{
        //    //inventory is infinity

        //    //add item
        //    otherInventoryTreeViewDataSourceMgr.AddItemAndSave(item);

        //    //remove from other inventory
        //    characterStatusDisplay.RemoveAndSaveItem(item);


        //    characterStatusDisplay.RefreshCharacterStatusAndItemList();
        //    otherInventoryTreeViewDataSourceMgr.DoRefreshDataSource();

        //}


        // this method is called from CharacterStatus Display . SetCharacterStatus
        private void DoRefreshDataSource()
        {

            //List<Item> itemList = characterStatusDisplay.GetItemList();

            EquipListTreeViewItemData tData;

            _mItemDataList.Clear();


            if (equipListContents.equipListContentList != null)
            {
                int count = 0;
                foreach (EquipListContentData equipListContentData in equipListContents.equipListContentList)
                {
                    //Debug.Log("data :" + equipListContentData.contentText);


                    tData = new EquipListTreeViewItemData { MName = equipListContentData.contentText, MIcon = count.ToString()};

                    foreach (UnitClass data in equipListContentData.unitWave.unitWave)
                    {
                        //Debug.Log("data :" + data.shortName);

                        if (equipListContentData != null)
                        {
                            tData.AddChild(data);
                        }

                        //tData.SortItemList();

                    }
                    _mItemDataList.Add(tData);

                    count++;

                }
            }
            //}


        }



    }
}