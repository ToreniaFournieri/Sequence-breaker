using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBase-", menuName = "Item/ItemBase", order = 1)]
public class ItemBaseMaster : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite icon;


    //1. Skill add
    [SerializeField] public List<SkillsMasterClass> SkillsMaster;

    //2. Ability add value
    [SerializeField] public AddAbilityClass AddAbility;

    //3. CombatBaseValues
    [SerializeField] public CombatClass CombatBaseValue;

        // Coefficient is like x1.05
    [SerializeField] public double CombatMagnificationCoefficient = 1.0f; 

    //4. Offense or defense magnification set
    [SerializeField] public List<MagnificationMasterClass> MagnificationMaster;


}
