using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    // ally unit list
    public List<UnitClass> allyUnitList;
    public List<BattleUnit> allyCurrentBattleUnitList;
    public PartyStatusIcons partystatusIcons;
    public InventoryManager inventoryManager;
    public LogListSRIA logList;
    public TransparentMessageController TransparentMessageController;


    public void UpdatePartyStatus()
    {
        partystatusIcons.partyBattleUnitList = allyCurrentBattleUnitList;
        partystatusIcons.UpdateStatus();
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
