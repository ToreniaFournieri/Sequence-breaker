using System;
using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._02_Play;
using SequenceBreaker._04_Timeline_Tab.Log.BattleLog;
using SequenceBreaker._08_Battle._2_BeforeBattle;
using SequenceBreaker._09_Drop;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._07_Play._1_View
{
    public sealed class GoScript : MonoBehaviour
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
            string missionLevel = " (lv:" + runBattle1.missionLevel + ")";

            List<GameObject> battleCopyList = new List<GameObject>();


            int wave = 0;
            foreach (List<Data> unused in runBattle1.dataList)
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
                        List<Item> itemListLocal = dropEngine.GetDroppedItems(enemyUnitList: enemyUnitSet.enemyUnitList, seed: seed);

                        foreach (Item item in itemListLocal)
                        {
                            itemList.Add(item);
                        }

                        // Exp gain, not use copy data! lost reference means worthless.
                        int experience = 0;
                        foreach (UnitClass enemyUnit in enemyUnitSet.enemyUnitList)
                        {
                            experience += enemyUnit.ExperienceFromBeaten();
                        }

                        // Distribution, not use copied data! lost reference means worthless.
                        experience /= missionController.allyUnitList.Count;

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



            foreach (Item item in itemList)
            {
                missionController.transparentMessageController.transparentText.text += "\n " + "[P1] " + item.ItemName;
            }

            foreach (var item in itemList)
            {
                missionController.inventoryItemList.AddItemAndSave(item);
            }

            missionController.inventoryTreeViewDataSourceMgr.DoRefreshDataSource();

            missionController.transparentMessageController.transparentMessage.SetActive(true);



        }
    }
}
