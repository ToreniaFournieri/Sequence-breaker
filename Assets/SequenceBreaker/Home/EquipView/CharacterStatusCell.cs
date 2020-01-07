using System;
using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Home.EquipView
{

    public class CharacterStatusCell : MonoBehaviour
    {

        //character related infomation.
        //public CharacterStatusContent charcterStatusContent;
        public UnitClass unitClass;

        // UI objects.
        public Text bigOneLineText;
        public Image characterIcon;
        public Text upsideDescriptionText;
        public Text detailText;


        public GameObject mContentRootObj;

        //Action<CharacterStatusContent> _mClickCharacterDetailHander;
        Action<UnitClass> _mClickCharacterHander;

        // button in detailFlag
        public Button detailFlag;

        //button to select to transfer
        public Button itemContent;

        //ItemList Character
        public bool isCharacterInventory;
        public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
        public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;


        //public Item item;
        public string characterPopupDescriptionString;

        // Show Item detail View
        public GameObject itemDetailView;

        public void Init()
        {


            detailFlag.onClick.AddListener(OnButtonClicked);
            itemContent.onClick.AddListener(OnContentClicked);


        }


        public void SetClickCallBack(Action<UnitClass> clickHandler)
        {
            _mClickCharacterHander = clickHandler;
        }

        //public void SetClickCallBack(Action<CharacterStatusContent> clickHandler)
        //{
        //    _mClickCharacterDetailHander = clickHandler;
        //}

        // item detail button clicked
        void OnButtonClicked()
        {

            //if (_mClickCharacterDetailHander != null)
            //{

            // Popup message set
            Text detailText = itemDetailView.transform.Find("ItemMiddleView/ItemDetailText").GetComponent<Text>();
            detailText.text = characterPopupDescriptionString;
            itemDetailView.SetActive(true);
            itemDetailView.transform.SetAsLastSibling();

            //}

        }

        // item content button clicked
        void OnContentClicked()
        {

            Debug.Log(" On Content Clicked, CharcterStatus Cell");
            //if (item != null)
            //{

            //    if (isCharacterInventory)
            //    {
            //        characterTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
            //    }
            //    else
            //    {
            //        inventoryTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
            //    }
            //}

        }


        public void SetCharacterStatusData(UnitClass unitClass)
        {

            this.unitClass = unitClass;
            (string bigText, string upsideDescription, string detailString) characterStatusContent = CharacterStatusContent.Get(unitClass);

            bigOneLineText.text = characterStatusContent.bigText;
            upsideDescriptionText.text = characterStatusContent.upsideDescription;
            detailText.text = characterStatusContent.detailString;
            //characterPopupDescriptionString = characterStatusContent.popupDescription;
        }
    }

}