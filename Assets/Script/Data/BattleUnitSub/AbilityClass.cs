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
}