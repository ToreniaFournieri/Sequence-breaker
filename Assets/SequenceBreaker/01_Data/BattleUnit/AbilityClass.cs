using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data.BattleUnit
{
    [CreateAssetMenu(fileName = "AbilityClass-", menuName = "BattleUnit/AbilityClass", order = 12)]
    public sealed class AbilityClass : ScriptableObject
    {
        // Range is between 5 to 35
        [FormerlySerializedAs("Power")] [SerializeField] public int power;
        [FormerlySerializedAs("Generation")] [SerializeField] public int generation;
        [FormerlySerializedAs("Stability")] [SerializeField] public int stability;
        [FormerlySerializedAs("Responsiveness")] [SerializeField] public int responsiveness;
        [FormerlySerializedAs("Precision")] [SerializeField] public int precision;
        [FormerlySerializedAs("Intelligence")] [SerializeField] public int intelligence;
        [FormerlySerializedAs("Luck")] [SerializeField] public int luck;

        public AbilityClass(int power, int generation, int stability, int responsiveness, int precision, int intelligence, int luck)
        {
            this.power = power; this.generation = generation; this.stability = stability; this.responsiveness = responsiveness;
            this.precision = precision; this.intelligence = intelligence; this.luck = luck;
        }

        public void Set(int iPower, int iGeneration, int iStability, int iResponsiveness, int iPrecision, int iIntelligence, int iLuck)
        {
            this.power = iPower; this.generation = iGeneration; this.stability = iStability; this.responsiveness = iResponsiveness;
            this.precision = iPrecision; this.intelligence = iIntelligence; this.luck = iLuck;
        }
        
        public AbilityClass(AbilityClass abilityClass)
        {
            power = abilityClass.power; generation = abilityClass.generation; stability = abilityClass.stability; responsiveness = abilityClass.responsiveness;
            precision = abilityClass.precision; intelligence = abilityClass.intelligence; luck = abilityClass.luck;
        }

        public AbilityClass DeepCopy()
        {
            AbilityClass other = (AbilityClass)MemberwiseClone();
            return other;
        }

        public AbilityClass (AbilityClass first, AbilityClass second)
        {
            power = first.power + second.power;
            generation = first.generation + second.generation;
            stability = first.stability + second.stability;
            responsiveness = first.responsiveness + second.responsiveness;
            precision = first.precision + second.precision;
            intelligence = first.intelligence + second.intelligence;
            luck = first.luck + second.luck;

        }

        public void AddUp (AbilityClass addAbility)
        {
            power = power + addAbility.power;
            generation = generation +  addAbility.generation;
            stability = stability + addAbility.stability;
            responsiveness = responsiveness + addAbility.responsiveness;
            precision = precision + addAbility.precision;
            intelligence = intelligence + addAbility.intelligence;
            luck = luck + addAbility.luck;
        }

    }
}
