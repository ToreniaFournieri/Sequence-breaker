using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject battle;
    //public KohmaiWorks.Scroller.BattleLogEnhancedScrollController battleLogEnhancedScrollController; // to pass the log value
    public GameObject transparentMessage;

    //for adding battle log to log list
    public LogListSRIA logList;

    //for item drop
    public GameObject inventoryManager;

    private List<GameObject> battleForLogList;

    public void GoBattle()
    {
        battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);
        //battleLogEnhancedScrollController.Battle = Battle;

        //Not works well
        RunBattle _runbattle = battle.GetComponent<RunBattle>();
        //get Mission name
        string missionName = _runbattle.missionText;
        string missionLevel = " (lv:" + _runbattle.missionLevel.ToString() + ")";


        // copy, set, for log list
        GameObject _battleCopy = new GameObject();
        RunBattle _runBattle = battle.GetComponent<RunBattle>().Copy();
        _battleCopy.transform.parent = battle.transform;
        _battleCopy.name = battle.name + " log:" + DateTime.Now;
        _battleCopy.gameObject.AddComponent<RunBattle>();
        _battleCopy.GetComponent<RunBattle>().Set(_runbattle);
        logList.battleList.Add(_battleCopy);

        logList.ChangeModelsAndReset(logList.battleList.Count + 1 - 1);

        // Drop list
        List<Item> itemList = new List<Item>();
        DropEngine dropEngine = new DropEngine();
        if (_battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.allyWin)
        {
            int seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
            itemList = dropEngine.GetDropedItems(enemyUnitList: _battleCopy.GetComponent<RunBattle>().enemyUnitList, seed: seed);
        }

        transparentMessage.GetComponentInChildren<Text>().text += "\n " + "Mission: " + missionName + missionLevel

        //transparentMessage.GetComponentInChildren<Text>().text += "\n " + "Mission: " + _battleCopy.GetComponent<RunBattle>().missionText + missionLevel
            + " [" + _battleCopy.GetComponent<RunBattle>().whichWin +  "] " + _battleCopy.GetComponent<RunBattle>().winRatio;


        foreach (Item item in itemList)
        {
            //Item copyedItem = (Item)ScriptableObject.CreateInstance<Item>();

            Item copyedItem = Instantiate(item.Copy());
            //copyedItem = item.Copy();
            GameObject itemObject = new GameObject();
            itemObject.transform.parent = inventoryManager.transform;
            itemObject.name =  copyedItem.name + " got:" + DateTime.Now;
            itemObject.gameObject.AddComponent<DropedItem>();
            itemObject.GetComponent<DropedItem>().SetItem(copyedItem);
            transparentMessage.GetComponentInChildren<Text>().text += "\n " + "[P1] " + itemObject.GetComponent<DropedItem>().item.itemName;
            inventoryManager.GetComponent<InventoryManager>().inventoryScrollList.itemList.Add(itemObject.GetComponent<DropedItem>().item);
            inventoryManager.GetComponent<InventoryManager>().inventoryScrollList.RefreshDisplay();
        }

        transparentMessage.SetActive(true);
    }
}
