using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SuperScrollView;
using System;

public class InventoryListItemContent : MonoBehaviour
{
    public Text mNameText;
    public Image mIcon;

    //public Image[] mStarArray;
    //public Text mDescText;
    public Text mDescriptionText;
    //public Color32 mRedStarColor = new Color32(249, 227, 101, 255);
    //public Color32 mGrayStarColor = new Color32(215, 215, 215, 255);
    public GameObject mContentRootObj;

    // Detail button = Detail Flag, handler
    //public GameObject detailFlag;
    // detail Flag click
    Action<Item> mClickItemDetailHandler;
    // button in detailFlag
    public Button detailFlag;

    //button to select to transfer
    Action<Item> mClickItemContentHandler;
    public Button itemContent;

    //ItemList Character
    public bool isCharacterInventory;
    public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
    public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;


    public Item item;


    // Show Item detail View
    public GameObject itemDetailView;


    int mItemDataIndex = -1;
    int mChildDataIndex = -1;

    public void Init()
    {

        //for (int i = 0; i < mStarArray.Length; ++i)
        //{
        //    int index = i;
        //ClickEventListener listener = ClickEventListener.Get(detailFlag.gameObject);
        //listener.SetClickEventHandler(delegate (GameObject obj) { OnButtonClicked(); });
        //}

        detailFlag.onClick.AddListener(OnButtonClicked);
        itemContent.onClick.AddListener(OnContentClicked);


    }

    public void SetClickCallBack(Action<Item> clickHandler)
    {
        mClickItemDetailHandler = clickHandler;
    }

    // item detail button clicked
    void OnButtonClicked()
    {

        if (mClickItemDetailHandler != null)
        {

            // Popup message set
            Text _detailText = itemDetailView.transform.Find("ItemMiddleView/ItemDetailText").GetComponent<Text>();
            _detailText.text = item.GetItemDetailDescription();
            itemDetailView.SetActive(true);
            itemDetailView.transform.SetAsLastSibling();

        }

    }

    public void SetClickContentCallBack(Action<Item> clickHandler)
    {
        mClickItemContentHandler = clickHandler;
    }

    // item content button clicked
    void OnContentClicked()
    {

        if (mClickItemContentHandler != null)
        {
            if (isCharacterInventory)
            {
                characterTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
                Debug.Log("character content is clicked!" + item.itemName);

            }
            else
            {
                inventoryTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
                Debug.Log("inventory content is clicked!" + item.itemName);

            }

        }
    }



    void OnStarClicked(int index)
    {
        ItemData data = TreeViewDataSourceMgr.Get.GetItemChildDataByIndex(mItemDataIndex, mChildDataIndex);
        if (data == null)
        {
            return;
        }
        if (index == 0 && data.mStarCount == 1)
        {
            data.mStarCount = 0;
        }
        else
        {
            data.mStarCount = index + 1;
        }
        //SetStarCount(data.mStarCount);
    }

    //public void SetStarCount(int count)
    //{
    //    int i = 0;
    //    for (; i < count; ++i)
    //    {
    //        mStarArray[i].color = mRedStarColor;
    //    }
    //    for (; i < mStarArray.Length; ++i)
    //    {
    //        mStarArray[i].color = mGrayStarColor;
    //    }
    //}



    public void SetItemData(Item _item, int itemIndex, int childIndex)
    {
        mItemDataIndex = itemIndex;
        mChildDataIndex = childIndex;
        mNameText.text = _item.itemName;
        //mDescText.text = itemData.mFileSize.ToString() + "KB";
        mDescriptionText.text = _item.itemDescription;
        //mIcon.sprite = null;

        this.item = _item;
        //mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
        //SetStarCount(itemData.mStarCount);
    }


}

