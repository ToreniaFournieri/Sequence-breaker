using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BattleLogClass
{
    public BattleLogClass() { } // for null call
    public BattleLogClass(OrderConditionClass orderCondition, bool isNavigation, string log, int importance)
    { OrderCondition = orderCondition; IsNavigation = isNavigation; Importance = importance; Log = log; }

    public OrderConditionClass OrderCondition { get; }
    public bool IsNavigation { get; }
    public int Importance { get; }
    public string Log { get; }
}


public class BattleLogList : MonoBehaviour
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
