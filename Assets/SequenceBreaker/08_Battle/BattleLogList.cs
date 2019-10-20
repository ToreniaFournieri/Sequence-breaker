using System;
using System.Collections.Generic;
using SequenceBreaker._08_Battle;
using UnityEngine;

[Serializable]
sealed public class BattleLogClass
{
    public BattleLogClass() { } // for null call
    public BattleLogClass(OrderConditionClass orderCondition, bool isNavigation, OrderClass order, string firstLine, string log, int importance, Affiliation whichAffiliationAct)
    {
        OrderCondition = orderCondition; IsNavigation = isNavigation; Importance = importance;
        if (order != null) { Order = order.Copy(); }
        FirstLine = firstLine; Log = log; WhichAffiliationAct = whichAffiliationAct;

        IsHeaderInfo = false;
        HeaderInfoText = null;
    }

    public OrderConditionClass OrderCondition { get; }
    public bool IsNavigation { get; }
    public int Importance { get; }
    public OrderClass Order { get; }
    public string FirstLine { get; }
    public string Log { get; }
    public Affiliation WhichAffiliationAct { get; }
    public List<BattleUnit> Characters;

    public bool IsHeaderInfo;
    public string HeaderInfoText;

    /// <summary>
    /// We will store the cell size in the model so that the cell view can update it
    /// </summary>
    //public float cellSize;
}


sealed public class BattleLogList : MonoBehaviour
{
    public List<BattleLogClass> battleLogList;
    public Transform contentPanel;
    public RefreshController refreshController;

    public SimpleObjectPool buttonObjectPool;

    private void Start()
    {
        BattleRun();
        AddList();
    }

    private void Update()
    {

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
        for (var i = 0; i < battleLogList.Count; i++)
        {
            var battleLog = battleLogList[i];
            var newLogButton = buttonObjectPool.GetObject();
            newLogButton.transform.SetParent(contentPanel);

            var logButton = newLogButton.GetComponent<LogButton>();
            logButton.Setup(battleLog);


        }
    }

}
