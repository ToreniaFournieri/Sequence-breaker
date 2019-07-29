using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equip-", menuName = "Equip/Equipment", order = 1)]
public class Equipment : ScriptableObject
{
    [SerializeField] public int EquipmentBaseID;

    // Base Combat class parameter
    // [TO DO] make an equipment class.

    // Enhancement level -3 ~ 0, +1, +2 ~ +99
    [SerializeField] public int EnhancementLevel;

    //Prefix
    // [TO DO] make a Prefix class.

    //Suffix
    // [TO DO] make a Suffix class.

    //Enchantment slots, 0 to 4 (max)
    // [TO DO] make an EnchantmentSlot class.
    // which has fixed slot, random slot or no slot.
    // EnchantSlotA
    // EnchantSlotB
    // EnchantSlotC
    // EnchantSlotD


}
