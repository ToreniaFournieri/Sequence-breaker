using UnityEngine;
using UnityEngine.Serialization;

//Offense magnification
namespace SequenceBreaker._01_Data.BattleUnit
{
    [CreateAssetMenu(fileName = "OffenseMagnificationClass-", menuName = "BattleUnit/OffenseMagnificationClass", order = 20)]
    public sealed class OffenseMagnificationClass : MagnificationClass
    {
        public OffenseMagnificationClass(double optimumRangeBonus, double critical, double kinetic, double chemical, double thermal, double vsBeast, double vsCyborg, double vsDrone, double vsRobot, double vsTitan)
            : base(critical, kinetic, chemical, thermal, vsBeast, vsCyborg, vsDrone, vsRobot, vsTitan)
        { this.optimumRangeBonus = optimumRangeBonus; }
        [FormerlySerializedAs("OptimumRangeBonus")] [SerializeField] public double optimumRangeBonus;
    }
}