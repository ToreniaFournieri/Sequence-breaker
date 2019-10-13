using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperScrollView;


public class TreeViewItemDataTEST
{
    public string mName;
    public string mIcon;
    List<ItemData> mChildItemDataList = new List<ItemData>();

    public int ChildCount
    {
        get { return mChildItemDataList.Count; }
    }

    public void AddChild(ItemData data)
    {
        mChildItemDataList.Add(data);
    }
    public ItemData GetChild(int index)
    {
        if (index < 0 || index >= mChildItemDataList.Count)
        {
            return null;
        }
        return mChildItemDataList[index];
    }
}

public class TreeViewDataSourceMgrTEST : MonoBehaviour
{

    List<TreeViewItemDataTEST> mItemDataList = new List<TreeViewItemDataTEST>();

    static TreeViewDataSourceMgrTEST instance = null;
    int mTreeViewItemCount = 20;
    int mTreeViewChildItemCount = 30;

    public static TreeViewDataSourceMgrTEST Get
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindObjectOfType<TreeViewDataSourceMgrTEST>();
            }
            return instance;
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

    public TreeViewItemDataTEST GetItemDataByIndex(int index)
    {
        if (index < 0 || index >= mItemDataList.Count)
        {
            return null;
        }
        return mItemDataList[index];
    }

    public ItemData GetItemChildDataByIndex(int itemIndex, int childIndex)
    {
        TreeViewItemDataTEST data = GetItemDataByIndex(itemIndex);
        if (data == null)
        {
            return null;
        }
        return data.GetChild(childIndex);
    }

    public int TreeViewItemCount
    {
        get
        {
            return mItemDataList.Count;
        }
    }

    public int TotalTreeViewItemAndChildCount
    {
        get
        {
            int count = mItemDataList.Count;
            int totalCount = 0;
            for (int i = 0; i < count; ++i)
            {
                totalCount = totalCount + mItemDataList[i].ChildCount;
            }
            return totalCount;
        }
    }


    void DoRefreshDataSource()
    {
        mItemDataList.Clear();
        for (int i = 0; i < mTreeViewItemCount; ++i)
        {
            TreeViewItemDataTEST tData = new TreeViewItemDataTEST();
            tData.mName = "Item" + i;
            tData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
            mItemDataList.Add(tData);
            int childCount = mTreeViewChildItemCount;
            for (int j = 1; j <= childCount; ++j)
            {
                ItemData childItemData = new ItemData();
                childItemData.mName = "Item" + i + ":Child" + j;
                childItemData.mDesc = "Item Desc For " + childItemData.mName;
                childItemData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
                childItemData.mStarCount = Random.Range(0, 6);
                childItemData.mFileSize = Random.Range(20, 999);
                tData.AddChild(childItemData);
            }
        }
    }



}

