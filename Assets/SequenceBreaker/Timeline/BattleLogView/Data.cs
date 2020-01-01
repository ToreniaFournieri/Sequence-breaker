using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;

namespace SequenceBreaker.Timeline.BattleLogView
{
//    [CreateAssetMenu(fileName = "Data-", menuName = "Data/data", order = 5)]
    public sealed class Data 
    {
        public int Index;
        public int Turn;
        public string ReactText;
        public string UnitInfo;
        public string FirstLine;
        public string MainText;
        public string BigText; // no ReactText, UnitInfo, FirstLine. only this BigText.

        /// <summary>
        /// We will store the cell size in the model so that the cell view can update it
        /// </summary>
        public float CellSize;
        public int NestLevel;

        public bool IsDead;
        public int BarrierRemains;
        public float ShieldRatio;
        public float HpRatio;

        public string HeaderText;
        public bool IsHeaderInfo;
        public List<BattleUnit> Characters;


        public Affiliation Affiliation;
    }

}