using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "FeatureClass-", menuName = "BattleUnit/FeatureClass", order = 18)]
sealed public class FeatureClass : ScriptableObject
{
    [FormerlySerializedAs("AbsorbShieldRatioInitial")] [SerializeField] public double absorbShieldRatioInitial;
    [FormerlySerializedAs("AbsorbShieldRatioCurrent")] [SerializeField] public double absorbShieldRatioCurrent;
    [FormerlySerializedAs("AbsorbShieldMaxRatioInitial")] [SerializeField] public double absorbShieldMaxRatioInitial;
    [FormerlySerializedAs("AbsorbShieldMaxRatioCurrent")] [SerializeField] public double absorbShieldMaxRatioCurrent;
    [FormerlySerializedAs("DamageControlAssist")] [SerializeField] public bool damageControlAssist;
    [FormerlySerializedAs("HateInitial")] [SerializeField] public double hateInitial;
    [FormerlySerializedAs("HateCurrent")] [SerializeField] public double hateCurrent;
    [FormerlySerializedAs("HateMagnificationPerTurn")] [SerializeField] public double hateMagnificationPerTurn;

    public FeatureClass(double absorbShieldInitial, bool damageControlAssist, double hateInitial, double hateMagnificationPerTurn)
    {
        this.absorbShieldRatioCurrent = absorbShieldInitial; this.absorbShieldRatioInitial = absorbShieldInitial;
        this.absorbShieldMaxRatioCurrent = absorbShieldInitial * 3.0; this.absorbShieldMaxRatioInitial = absorbShieldInitial * 3.0;
        this.damageControlAssist = damageControlAssist;
        this.hateInitial = hateInitial; this.hateCurrent = hateInitial; this.hateMagnificationPerTurn = hateMagnificationPerTurn;
    }
    // absorb level should i call?
    // int absorbLevel  1= (3 * absorbLevel)% of attack and total (9 + 3* absorbLevel)% of max shield heal  etc...
    public void InitializeFeature() { this.hateCurrent = hateInitial; }
}