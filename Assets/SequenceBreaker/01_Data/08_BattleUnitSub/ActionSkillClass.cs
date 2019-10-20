using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._08_BattleUnitSub
{
    [CreateAssetMenu(fileName = "ActionSkillClass-", menuName = "BattleUnit/ActionSkillClass", order = 20)]
    public sealed class ActionSkillClass : ScriptableObject
    {
        [FormerlySerializedAs("Move")] [SerializeField] public double move;
        [FormerlySerializedAs("Heal")] [SerializeField] public double heal;
        [FormerlySerializedAs("Counter")] [SerializeField] public double counter;
        [FormerlySerializedAs("Chain")] [SerializeField] public double chain;
        [FormerlySerializedAs("ReAttack")] [SerializeField] public double reAttack;
        [FormerlySerializedAs("Interrupt")] [SerializeField] public double interrupt;
        [FormerlySerializedAs("AtBeginning")] [SerializeField] public double atBeginning;
        [FormerlySerializedAs("AtEnding")] [SerializeField] public double atEnding;
        public ActionSkillClass(double move, double heal, double counter, double chain, double reAttack, double interrupt, double atBeginning, double atEnding)
        {
            this.move = move; this.heal = heal; this.counter = counter; this.chain = chain; this.reAttack = reAttack;
            this.interrupt = interrupt; this.atBeginning = atBeginning; this.atEnding = atEnding;
        }

    }
}