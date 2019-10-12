using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GoScript : MonoBehaviour
{
    public Slider levelOfMissionSlider;
    //public GameObject battle;
    public RunBattle runBattle;

    // for mission controller
    public MissionController missionController;

    public void GoBattle()
    {
        runBattle.GetComponent<RunBattle>().Run((int)levelOfMissionSlider.value, missionController.allyUnitList);

        //Not works well
        RunBattle _runbattle = runBattle;
        //get Mission name
        string missionName = _runbattle.missionText;
        string missionLevel = " (lv:" + _runbattle.missionLevel.ToString() + ")";

        List<GameObject> _battleCopyList = new List<GameObject>();


        int _wave = 0;
        foreach (List<KohmaiWorks.Scroller.Data> _data in _runbattle.DataList)
        {
            _battleCopyList.Add(new GameObject());
            _battleCopyList[_wave].transform.parent = runBattle.transform;
            _battleCopyList[_wave].name = runBattle.name + " log:" + DateTime.Now;
            _battleCopyList[_wave].gameObject.AddComponent<RunBattle>();

            RunBattle _localRunBattle = _runbattle.Copy(_wave);

            _battleCopyList[_wave].GetComponent<RunBattle>().Set(_localRunBattle);

            _battleCopyList[_wave].GetComponent<RunBattle>().missionText += " [wave:" + (_wave + 1) + "]";
            missionController.logList.battleList.Add(_battleCopyList[_wave]);
            missionController.logList.ChangeModelsAndReset(missionController.logList.battleList.Count + 1 - 1);

            missionController.allyCurrentBattleUnitList = _localRunBattle.currentAllyUnitList;
            missionController.UpdatePartyStatus();

            _wave += 1;

        }


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
                    _experience = (int)(_experience / missionController.allyUnitList.Count);

                    foreach (UnitClass _allyUnit in missionController.allyUnitList)
                    {
                        _allyUnit.GainExperience(_experience);
                    }

                }

            }

            missionController.TransparentMessageController.transparentText.text += "\n " + "Mission: " + missionName + missionLevel
                + " wave:" + (_wave + 1) + " [" + _battleCopy.GetComponent<RunBattle>().whichWinEachWaves[_wave] + "] ";


            _wave += 1;
        }


        //Item copyedItem = new Item();
        //GameObject itemObject = new GameObject();
        //itemObject.gameObject.AddComponent<DropedItem>();
        //itemObject.transform.parent = missionController.inventoryManager.transform;

        //DropedItem dropedItem = itemObject.gameObject.GetComponent<DropedItem>();

        foreach (Item item in itemList)
        {
            //copyedItem = null;
            //copyedItem = item.Copy();
            //dropedItem.item = null;
            //dropedItem.SetItem(item);
            missionController.TransparentMessageController.transparentText.text += "\n " + "[P1] " + item.itemName;
            missionController.inventoryManager.inventoryScrollList.AddItem(item.Copy());
            //Debug.Log(" Droped:" + item.itemName);

        }
        missionController.inventoryManager.inventoryScrollList.RefreshDisplay();
        missionController.inventoryManager.inventoryScrollList.Save(missionController.inventoryManager.inventoryScrollList);

        missionController.TransparentMessageController.transparentMessage.SetActive(true);



    }
}
