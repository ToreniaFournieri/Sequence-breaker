using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{

    public class TreeViewItemData
    {
        public string MName;
        public string MIcon;
        List<ItemData> _mChildItemDataList = new List<ItemData>();

        public int ChildCount
        {
            get { return _mChildItemDataList.Count; }
        }

        public void AddChild(ItemData data)
        {
            _mChildItemDataList.Add(data);
        }
        public ItemData GetChild(int index)
        {
            if(index < 0 || index >= _mChildItemDataList.Count)
            {
                return null;
            }
            return _mChildItemDataList[index];
        }
    }

    public class TreeViewDataSourceMgr : MonoBehaviour
    {

        List<TreeViewItemData> _mItemDataList = new List<TreeViewItemData>();

        static TreeViewDataSourceMgr _instance = null;
        int _mTreeViewItemCount = 20;
        int _mTreeViewChildItemCount = 30;

        public static TreeViewDataSourceMgr Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Object.FindObjectOfType<TreeViewDataSourceMgr>();
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

        public TreeViewItemData GetItemDataByIndex(int index)
        {
            if (index < 0 || index >= _mItemDataList.Count)
            {
                return null;
            }
            return _mItemDataList[index];
        }

        public ItemData GetItemChildDataByIndex(int itemIndex,int childIndex)
        {
            TreeViewItemData data = GetItemDataByIndex(itemIndex);
            if(data == null)
            {
                return null;
            }
            return data.GetChild(childIndex);
        }

        public int TreeViewItemCount
        {
            get
            {
                return _mItemDataList.Count;
            }
        }

        public int TotalTreeViewItemAndChildCount
        {
            get
            {
                int count =  _mItemDataList.Count;
                int totalCount = 0;
                for(int i = 0;i<count;++i)
                {
                    totalCount = totalCount + _mItemDataList[i].ChildCount;
                }
                return totalCount;
            }
        }


        void DoRefreshDataSource()
        {
            _mItemDataList.Clear();
            for (int i = 0; i < _mTreeViewItemCount; ++i)
            {
                TreeViewItemData tData = new TreeViewItemData();
                tData.MName = "Item" + i;
                tData.MIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                _mItemDataList.Add(tData);
                int childCount = _mTreeViewChildItemCount;
                for (int j = 1;j <= childCount;++j)
                {
                    ItemData childItemData = new ItemData();
                    childItemData.MName = "Item" + i + ":Child" + j;
                    childItemData.MDesc = "Item Desc For " + childItemData.MName;
                    childItemData.MIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                    childItemData.MStarCount = Random.Range(0, 6);
                    childItemData.MFileSize = Random.Range(20, 999);
                    tData.AddChild(childItemData);
                }
            }
        }

      

    }

}