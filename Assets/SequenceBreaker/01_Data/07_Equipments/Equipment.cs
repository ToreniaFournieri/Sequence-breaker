using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Equip-", menuName = "Equip/Equipment", order = 1)]
sealed public class Equipment : ScriptableObject
{
    [FormerlySerializedAs("EquipmentBaseID")] [SerializeField] public int equipmentBaseId;

    // Base Combat class parameter
    // [TO DO] make an equipment class.

    // Enhancement level -3 ~ 0, +1, +2 ~ +99
    [FormerlySerializedAs("EnhancementLevel")] [SerializeField] public int enhancementLevel;

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
