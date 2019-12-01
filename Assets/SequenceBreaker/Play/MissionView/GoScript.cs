using System;
using System.Collections.Generic;
using SequenceBreaker.Drop;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.Prepare;
using SequenceBreaker.Timeline.BattleLogView;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Play.MissionView
{
    public sealed class GoScript : MonoBehaviour
    {
        public Slider levelOfMissionSlider;
        //public GameObject battle;
        public RunBattle runBattle;

        // for mission controller
        public MissionController missionController;

        public void ChangeMissionLevelValue()
        {
            if (runBattle != null)
            {
                foreach (UnitWave unitWave in runBattle.mission.unitSet.unitSetList)
                {
                    foreach (UnitClass unit in unitWave.unitWave)
                    {
                        unit.level = (int)levelOfMissionSlider.value;
                    }
                }


                //foreach (var unitSet in runBattle.enemyUnitSetList)
                //{
                //    foreach (var unit in unitSet.enemyUnitList)
                //    {
                //        unit.level = (int) levelOfMissionSlider.value;
                //    }
                //}
            }


        }
        
        public void GoBattle()
        {
            runBattle.Run((int)levelOfMissionSlider.value, missionController.allyUnitList);

            //Not works well
            RunBattle runBattle1 = runBattle;


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

                battleCopyList[wave].GetComponent<RunBattle>().currentMissionName = battleCopyList[wave].GetComponent<RunBattle>().mission.missionName + " [wave:" + (wave + 1) + "]";
                battleCopyList[wave].GetComponent<RunBattle>().currentLevel = runBattle1.currentLevel ;
                
                missionController.allyCurrentBattleUnitList = localRunBattle.currentAllyUnitList;
                missionController.UpdatePartyStatus();

                // temp 2019/11/10
                missionController.logListDataSourceMgr.runBattleList.Add(battleCopyList[wave].GetComponent<RunBattle>());

                
                wave += 1;

            }

            //get Mission unitName
            string missionName = runBattle1.mission.missionName;
            string missionLevel = " (lv:" + battleCopyList[0].GetComponent<RunBattle>().currentLevel + ")";

            missionController.logListDataSourceMgr.Refresh();
            

            // Drop list
            List<Item> itemList = new List<Item>();
            DropEngine dropEngine = new DropEngine();

            wave = 0;
            foreach (GameObject battleCopy in battleCopyList)
            {
                if (battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.AllyWin)
                {
                    int seed = (int)DateTime.Now.Ticks;
                    System.Random random = new System.Random(seed);
                    // when you find something wrong, use seed value to Reproduction the situation
                    //foreach (EnemyUnitSet enemyUnitSet in battleCopy.GetComponent<RunBattle>().enemyUnitSetList)
                    foreach (UnitWave unitWave in battleCopy.GetComponent<RunBattle>().mission.unitSet.unitSetList)
                    {
                        List<Item> itemListLocal = dropEngine.GetDroppedItems(enemyUnitList: unitWave.unitWave, random: random);

                        foreach (Item item in itemListLocal)
                        {
                            itemList.Add(item);
                        }

                        // Exp gain, not use copy data! lost reference means worthless.
                        int experience = 0;
                        foreach (UnitClass enemyUnit in unitWave.unitWave)
                        {
                            experience += enemyUnit.ExperienceFromBeaten();
                        }



                        // Distribution, not use copied data! lost reference means worthless.
                        experience = (int)Math.Ceiling((double)experience / (double)missionController.allyUnitList.Count);

                        //Debug.Log("experience" + experience);


                        foreach (UnitClass allyUnit in missionController.allyUnitList)
                        {
                            var levelUpAmount = allyUnit.GainExperience(experience);
                            if (levelUpAmount > 0)
                            {
                                missionController
                                        .transparentMessageController
                                        .transparentText.text
                                    += "\n " + "[P1]" + allyUnit.name + " +" + levelUpAmount + " Level up! (level:" + allyUnit.level + ")";

                            }
                        }

                    }

                }

                missionController.transparentMessageController.transparentText.text += "\n " + "Mission: " + missionName + missionLevel
                                                                                       + " wave:" + (wave + 1) + " [" + battleCopy.GetComponent<RunBattle>().whichWinEachWaves[wave] + "] ";
                wave += 1;
            }

            // gain experience
            foreach (UnitClass allyUnit in missionController.allyUnitList)
            {
                ItemDataBase.Get.SaveUnitInfo(allyUnit);
                //missionController.inventoryItemList.itemDataBase.SaveUnitInfo(allyUnit);
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
