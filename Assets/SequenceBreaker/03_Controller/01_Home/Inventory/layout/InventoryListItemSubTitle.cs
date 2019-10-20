using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

sealed public class InventoryListItemSubTitle : MonoBehaviour
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
        if (_mClickHandler != null)
        {
            _mClickHandler(_mTreeItemIndex);

            //Debug.Log("mClickHandler is clicked! value: " + mTreeItemIndex);
        }

    }
    public void SetExpand(bool expand)
    {
        if (expand)
        {
            mArrow.transform.localEulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            mArrow.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
    }

    public void SetItemData(int treeItemIndex, bool expand)
    {
        _mTreeItemIndex = treeItemIndex;
        SetExpand(expand);
    }
}

