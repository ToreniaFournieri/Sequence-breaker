using System;
using System.Collections.Generic;
using SequenceBreaker._01_Data._03_UnitClass;
using UnityEngine;


namespace SequenceBreaker._03_Controller._01_Home
{

    [Serializable]
    public class HomeContentData
    {
        [SerializeField] public string contentText;
        public string description;
        public List<UnitClass> unitClassList;
        public string inventorySavedFileName;
        
        //for SuperScroll use
        public int mId;
        public bool mChecked;
        public bool mIsExpand;
    }
    
    [Serializable]
    public class HomeContents : MonoBehaviour
    {

//        public GameObject jumpLinkGameObject;

        [SerializeField]public List<HomeContentData> homeContentList;
        
        
    }



}
