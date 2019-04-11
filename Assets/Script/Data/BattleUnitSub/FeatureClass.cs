using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FeatureClass-", menuName = "BattleUnit/FeatureClass", order = 18)]
public class FeatureClass : ScriptableObject
{
    [SerializeField] public double AbsorbShieldRatioInitial;
    [SerializeField] public double AbsorbShieldRatioCurrent;
    [SerializeField] public double AbsorbShieldMaxRatioInitial;
    [SerializeField] public double AbsorbShieldMaxRatioCurrent;
    [SerializeField] public bool DamageControlAssist;
    [SerializeField] public double HateInitial;
    [SerializeField] public double HateCurrent;
    [SerializeField] public double HateMagnificationPerTurn;

    public FeatureClass(double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
    {
        this.AbsorbShieldRatioCurrent = absorbShieldInitial; this.AbsorbShieldRatioInitial = absorbShieldInitial;
        this.AbsorbShieldMaxRatioCurrent = absorbShieldInitial * 3.0; this.AbsorbShieldMaxRatioInitial = absorbShieldInitial * 3.0;
        this.DamageControlAssist = damageControlAssist;
        this.HateInitial = hateInitial; this.HateCurrent = hateInitial; this.HateMagnificationPerTurn = hateMagnificationPerTurn;
    }
    // absorb level should i call?
    // int absorbLevel  1= (3 * absorbLevel)% of attack and total (9 + 3* absorbLevel)% of max shield heal  etc...
    public void InitializeFeature() { this.HateCurrent = HateInitial; }
}