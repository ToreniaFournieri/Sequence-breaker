using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerBase-", menuName = "Skill/SubClass/TriggerBaseClass", order = 5)]
sealed public class TriggerBaseClass : ScriptableObject
{
    [SerializeField] public double PossibilityBaseRate;
    [SerializeField] public double PossibilityWeight;
    [SerializeField] public ReferenceStatistics AccumulationReference;
    [SerializeField] public double AccumulationBaseRate;
    [SerializeField] public double AccumulationWeight;
}
