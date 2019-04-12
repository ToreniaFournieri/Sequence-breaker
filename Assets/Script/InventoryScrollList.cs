using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    public Ability ability;
    public int addAbility;
}

public class InventoryScrollList : MonoBehaviour
{
    public List<Item> itemList;
    public Transform contentPanel;
    public InventoryScrollList otherInventory;
    public RefreshController refreshController;
    //public Text myGoldDisplay;
    public BattleUnit battleUnit;
    //public AbilityClass unitAbility;
    public Text abilityText;
    //public Text MagnificationStatusText;
    public SimpleObjectPool itemObjectPool;
    public float capacity = 10f;

    void Start()
    {


        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        refreshController.NeedToRefresh = true;
        RemovePanels();
        AddPanels();

    }

    private void RemovePanels()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            itemObjectPool.ReturnObject(toRemove);
        }
    }

    public void TryTransferItemToOtherInventory(Item item)
    {
        if (otherInventory.capacity >= 1f)
        {
            capacity += 1;
            otherInventory.capacity -= 1;

            //AbilityClass targetAbility;
            //if (battleUnit != null && battleUnit.Ability != null)
            //{
            //    targetAbility = battleUnit.Ability;
            //    //RemoveStatus(targetAbility, item);
            //}
            //else if (otherInventory.battleUnit != null && otherInventory.battleUnit.Ability != null)
            //{
            //    targetAbility = otherInventory.battleUnit.Ability;
            //    //AddStatus(targetAbility, item);
            //}
            //else { Debug.Log("Non unit to non unit transfer. Is it expected?"); }

            //refreshController.NeedToRefresh = true;

            AddItem(item, otherInventory);
            RemoveItem(item, this);

            RefreshDisplay();
            otherInventory.RefreshDisplay();
        }
    }

    //private void AddStatus(AbilityClass targetAbility, Item item)
    //{

    //    switch (item.ability)
    //    {
    //        case Ability.power:
    //            targetAbility.Power += item.addAbility;
    //            break;
    //        case Ability.stability:
    //            targetAbility.Stability += item.addAbility;
    //            break;
    //        default:
    //            break;
    //    }
    //}

    //private void RemoveStatus(AbilityClass targetAbility, Item item)
    //{

    //    switch (item.ability)
    //    {
    //        case Ability.power:
    //            targetAbility.Power -= item.addAbility;
    //            break;
    //        case Ability.stability:
    //            targetAbility.Stability -= item.addAbility;
    //            break;
    //        default:
    //            break;
    //    }
    //}

    private void AddPanels()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            Item item = itemList[i];
            GameObject newPanel = itemObjectPool.GetObject();
            newPanel.transform.SetParent(contentPanel);

            ItemPanel itemPanel = newPanel.GetComponent<ItemPanel>();
            itemPanel.Setup(item, this);
        }
    }

    private void AddItem(Item itemToAdd, InventoryScrollList inventoryList)
    {
        inventoryList.itemList.Add(itemToAdd);
    }

    private void RemoveItem(Item itemToRemove, InventoryScrollList inventoryList)
    {
        for (int i = inventoryList.itemList.Count - 1; i >= 0; i--)
        {
            if (inventoryList.itemList[i] == itemToRemove)
            {
                inventoryList.itemList.RemoveAt(i);
            }
        }
    }

}