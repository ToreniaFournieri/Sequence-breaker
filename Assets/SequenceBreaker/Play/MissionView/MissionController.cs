using System.Collections.Generic;
using SequenceBreaker.GUIController;
using SequenceBreaker.Home.EquipView;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.UnitClass;
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

        // inventory
        public InventoryItemList inventoryItemList;

        public InventoryTreeViewDataSourceMgr inventoryTreeViewDataSourceMgr;

        //[Obsolete]public InventoryManager inventoryManager;
        public LogListSria logListSria;
        
        // Keep runBattle list
        public LogListDataSourceMgr logListDataSourceMgr;
        
        [FormerlySerializedAs("TransparentMessageController")] public TransparentMessageController transparentMessageController;
        
        

        public void UpdatePartyStatus()
        {
            partyStatusIcons.partyBattleUnitList = allyCurrentBattleUnitList;
            partyStatusIcons.UpdateStatus();
        }

    }
}
