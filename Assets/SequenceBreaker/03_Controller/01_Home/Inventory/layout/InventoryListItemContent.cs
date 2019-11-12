using System;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._03_Controller._01_Home.Character;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller._01_Home.Inventory.layout
{
    public sealed class InventoryListItemContent : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;

        public Text mDescriptionText;
        public GameObject mContentRootObj;
        
        // detail Flag click
        Action<Item> _mClickItemDetailHandler;
        // button in detailFlag
        public Button detailFlag;

        //button to select to transfer
        public Button itemContent;

        //ItemList Character
        public bool isCharacterInventory;
        public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
        public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;


        public Item item;


        // Show Item detail View
        public GameObject itemDetailView;


        int _mItemDataIndex = -1;
        int _mChildDataIndex = -1;

        public void Init()
        {


            detailFlag.onClick.AddListener(OnButtonClicked);
            itemContent.onClick.AddListener(OnContentClicked);


        }

        public void SetClickCallBack(Action<Item> clickHandler)
        {
            _mClickItemDetailHandler = clickHandler;
        }

        // item detail button clicked
        void OnButtonClicked()
        {

            if (_mClickItemDetailHandler != null)
            {

                // Popup message set
                Text detailText = itemDetailView.transform.Find("ItemMiddleView/ItemDetailText").GetComponent<Text>();
                detailText.text = item.GetItemDetailDescription();
                itemDetailView.SetActive(true);
                itemDetailView.transform.SetAsLastSibling();

            }

        }


        // item content button clicked
        void OnContentClicked()
        {


            if (item != null)
            {

                if (isCharacterInventory)
                {
                    characterTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
                }
                else
                {
                    inventoryTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
                }
            }

        }

        

        public void SetItemData(Item item, int itemIndex, int childIndex)
        {
            _mItemDataIndex = itemIndex;
            _mChildDataIndex = childIndex;
            mNameText.text = item.ItemName;
            mDescriptionText.text = item.ItemDescription;

            this.item = item;
        }


    }
}

