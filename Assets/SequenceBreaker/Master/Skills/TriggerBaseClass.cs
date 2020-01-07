using SequenceBreaker.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Master.Skills
{
    [CreateAssetMenu(fileName = "TriggerBase-", menuName = "Skill/SubClass/TriggerBaseClass", order = 5)]
    public sealed class TriggerBaseClass : ScriptableObject
    {
        [FormerlySerializedAs("PossibilityBaseRate")] [SerializeField] public double possibilityBaseRate;
        [FormerlySerializedAs("PossibilityWeight")] [SerializeField] public double possibilityWeight;
        [FormerlySerializedAs("AccumulationReference")] [SerializeField] public ReferenceStatistics accumulationReference;
        [FormerlySerializedAs("AccumulationBaseRate")] [SerializeField] public double accumulationBaseRate;
        [FormerlySerializedAs("AccumulationWeight")] [SerializeField] public double accumulationWeight;
    }
}
