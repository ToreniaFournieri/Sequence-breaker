using System;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_Controller._01_Home.Inventory.layout
{
    public sealed class InventoryListItemSubTitle : MonoBehaviour
    {
        public Text mText;
        public GameObject mArrow;
        public Button mButton;
        int _mTreeItemIndex = -1;
        Action<int> _mClickHandler;

        public int TreeItemIndex
        {
            get { return _mTreeItemIndex; }
        }

        public void Init()
        {
            mButton.onClick.AddListener(OnButtonClicked);
        }
        public void SetClickCallBack(Action<int> clickHandler)
        {
            _mClickHandler = clickHandler;
        }

        void OnButtonClicked()
        {
            _mClickHandler?.Invoke(_mTreeItemIndex);

        }
        public void SetExpand(bool expand)
        {
            mArrow.transform.localEulerAngles = expand ? new Vector3(0, 0, -90) : new Vector3(0, 0, 90);
        }

        public void SetItemData(int treeItemIndex, bool expand)
        {
            _mTreeItemIndex = treeItemIndex;
            SetExpand(expand);
        }
    }
}

