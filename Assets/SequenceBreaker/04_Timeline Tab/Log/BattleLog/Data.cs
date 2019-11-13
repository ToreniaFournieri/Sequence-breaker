using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data;
using SequenceBreaker._01_Data.BattleUnit;
using UnityEngine;

namespace SequenceBreaker._04_Timeline_Tab.Log.BattleLog
{
    [CreateAssetMenu(fileName = "Data-", menuName = "Data/data", order = 5)]
    public sealed class Data : ScriptableObject
    {
        public int index;
        public int turn;
        public string reactText;
        public string unitInfo;
        public string firstLine;
        public string mainText;

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it
        /// </summary>
        public float cellSize;
        public int nestLevel;

        public bool isDead;
        public int barrierRemains;
        public float shieldRatio;
        public float hPRatio;

        public string headerText;
        public bool isHeaderInfo;
        public List<BattleUnit> characters;


        public Affiliation affiliation;
    }

}