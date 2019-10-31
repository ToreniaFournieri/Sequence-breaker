using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    [CreateAssetMenu(fileName = "CoreFrame-", menuName = "Unit/CoreFrame", order = 3)]
    public sealed class CoreFrame : ScriptableObject
    {
        [FormerlySerializedAs("Name")] [SerializeField] public string name;

        [FormerlySerializedAs("FrameType")] [SerializeField] public FrameType frameType;
        [FormerlySerializedAs("TuningStyle")] [SerializeField] public TuningStyle tuningStyle;

        [FormerlySerializedAs("Shield")] [SerializeField] public int shield;
        [FormerlySerializedAs("HP")] [SerializeField] public int hp;


    }


    public enum FrameType { TwoLegs, SpiderLegs, ReverseJointLegs, Caterpillar, Wheel }
    public enum TuningStyle { None,Commander, Destroyer, Fighter ,Gunner, Jammer,Lancer, Medic, Reconnoiter , Sniper, Tank,  }
}