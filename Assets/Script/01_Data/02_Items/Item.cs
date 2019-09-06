using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item-", menuName = "Item/ItemMaster", order = 10)]
public class Item : ScriptableObject
{
        public string itemName;
        public string itemDescription;
        public Sprite icon;
        public Ability ability;
        public int addAbility;
}
