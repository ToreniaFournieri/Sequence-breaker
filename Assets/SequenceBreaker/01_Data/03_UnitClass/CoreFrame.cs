using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CoreFrame-", menuName = "Unit/CoreFrame", order = 3)]
sealed public class CoreFrame : ScriptableObject
{
    [FormerlySerializedAs("Name")] [SerializeField] public string name;

    [FormerlySerializedAs("FrameType")] [SerializeField] public FrameType frameType;
    [FormerlySerializedAs("TuningStype")] [SerializeField] public TuningStype tuningStype;

    [FormerlySerializedAs("Shield")] [SerializeField] public int shield;
    [FormerlySerializedAs("HP")] [SerializeField] public int hp;


}


public enum FrameType { TwoLegs, SpiderLegs, ReverseJointLegs, Caterpillar, Wheel }
public enum TuningStype { None, AvoidTank, HeavyTank, ChargeMelee, SwordsMelee, AssaultMelee, RifleRenged, SinpeRenged, Buff, Debuff, Assist, Medic, Supply }



