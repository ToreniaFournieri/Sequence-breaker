using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data.UnitClass;
using UnityEngine;

namespace SequenceBreaker._03_Controller.Home
{

    [Serializable]
    public class HomeContentData
    {
        [SerializeField] public string contentText;
        public string description;
        public List<UnitClass> unitClassList;
        public bool isInfinityInventoryMode;
        
        //for SuperScroll use
        public int mId;
        public bool mChecked;
        public bool mIsExpand;
    }
    
    [Serializable]
    public class HomeContents : MonoBehaviour
    {
        
        [SerializeField]public List<HomeContentData> homeContentList;

    }
    
    



}
