using System;
using System.Collections.Generic;
using SequenceBreaker.Drop;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.Units;
using SequenceBreaker.Play.Prepare;
using SequenceBreaker.Timeline.BattleLogView;
using SequenceBreaker.Translate;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.Play.MissionView
{
    public sealed class GoScript : MonoBehaviour
    {
        public Slider levelOfMissionSlider;
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

            }


        }

        public void GoBattle()
        {

            //[1]. [Battle Engine] prepare the battle and battle run

            //[1-1]. Battle: Run
            runBattle.Run((int)levelOfMissionSlider.value, missionController.allyUnitList);


            //[1-2]. Battle log to GameObject.
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

                battleCopyList[wave].GetComponent<RunBattle>().currentMissionName =

                    "[" +Word.Get(battleCopyList[wave].GetComponent<RunBattle>().whichWinEachWaves[wave].ToString()) + "] "
                    + battleCopyList[wave].GetComponent<RunBattle>().mission.missionName
                    + " (" + Word.Get("Xth wave", (wave + 1).ToString(), true) + ")";
                battleCopyList[wave].GetComponent<RunBattle>().currentLevel = runBattle1.currentLevel;

                missionController.allyCurrentBattleUnitList = localRunBattle.currentAllyUnitList;
                missionController.UpdatePartyStatus();

                // temp 2019/11/10
                missionController.logListDataSourceMgr.runBattleList.Add(battleCopyList[wave].GetComponent<RunBattle>());

                wave += 1;
            }

            //[1-3]. Get Mission unitName
            string missionName = runBattle1.mission.missionName;
            string missionLevel = " (lv:" + battleCopyList[0].GetComponent<RunBattle>().currentLevel + ")";




            //[2]. Get Exparience and Drop Item
            List<Item> itemList = new List<Item>();
            DropEngine dropEngine = new DropEngine();

            //[2-1]. Loop only when battle Win. and see transparent message 
            wave = 0;
            foreach (GameObject battleCopy in battleCopyList)
            {
                if (battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.AllyWin)
                {

                    foreach (UnitWave unitWave in battleCopy.GetComponent<RunBattle>().mission.unitSet.unitSetList)
                    {
                        List<Item> itemListLocal = dropEngine.GetDroppedItems(enemyUnitList: unitWave.unitWave);

                        foreach (Item item in itemListLocal)
                        {

                            //[TEMP for demo] Prefix add 0.1 possibility
                            if (0.001f >= UnityEngine.Random.Range(0, 1.0f))
                            {
                                item.prefixItem = ItemDataBase.instance.prefixBaseList.itemBaseMasterList[0];
                            }

                            itemList.Add(item);
                        }

                        //[2-2]. Caracter gains Exp.
                        // not use copy data! lost reference means worthless.
                        int experience = 0;
                        foreach (UnitClass enemyUnit in unitWave.unitWave)
                        {
                            experience += enemyUnit.ExperienceFromBeaten();
                        }

                        // Distribution, not use copied data! lost reference means worthless.
                        experience = (int)Math.Ceiling((double)experience / (double)missionController.allyUnitList.Count);

                        foreach (UnitClass allyUnit in missionController.allyUnitList)
                        {
                            var levelUpAmount = allyUnit.GainExperience(experience);
                            if (levelUpAmount > 0)
                            {

                                missionController.transparentMessageController
                                    .AddTextAndActive("[P1]" + allyUnit.shortName + " +" + levelUpAmount + " " + Word.Get("Level up")
                                    + "! (" + Word.Get("level") + ":" + allyUnit.level + ")", false);
                            }
                        }
                    }
                }

                if (wave + 1 == battleCopyList.Count)
                {
                    missionController.transparentMessageController
                                .AddTextAndActive( Word.Get("Mission") + ": " + missionName + missionLevel
                               + " " + Word.Get("Xth wave", (wave + 1).ToString(), true) + " ["
                               + Word.Get(battleCopy.GetComponent<RunBattle>().whichWinEachWaves[wave].ToString()) + "] ", false);
                }

                wave += 1;
            }

            //[2-3]. Save characters infomation (Experience).
            foreach (UnitClass allyUnit in missionController.allyUnitList)
            {
                ItemDataBase.instance.SaveUnitInfo(allyUnit);
            }

            //[2-3]. Display transparent message of item drop.
            foreach (Item item in itemList)
            {
                bool isBoldRed = false;
                if (item.suffixItem) { isBoldRed = true; }
                if (item.prefixItem) { isBoldRed = true; }

                missionController.transparentMessageController.AddTextAndActive("[P1] " + item.ItemName, isBoldRed);

                //[2-4]. Save Item drop infomation.
                missionController.inventoryItemList.AddItemAndSave(item);
            }



            //[3]. Reflesh data
            //[3-1]. Reflesh the log list view
            missionController.logListDataSourceMgr.Refresh();


            //[3-2]. Reflesh inventory view.
            missionController.inventoryTreeViewDataSourceMgr.DoRefreshDataSource();


        }
    }
}
