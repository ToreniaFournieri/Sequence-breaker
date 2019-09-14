using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace KohmaiWorks.Scroller
{
    [CreateAssetMenu(fileName = "Data-", menuName = "Data/data", order = 5)]
    public class Data : ScriptableObject
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