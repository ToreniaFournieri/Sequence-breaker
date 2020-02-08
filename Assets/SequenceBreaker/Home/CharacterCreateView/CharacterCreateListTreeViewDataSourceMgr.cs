using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Units;
using UnityEngine;

namespace SequenceBreaker.Home.CharacterCreate
{
    public sealed class CharacterCreateListTreeViewItemData
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

    public sealed class CharacterCreateListTreeViewDataSourceMgr : MonoBehaviour
    {
        public UnitClass selectedCharacter;

        public CharacterCreateListContents characterCreateListContents;
        //public CharacterStatusDisplay characterStatusDisplay;

        // other inventory
        //public InventoryTreeViewDataSourceMgr otherInventoryTreeViewDataSourceMgr;

        readonly List<CharacterCreateListTreeViewItemData> _mItemDataList = new List<CharacterCreateListTreeViewItemData>();

        static CharacterCreateListTreeViewDataSourceMgr _instance;
        int _mTreeViewItemCount = 2;

        public static CharacterCreateListTreeViewDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<CharacterCreateListTreeViewDataSourceMgr>();
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


        public CharacterCreateListTreeViewItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public UnitClass GetItemChildDataByIndex(int itemIndex, int childIndex)
        {
            CharacterCreateListTreeViewItemData data = GetItemDataByIndex(itemIndex);
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

            CharacterCreateListTreeViewItemData tData;

            _mItemDataList.Clear();


            if (characterCreateListContents.characterCreateListContentList != null)
            {
                int count = 0;
                foreach (CharacterCreateListContentData characterCreateListContentData in characterCreateListContents.characterCreateListContentList)
                {
                    //Debug.Log("data :" + equipListContentData.contentText);


                    tData = new CharacterCreateListTreeViewItemData { MName = characterCreateListContentData.contentText, MIcon = count.ToString()};

                    foreach (UnitClass data in characterCreateListContentData.unitWave.unitWave)
                    {
                        //Debug.Log("data :" + data.shortName);

                        if (characterCreateListContentData != null)
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