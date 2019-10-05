using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    public GameObject battle;
    public GameObject transparentMessage;

    //for adding battle log to log list
    public LogListSRIA logList;

    // for party status update
    public GameObject partyStatusIcons;

    //for item drop
    public GameObject inventoryManager;

    private List<GameObject> battleForLogList;

    public void GoBattle()
    {
        battle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value);

        //Not works well
        RunBattle _runbattle = battle.GetComponent<RunBattle>();
        //get Mission name
        string missionName = _runbattle.missionText;
        string missionLevel = " (lv:" + _runbattle.missionLevel.ToString() + ")";

        List<GameObject> _battleCopyList = new List<GameObject>();



        int _wave = 0;
        foreach (List<KohmaiWorks.Scroller.Data> _data in _runbattle.DataList)
        {
            _battleCopyList.Add(new GameObject());
            _battleCopyList[_wave].transform.parent = battle.transform;
            _battleCopyList[_wave].name = battle.name + " log:" + DateTime.Now;
            _battleCopyList[_wave].gameObject.AddComponent<RunBattle>();

            RunBattle _localRunBattle = _runbattle.Copy(_wave);

            _battleCopyList[_wave].GetComponent<RunBattle>().Set(_localRunBattle);

            _battleCopyList[_wave].GetComponent<RunBattle>().missionText +=  " [wave:" + (_wave + 1) + "]";
            logList.battleList.Add(_battleCopyList[_wave]);
            logList.ChangeModelsAndReset(logList.battleList.Count + 1 - 1);

            partyStatusIcons.GetComponent<PartyStatusIcons>().partyBattleUnitList = _localRunBattle.currentAllyUnitList;

            _wave += 1;

        }

        partyStatusIcons.GetComponent<PartyStatusIcons>().UpdateStatus();



        // Drop list
        List<Item> itemList = new List<Item>();
        DropEngine dropEngine = new DropEngine();

        _wave = 0;
        foreach (GameObject _battleCopy in _battleCopyList)
        {
            if (_battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.allyWin)
            {
                int seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
                foreach (EnemyUnitSet _enemyUnitSet in _battleCopy.GetComponent<RunBattle>().enemyUnitSetList)
                {
                    List<Item> _itemlist = dropEngine.GetDropedItems(enemyUnitList: _enemyUnitSet.enemyUnitList, seed: seed);

                    foreach (Item _item in _itemlist)
                    {
                        itemList.Add(_item);
                    }

                    // Exp gain, not use copy data! lost reference means worthlesss.
                    int _experience = 0;
                    foreach (UnitClass _enemyUnit in _enemyUnitSet.enemyUnitList)
                    {
                        _experience += _enemyUnit.ExperienceFromBeaten();
                    }

                    // Distribution, not use copied data! lost reference means worthlesss.
                    _experience = (int)(_experience / _runbattle.GetComponent<RunBattle>().allyUnitList.Count);

                    foreach (UnitClass _allyUnit in _runbattle.GetComponent<RunBattle>().allyUnitList)
                    {
                        _allyUnit.GainExperience(_experience);
                    }

                }

            }

            transparentMessage.GetComponentInChildren<Text>().text += "\n " + "Mission: " + missionName + missionLevel
                + " wave:" + (_wave + 1)
    + " [" + _battleCopy.GetComponent<RunBattle>().whichWinEachWaves[_wave] + "] " ;


            _wave += 1;
        }





        foreach (Item item in itemList)
        {

            Item copyedItem = Instantiate(item.Copy());
            GameObject itemObject = new GameObject();
            itemObject.transform.parent = inventoryManager.transform;
            itemObject.name = copyedItem.name + " got:" + DateTime.Now;
            itemObject.gameObject.AddComponent<DropedItem>();
            itemObject.GetComponent<DropedItem>().SetItem(copyedItem);
            transparentMessage.GetComponentInChildren<Text>().text += "\n " + "[P1] " + itemObject.GetComponent<DropedItem>().item.itemName;
            inventoryManager.GetComponent<InventoryManager>().inventoryScrollList.AddItemAndSave(itemObject.GetComponent<DropedItem>().item);
            inventoryManager.GetComponent<InventoryManager>().inventoryScrollList.RefreshDisplay();
        }

        transparentMessage.SetActive(true);


    }
}
