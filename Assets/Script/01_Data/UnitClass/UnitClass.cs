using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
public class UnitClass : ScriptableObject
{
    [SerializeField] public int UniqueID;
    [SerializeField] public string Name;
    [SerializeField] public Affiliation Affiliation;
    [SerializeField] public UnitType UnitType;
    [SerializeField] public AbilityClass InitialAbility;
    [SerializeField] public int ItemCapacity;

    [SerializeField] public List<Item> itemList;

    public AbilityClass Ability { get; set; }

    [SerializeField] public CoreFrame CoreFrame;
    [SerializeField] public Pilot Pilot;

}
