using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedItem : MonoBehaviour
{
    public Item item;

    public void SetItem (Item _item)
    {
        this.item = _item;
    }

}
