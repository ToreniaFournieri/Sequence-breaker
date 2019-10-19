using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionSkillClass-", menuName = "BattleUnit/ActionSkillClass", order = 20)]
sealed public class ActionSkillClass : ScriptableObject
{
    [SerializeField] public double Move;
    [SerializeField] public double Heal;
    [SerializeField] public double Counter;
    [SerializeField] public double Chain;
    [SerializeField] public double ReAttack;
    [SerializeField] public double Interrupt;
    [SerializeField] public double AtBeginning;
    [SerializeField] public double AtEnding;
    public ActionSkillClass(double move, double heal, double counter, double chain, double reAttack, double interrupt, double atBeginning, double atEnding)
    {
        this.Move = move; this.Heal = heal; this.Counter = counter; this.Chain = chain; this.ReAttack = reAttack;
        this.Interrupt = interrupt; this.AtBeginning = atBeginning; this.AtEnding = atEnding;
    }

}