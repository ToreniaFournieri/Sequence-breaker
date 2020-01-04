using System;
using System.Collections.Generic;
using System.Linq;
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


            //int wave = 0;

            int MaxWave = runBattle1.dataList.Max(x => x.Wave);
            //Debug.Log("Max wave is " + MaxWave);

            for (int i = 0; i <= MaxWave; i++)
            {
                battleCopyList.Add(new GameObject());
                battleCopyList[i].transform.parent = runBattle.transform;
                battleCopyList[i].name = runBattle.name + " log:" + DateTime.Now;
                battleCopyList[i].gameObject.AddComponent<RunBattle>();

                RunBattle localRunBattle = runBattle1.Copy(i);


                battleCopyList[i].GetComponent<RunBattle>().Set(localRunBattle);

                battleCopyList[i].GetComponent<RunBattle>().currentMissionName =

                    "[" + Word.Get(battleCopyList[i].GetComponent<RunBattle>().whichWinEachWaves[i].ToString()) + "] "
                    + battleCopyList[i].GetComponent<RunBattle>().mission.missionName
                    + " (" + Word.Get("Xth wave", (i + 1).ToString(), true) + ")";
                battleCopyList[i].GetComponent<RunBattle>().currentLevel = runBattle1.currentLevel;

                missionController.allyCurrentBattleUnitList = localRunBattle.currentAllyUnitList;
                missionController.UpdatePartyStatus();

                // temp 2019/11/10
                missionController.logListDataSourceMgr.runBattleList.Add(battleCopyList[i].GetComponent<RunBattle>());
            }

            //foreach (Data unused in runBattle1.dataList)
            //{
            //    battleCopyList.Add(new GameObject());
            //    battleCopyList[wave].transform.parent = runBattle.transform;
            //    battleCopyList[wave].name = runBattle.name + " log:" + DateTime.Now;
            //    battleCopyList[wave].gameObject.AddComponent<RunBattle>();

            //    RunBattle localRunBattle = runBattle1.Copy(wave);

            //    battleCopyList[wave].GetComponent<RunBattle>().Set(localRunBattle);

            //    battleCopyList[wave].GetComponent<RunBattle>().currentMissionName =

            //        "[" +Word.Get(battleCopyList[wave].GetComponent<RunBattle>().whichWinEachWaves[wave].ToString()) + "] "
            //        + battleCopyList[wave].GetComponent<RunBattle>().mission.missionName
            //        + " (" + Word.Get("Xth wave", (wave + 1).ToString(), true) + ")";
            //    battleCopyList[wave].GetComponent<RunBattle>().currentLevel = runBattle1.currentLevel;

            //    missionController.allyCurrentBattleUnitList = localRunBattle.currentAllyUnitList;
            //    missionController.UpdatePartyStatus();

            //    // temp 2019/11/10
            //    missionController.logListDataSourceMgr.runBattleList.Add(battleCopyList[wave].GetComponent<RunBattle>());

            //    wave += 1;
            //}

            //[1-3]. Get Mission unitName
            string missionName = runBattle1.mission.missionName;
            string missionLevel = " (lv:" + battleCopyList[0].GetComponent<RunBattle>().currentLevel + ")";




            //[2]. Get Exparience and Drop Item
            List<Item> itemList = new List<Item>();
            DropEngine dropEngine = new DropEngine();

            //[2-1]. Loop only when battle Win. and see transparent message 
            //wave = 0;
            foreach (GameObject battleCopy in battleCopyList)
            {
                if (battleCopy.GetComponent<RunBattle>().whichWin == WhichWin.AllyWin)
                {
                    List<(UnitClass unit, int exp, string levelupString)> gainExpList = new List<(UnitClass unit, int exp, string levelupString)>();
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
                            string levelUptext = null;
                            if (levelUpAmount > 0)
                            {
                                levelUptext = " +" + levelUpAmount + " " + Word.Get("Level up")
                                    + "! (" + Word.Get("level") + ":" + allyUnit.level + ")";
                                missionController.transparentMessageController
                                    .AddTextAndActive("[P1]" + allyUnit.shortName + levelUptext, false);
                            }

                            gainExpList.Add(( allyUnit, experience, levelUptext));
                        }
                    }

                    
                    Data _data = new Data();
                    _data.Wave = battleCopy.GetComponent<RunBattle>().waveForSaved;
                    //_data.Wave = battleCopy.GetComponent<RunBattle>().waveForSaved;
                    _data.BigText = "Gain Experience \n";
                    foreach (var info in gainExpList)
                    {
                        _data.BigText += info.unit.shortName + " gets " + info.exp + " experience. \n";
                        if (info.levelupString != null)
                        {
                            _data.BigText += new string(' ', 3) + "And" + info.levelupString + "\n";
                        }
                        _data.BigText += new string(' ', 3) + " To the next level("+ (info.unit.level +1)  +") , need " + info.unit.toNextLevel + " experience. \n";

                    }

                    _data.Index = battleCopy.GetComponent<RunBattle>().dataList.Max(x => x.Index) + 1;
                    _data.Turn = battleCopy.GetComponent<RunBattle>().dataList.Max(x => x.Turn) - 1; //last Turn is dummy
                    _data.IsDead = true;
                    _data.Affiliation = Affiliation.None;
                    battleCopy.GetComponent<RunBattle>().dataList.Add(_data);

                }

                int lastWave = battleCopy.GetComponent<RunBattle>().waveForSaved;
                //Debug.Log(" battleCopyList.Count: " + battleCopyList.Count + " last wave:" + lastWave);
                if (lastWave + 1 == battleCopyList.Count)
                {
                    missionController.transparentMessageController
                                .AddTextAndActive(Word.Get("Mission") + ": " + missionName + missionLevel
                               + " " + Word.Get("Xth wave", (lastWave + 1).ToString(), true) + " ["
                               + Word.Get(battleCopy.GetComponent<RunBattle>().whichWinEachWaves[lastWave].ToString()) + "] ", false);
                }

                //wave += 1;
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
