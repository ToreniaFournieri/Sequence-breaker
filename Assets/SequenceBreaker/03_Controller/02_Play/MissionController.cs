using System.Collections.Generic;
using SequenceBreaker._01_Data;
using SequenceBreaker._01_Data._03_UnitClass;
using SequenceBreaker._03_Controller._01_Home.Inventory;
using SequenceBreaker._03_Controller._03_Log.LogList;
using SequenceBreaker._04_Timeline_Tab.Log;
using SequenceBreaker._10_Global;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._03_Controller._02_Play
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

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {



        }
    }
}
