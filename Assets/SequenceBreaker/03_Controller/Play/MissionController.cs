using System.Collections.Generic;
using SequenceBreaker._01_Data.BattleUnit;
using SequenceBreaker._01_Data.UnitClass;
using SequenceBreaker._03_Controller.GUIController;
using SequenceBreaker._03_Controller.Home.Inventory;
using SequenceBreaker._04_Timeline_Tab.Log;
using SequenceBreaker._04_Timeline_Tab.Log.LogList;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._03_Controller.Play
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
