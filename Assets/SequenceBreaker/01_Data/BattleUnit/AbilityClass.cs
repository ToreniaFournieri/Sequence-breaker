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

        public AbilityClass(AbilityClass abilityClass)
        {
            this.power = abilityClass.power; this.generation = abilityClass.generation; this.stability = abilityClass.stability; this.responsiveness = abilityClass.responsiveness;
            this.precision = abilityClass.precision; this.intelligence = abilityClass.intelligence; this.luck = abilityClass.luck;
        }

        public AbilityClass DeepCopy()
        {
            AbilityClass other = (AbilityClass)this.MemberwiseClone();
            return other;
        }

        public AbilityClass (AbilityClass first, AbilityClass second)
        {
            this.power = first.power + second.power;
            this.generation = first.generation + second.generation;
            this.stability = first.stability + second.stability;
            this.responsiveness = first.responsiveness + second.responsiveness;
            this.precision = first.precision + second.precision;
            this.intelligence = first.intelligence + second.intelligence;
            this.luck = first.luck + second.luck;

        }

        public void AddUp (AbilityClass addAbility)
        {
            this.power = this.power + addAbility.power;
            this.generation = this.generation +  addAbility.generation;
            this.stability = this.stability + addAbility.stability;
            this.responsiveness = this.responsiveness + addAbility.responsiveness;
            this.precision = this.precision + addAbility.precision;
            this.intelligence = this.intelligence + addAbility.intelligence;
            this.luck = this.luck + addAbility.luck;
        }

    }
}
