using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data.BattleUnit
{
    [CreateAssetMenu(fileName = "FeatureClass-", menuName = "BattleUnit/FeatureClass", order = 18)]
    public sealed class FeatureClass : ScriptableObject
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
            absorbShieldRatioCurrent = absorbShieldInitial; absorbShieldRatioInitial = absorbShieldInitial;
            absorbShieldMaxRatioCurrent = absorbShieldInitial * 3.0; absorbShieldMaxRatioInitial = absorbShieldInitial * 3.0;
            this.damageControlAssist = damageControlAssist;
            this.hateInitial = hateInitial; hateCurrent = hateInitial; this.hateMagnificationPerTurn = hateMagnificationPerTurn;
        }

        public void Set(double iAbsorbShieldInitial, bool iDamageControlAssist, double iHateInitial, double iHateMagnificationPerTurn)
        {
            absorbShieldRatioCurrent = iAbsorbShieldInitial;
            absorbShieldRatioInitial = iAbsorbShieldInitial;
            absorbShieldMaxRatioCurrent = iAbsorbShieldInitial * 3.0; 
            absorbShieldMaxRatioInitial = iAbsorbShieldInitial * 3.0;
            damageControlAssist = iDamageControlAssist;
            hateInitial = iHateInitial; 
            hateCurrent = hateInitial;
            hateMagnificationPerTurn = iHateMagnificationPerTurn;            
        }
        
        
        // absorb level should i call?
        // int absorbLevel  1= (3 * absorbLevel)% of attack and total (9 + 3* absorbLevel)% of max shield heal  etc...
        public void InitializeFeature() { hateCurrent = hateInitial; }
    }
}