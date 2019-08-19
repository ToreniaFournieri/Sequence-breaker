using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CoreFrame-", menuName = "Unit/CoreFrame", order = 3)]
public class CoreFrame : ScriptableObject
{
    [SerializeField] public string Name;

    [SerializeField] public FrameType FrameType;
    [SerializeField] public TuningStype TuningStype;

    [SerializeField] public int Shield;
    [SerializeField] public int HP;


}


public enum FrameType { twoLegs, spiderLegs, reverseJointLegs, caterpillar, wheel }
public enum TuningStype { none, avoidTank, heavyTank, chargeMelee, swordsMelee, assaultMelee, rifleRenged, sinpeRenged, buff, debuff, assist, medic, supply }



