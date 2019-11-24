using System.Collections.Generic;
using SequenceBreaker.GUIController;
using SequenceBreaker.Home.EquipView;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Mission;
using SequenceBreaker.Master.UnitClass;
using SequenceBreaker.Play.Prepare;
using SequenceBreaker.Timeline;
using SequenceBreaker.Timeline.LogListView;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Play.MissionView
{
    public sealed class MissionController : MonoBehaviour
    {
        // ally unit list
        public List<UnitClass> allyUnitList;
        public List<BattleUnit> allyCurrentBattleUnitList;
        public PartyStatusIcons partyStatusIcons;


        // Mission List
        public List<MissionMaster> missionMasterList;

        // inventory
        public InventoryItemList inventoryItemList;

        public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;

        //[Obsolete]public InventoryManager inventoryManager;
        public LogListSria logListSria;
        
        // Keep runBattle list
        public LogListDataSourceMgr logListDataSourceMgr;
        
        [FormerlySerializedAs("TransparentMessageController")] public TransparentMessageController transparentMessageController;
        
        public List<RunBattle> GetRunBattleList()
        {

            List<RunBattle> runBattlelist = new List<RunBattle>();
          
            foreach (MissionMaster mission in missionMasterList)
            {
                GameObject runBattleObject = new GameObject();
                runBattleObject.transform.parent = this.transform;
                runBattleObject.name = mission.missionName;
                runBattleObject.AddComponent<RunBattle>();
                runBattleObject.GetComponent<RunBattle>().mission = mission;
                runBattlelist.Add(runBattleObject.GetComponent<RunBattle>());
            }

            return runBattlelist;
        }

        public void UpdatePartyStatus()
        {
            partyStatusIcons.partyBattleUnitList = allyCurrentBattleUnitList;
            partyStatusIcons.UpdateStatus();
        }

    }
}
