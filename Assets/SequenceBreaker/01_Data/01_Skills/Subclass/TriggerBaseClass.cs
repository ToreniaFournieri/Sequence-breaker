using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TriggerBase-", menuName = "Skill/SubClass/TriggerBaseClass", order = 5)]
sealed public class TriggerBaseClass : ScriptableObject
{
    [FormerlySerializedAs("PossibilityBaseRate")] [SerializeField] public double possibilityBaseRate;
    [FormerlySerializedAs("PossibilityWeight")] [SerializeField] public double possibilityWeight;
    [FormerlySerializedAs("AccumulationReference")] [SerializeField] public ReferenceStatistics accumulationReference;
    [FormerlySerializedAs("AccumulationBaseRate")] [SerializeField] public double accumulationBaseRate;
    [FormerlySerializedAs("AccumulationWeight")] [SerializeField] public double accumulationWeight;
}
