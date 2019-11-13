using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data;
using SequenceBreaker._01_Data.BattleUnit;
using SequenceBreaker._04_Timeline_Tab;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._08_Battle
{
    [Serializable]
    public sealed class BattleLogClass
    {
//        private readonly bool _isNavigation;
//        private readonly int _importance;
        public BattleLogClass() { } // for null call
        public BattleLogClass(OrderConditionClass orderCondition, OrderClass order, string firstLine, string log, Affiliation whichAffiliationAct)
        {
//            _isNavigation = isNavigation;
//            _importance = importance;
            OrderCondition = orderCondition;
            if (order != null) { Order = order.Copy(); }
            FirstLine = firstLine; Log = log; WhichAffiliationAct = whichAffiliationAct;

            isHeaderInfo = false;
            headerInfoText = null;
        }

        public OrderConditionClass OrderCondition { get; }
        public OrderClass Order { get; }
        public string FirstLine { get; }
        public string Log { get; }
        public Affiliation WhichAffiliationAct { get; }
        [FormerlySerializedAs("Characters")] public List<BattleUnit> characters;

        [FormerlySerializedAs("IsHeaderInfo")] public bool isHeaderInfo;
        [FormerlySerializedAs("HeaderInfoText")] public string headerInfoText;

        //public float cellSize;
    }


    public sealed class BattleLogList : MonoBehaviour
    {
        public List<BattleLogClass> battleLogList;
        public Transform contentPanel;
//        public RefreshController refreshController;

        public SimpleObjectPool buttonObjectPool;

        private void Start()
        {
            BattleRun();
            AddList();
        }

        private void BattleRun()
        {
            var battle = new BattleEngine();
            battleLogList = new List<BattleLogClass>();
            battle.Battle();
            battleLogList = battle.LogList;
        }

        private void AddList()
        {
            foreach (var battleLog in battleLogList)
            {
                var newLogButton = buttonObjectPool.GetObject();
                newLogButton.transform.SetParent(contentPanel);

                var logButton = newLogButton.GetComponent<LogButton>();
                logButton.Setup(battleLog);
            }
        }

    }
}