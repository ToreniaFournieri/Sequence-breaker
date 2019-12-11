using System;
using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.UnitClass;
using UnityEngine;
using UnityEngine.Events;

namespace SequenceBreaker.Home.HomeListView
{
    [Serializable]
    public class HomeListContentData
    {
        [SerializeField] public string contentText;
        public string description;

        public GameObject jumpToGameObject;

        public UnityAction unityAction;


        //public List<UnitClass> unitClassList;
        //public bool isInfinityInventoryMode;

        //for SuperScroll use
        public int mId;
        public bool mChecked;
        public bool mIsExpand;
    }

    [Serializable]
    public class HomeListContents : MonoBehaviour
    {

        [SerializeField] public List<HomeListContentData> equipListContentList;

    }

}
