using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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
        BattleEngine battle = new BattleEngine();
        battleLogList = new List<BattleLogClass>();
        battle.Battle();
        battleLogList = battle.logList;
    }

    private void AddList()
    {
        for (int i = 0; i < battleLogList.Count; i++)
        {
            BattleLogClass battleLog = battleLogList[i];
            GameObject newLogButton = buttonObjectPool.GetObject();
            newLogButton.transform.SetParent(contentPanel);

            LogButton logButton = newLogButton.GetComponent<LogButton>();
            logButton.Setup(battleLog);


        }
    }

}
