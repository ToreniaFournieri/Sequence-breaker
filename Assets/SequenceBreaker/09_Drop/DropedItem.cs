using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class DropedItem : MonoBehaviour
{
    public Item item;

    public void SetItem (Item item)
    {
        this.item = item;
    }

}
