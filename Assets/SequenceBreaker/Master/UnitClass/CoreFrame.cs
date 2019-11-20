using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    [CreateAssetMenu(fileName = "CoreFrame-", menuName = "Unit/CoreFrame", order = 3)]
    public sealed class CoreFrame : ScriptableObject
    {
        [SerializeField] public new string name;

        [SerializeField] public FrameType frameType;
        [SerializeField] public TuningStyle tuningStyle;

        
        [SerializeField] public int shield;
        [SerializeField] public int hp;


    }


    public enum FrameType { TwoLegs, SpiderLegs, ReverseJointLegs, Caterpillar, Wheel }

    // Note!! change make sure TuningStyleClass, because it is hard cording!
    public enum TuningStyle { None,Commander, Destroyer, Fighter ,Gunner, Jammer,Lancer, Medic, Reconnoiter , Sniper, Tank,  }
}