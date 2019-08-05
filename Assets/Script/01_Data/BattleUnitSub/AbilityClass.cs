using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityClass-", menuName = "BattleUnit/AbilityClass", order = 12)]
public class AbilityClass : ScriptableObject
{
    // Range is between 5 to 35
    [SerializeField] public int Power;
    [SerializeField] public int Generation;
    [SerializeField] public int Stability;
    [SerializeField] public int Responsiveness;
    [SerializeField] public int Precision;
    [SerializeField] public int Intelligence;
    [SerializeField] public int Luck;

    public AbilityClass(int power, int generation, int stability, int responsiveness, int precision, int intelligence, int luck)
    {
        this.Power = power; this.Generation = generation; this.Stability = stability; this.Responsiveness = responsiveness;
        this.Precision = precision; this.Intelligence = intelligence; this.Luck = luck;
    }

    public AbilityClass(AbilityClass abilityClass)
    {
        this.Power = abilityClass.Power; this.Generation = abilityClass.Generation; this.Stability = abilityClass.Stability; this.Responsiveness = abilityClass.Responsiveness;
        this.Precision = abilityClass.Precision; this.Intelligence = abilityClass.Intelligence; this.Luck = abilityClass.Luck;
    }

    public AbilityClass DeepCopy()
    {
        AbilityClass other = (AbilityClass)this.MemberwiseClone();
        return other;
    }

    public AbilityClass (AbilityClass First, AbilityClass Second)
    {
        //AbilityClass AddUp = new AbilityClass(0, 0, 0, 0, 0, 0, 0);
        this.Power = First.Power + Second.Power;
        this.Generation = First.Generation + Second.Generation;
        this.Stability = First.Stability + Second.Stability;
        this.Responsiveness = First.Responsiveness + Second.Responsiveness;
        this.Precision = First.Precision + Second.Precision;
        this.Intelligence = First.Intelligence + Second.Intelligence;
        this.Luck = First.Luck + Second.Luck;

    }

    public void AddUp (AbilityClass First, AbilityClass Second)
    {
        this.Power = First.Power + Second.Power;
        this.Generation = First.Generation + Second.Generation;
        this.Stability = First.Stability + Second.Stability;
        this.Responsiveness = First.Responsiveness + Second.Responsiveness;
        this.Precision = First.Precision + Second.Precision;
        this.Intelligence = First.Intelligence + Second.Intelligence;
        this.Luck = First.Luck + Second.Luck;
    }

}
