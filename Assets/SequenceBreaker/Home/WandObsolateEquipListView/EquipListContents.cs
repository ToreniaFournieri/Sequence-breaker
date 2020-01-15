using System;
using System.Collections.Generic;
using SequenceBreaker.Master.Mission;
using UnityEngine;

namespace SequenceBreaker.Home.WandObsolateEquipListView
{

    [Serializable]
    public class EquipListContentData
    {
        [SerializeField] public string contentText;
        public string description;

        public UnitWave unitWave;
        //public List<UnitClass> unitClassList;

        //public int level;
        public bool isInfinityInventoryMode;

        //for SuperScroll use
        public int mId;
        public bool mChecked;
        public bool mIsExpand;
    }

    [Serializable]
    public class EquipListContents : MonoBehaviour
    {

        [SerializeField] public List<EquipListContentData> equipListContentList;

    }





}
