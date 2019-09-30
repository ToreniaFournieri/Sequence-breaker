using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SwitchUnitController : MonoBehaviour
{
    public List<UnitClass> allyList;
    public StatusUpdate statusUpdate;
    public InventoryScrollList inventoryScrollListUnit;

    public void ActivateUnitToDisplay (UnitClass unitToActivate)
    {
        UnitClass unit = allyList.Find((obj) => obj == unitToActivate);

        if (unit != null){
            statusUpdate.unit = unit;
            //Debug.Log("ActivateUnitToDisplay " + unit.Name);
            inventoryScrollListUnit.unit = unit;
            inventoryScrollListUnit.SwitchUnit(unit);
            inventoryScrollListUnit.refreshController.NeedToRefresh = true;
            inventoryScrollListUnit.RefreshDisplay();
            statusUpdate.Refresh();

            //Debug.Log(inventoryScrollListUnit.abilityText.text);
        }

    }

}
