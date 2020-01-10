using System;
using SequenceBreaker.GUIController.Segue;
using SequenceBreaker.Home.EquipView;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Home.EquipListView
{
    public sealed class EquipListItemContent : MonoBehaviour
    {
        public Text mNameText;
        public Image mIcon;

        public Text mDescriptionText;
        public GameObject mContentRootObj;

        // detail Flag click
        Action<UnitClass> _mClickItemDetailHandler;
        //UnitClass _mClickItemDetailUnitClass;

        // button in detailFlag
        public Button detailFlag;

        //button to select to transfer
        public Button itemContent;

        //ItemList Character
        public bool isCharacterInventory;
        public EquipListTreeViewDataSourceMgr equipListTreeViewDataSourceMgr;
        //public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;


        //public Item item;
        public UnitClass unitClass;

        //public UnitClass clickedUnitClass;

        //Go to next segue
        //public GameObject equipView;
        //public CharacterTreeViewDataSourceMgr characterTreeViewDataSourceMgr;
        public CharacterStatusDisplay characterStatusDisplay;

        public void Init()
        {
            detailFlag.onClick.RemoveAllListeners();
            itemContent.onClick.RemoveAllListeners();

            detailFlag.onClick.AddListener(OnButtonClicked);
            itemContent.onClick.AddListener(OnContentClicked);


        }

        //public void SetClickCallBack(Action<UnitClass> clickHandler)
        //{
        //    _mClickItemDetailHandler = clickHandler;
        //}

        public void SetClickCallBack(Action<UnitClass> clickHandler , UnitClass unit)
        {
            _mClickItemDetailHandler = clickHandler;
            unitClass = unit;
            //Debug.Log("Set Click call back is called unit:" + unit.shortName);
            //clickedUnitClass = unit;
        }

        // item detail button clicked
        public void OnButtonClicked()
        {


            if (_mClickItemDetailHandler != null)
            {
                Debug.Log("On _mClickItemDetailHandler clicked " );


                ////Text detailText = equipView.transform.Find("ItemMiddleView/ItemDetailText").GetComponent<Text>();
                ////detailText.text = " detail text is here. " + unitClass.TrueName();
                ////UnitClass obj = new UnitClass();
                ////    _mClickItemDetailHandler.Invoke(obj);
                ////Debug.Log("obj is " + obj.shortName);
                //characterTreeViewDataSourceMgr.selectedCharacter = unitClass;

                //equipView.SetActive(true);
                //equipView.transform.SetAsLastSibling();

            }

        }


        // item content button clicked
        void OnContentClicked()
        {

            Debug.Log("On content clicked unit:" + unitClass.shortName);

            characterStatusDisplay.SetCharacterStatus(unitClass);
            //characterStatusDisplay.characterTreeViewWithStickyHeadScript.mLoopListView.RefreshAllShownItem();
            //characterStatusDisplay.characterTreeViewWithStickyHeadScript.Initialization();
            //characterStatusDisplay.characterTreeViewWithStickyHeadScript.Start();
            //characterTreeViewDataSourceMgr.selectedCharacter = unitClass;
            //CharacterTreeViewDataSourceMgr.Get.selectedCharacter = unitClass;
            //CharacterTreeViewDataSourceMgr.Get.Init();
            //equipView.SetActive(true);
            //equipView.transform.SetAsLastSibling();
            //if (item != null)
            //{

            //    if (isCharacterInventory)
            //    {
            //        equipListTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
            //    }
            //    else
            //    {
            //        inventoryTreeViewDataSourceMgr.TryTransferItemToOtherInventory(item);
            //    }
            //}





        }



        public void SetItemData(UnitClass unitClass, int itemIndex, int childIndex)
        {
            mNameText.text = unitClass.TrueName();

            int currentAmount = unitClass.GetItemAmount();
            if (unitClass.GetItemAmount() > unitClass.itemCapacity)
            {
                currentAmount = unitClass.itemCapacity;
            }

            mDescriptionText.text = "Item:"+ currentAmount + "/" + unitClass.itemCapacity;

            this.unitClass = unitClass;
        }


    }
}

