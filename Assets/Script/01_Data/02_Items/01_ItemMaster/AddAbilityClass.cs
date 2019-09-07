using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddAbility-", menuName = "Item/AddAbility", order = 7)]
public class AddAbilityClass : ScriptableObject
{

    [SerializeField] public Ability Ability;
    [SerializeField] public int ValueOfAbility;

}
