using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerBase-", menuName = "Skill/SubClass/TriggerBaseClass", order = 5)]
public class TriggerBaseClass : ScriptableObject
{
    //public TriggerBaseClass(double possibilityBaseRate, double possibilityWeight, ReferenceStatistics accumulationReference, double accumulationBaseRate, double accumulationWeight)
    //    {
    //        this.PossibilityBaseRate = possibilityBaseRate; this.PossibilityWeight = possibilityWeight; this.AccumulationReference = accumulationReference;
    //        this.AccumulationBaseRate = accumulationBaseRate; this.AccumulationWeight = accumulationWeight;
    //    }

    [SerializeField] public double PossibilityBaseRate;
    [SerializeField] public double PossibilityWeight;
    [SerializeField] public ReferenceStatistics AccumulationReference;
    [SerializeField] public double AccumulationBaseRate;
    [SerializeField] public double AccumulationWeight;
}
