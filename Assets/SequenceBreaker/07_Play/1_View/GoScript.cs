using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


sealed public class GoScript : MonoBehaviour
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
        RunBattle runBattle1 = runBattle;
        //get Mission name
        string missionName = runBattle1.missionText;
        string missionLevel = " (lv:" + runBattle1.missionLevel.ToString() + ")";

        List<GameObject> battleCopyList = new List<GameObject>();


        int wave = 0;
        foreach (List<KohmaiWorks.Scroller.Data> data in runBattle1.dataList)
        {
            battleCopyList.Add(new GameObject());
            battleCopyList[wave].transform.parent = runBattle.transform;
            battleCopyList[wave].name = runBattle.name + " log:" + DateTime.Now;
            battleCopyList[wave].gameObject.AddComponent<RunBattle>();

            RunBattle localRunBattle = runBattle1.Copy(wave);

            battleCopyList[wave].GetComponent<RunBattle>().Set(localRunBattle);

            battleCopyList[wave].GetComponent<RunBattle>().missionText += " [wave:" + (wave + 1) + "]";
            missionController.logListSria.battleList.Add(battleCopyList[wave]);
            missionController.logListSria.ChangeModelsAndReset(missionController.logListSria.battleList.Count + 1 - 1);

            missionController.allyCurrentBattleUnitList = localRunBattle.currentAllyUnitList;
            missionController.UpdatePartyStatus();

            wave += 1;

        }


        // Drop list
        List<Item> itemList = new List<Item>();
        DropEngine dropEngine = new DropEngine();

        wave = 0;
        foreach (GameObject battleCopy in battleCopyList)
        {
            if (battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.AllyWin)
            {
                int seed = (int)DateTime.Now.Ticks; // when you find something wrong, use seed value to Reproduction the situation
                foreach (EnemyUnitSet enemyUnitSet in battleCopy.GetComponent<RunBattle>().enemyUnitSetList)
                {
                    List<Item> itemlist = dropEngine.GetDropedItems(enemyUnitList: enemyUnitSet.enemyUnitList, seed: seed);

                    foreach (Item item in itemlist)
                    {
                        itemList.Add(item);
                    }

                    // Exp gain, not use copy data! lost reference means worthlesss.
                    int experience = 0;
                    foreach (UnitClass enemyUnit in enemyUnitSet.enemyUnitList)
                    {
                        experience += enemyUnit.ExperienceFromBeaten();
                    }

                    // Distribution, not use copied data! lost reference means worthlesss.
                    experience = (int)(experience / missionController.allyUnitList.Count);

                    foreach (UnitClass allyUnit in missionController.allyUnitList)
                    {
                        allyUnit.GainExperience(experience);
                    }

                }

            }

            missionController.transparentMessageController.transparentText.text += "\n " + "Mission: " + missionName + missionLevel
                + " wave:" + (wave + 1) + " [" + battleCopy.GetComponent<RunBattle>().whichWinEachWaves[wave] + "] ";


            wave += 1;
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
            missionController.transparentMessageController.transparentText.text += "\n " + "[P1] " + item.ItemName;
            //missionController.inventoryManager.inventoryScrollList.AddItem(item.Copy());
            //Debug.Log(" Droped:" + item.itemName);
        }
        missionController.inventoryItemList.AddItemListAndSave(itemList);
        missionController.inventoryTreeViewDataSourceMgr.DoRefreshDataSource();

        //missionController.inventoryManager.inventoryScrollList.RefreshDisplay();
        //missionController.inventoryManager.inventoryScrollList.Save(missionController.inventoryManager.inventoryScrollList);

        missionController.transparentMessageController.transparentMessage.SetActive(true);



    }
}
