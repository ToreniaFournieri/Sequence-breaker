using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "triggerTarget", menuName = "Skill/SubClass/TriggerTargetClass", order = 6)]
sealed public class TriggerTargetClass : ScriptableObject
{

    //public TriggerTargetClass(ActionType actionType, bool afterAllMoved, bool counter, bool chain,
    // bool reAttack, bool heal, bool move, Range optimumRange, AttackType majestyAttackType, CriticalOrNot critical, ActorOrTargetUnit whoCrushed, bool onlyWhenBeenHitMoreThanOnce, bool onlyWhenAvoidMoreThanOnce)
    //{
    //    this.ActionType = actionType; this.AfterAllMoved = afterAllMoved; this.Counter = counter; this.Chain = chain; this.ReAttack = reAttack; this.Heal = Heal;
    //    this.Move = move; this.OptimumRange = optimumRange; this.MajestyAttackType = majestyAttackType; this.Critical = critical; this.WhoCrushed = whoCrushed;
    //    this.OnlyWhenBeenHitMoreThanOnce = onlyWhenBeenHitMoreThanOnce; this.OnlyWhenAvoidMoreThanOnce = onlyWhenAvoidMoreThanOnce;
    //}

    [FormerlySerializedAs("ActionType")] public ActionType actionType;
    [FormerlySerializedAs("AfterAllMoved")] public bool afterAllMoved;
    [FormerlySerializedAs("Counter")] public bool counter;
    [FormerlySerializedAs("Chain")] public bool chain;
    [FormerlySerializedAs("ReAttack")] public bool reAttack;
    [FormerlySerializedAs("Heal")] public bool heal;
    [FormerlySerializedAs("Move")] public bool move;
    [FormerlySerializedAs("OptimumRange")] public Range optimumRange;
    [FormerlySerializedAs("MajestyAttackType")] public AttackType majestyAttackType;
    [FormerlySerializedAs("Critical")] public CriticalOrNot critical;
    [FormerlySerializedAs("WhoCrushed")] public ActorOrTargetUnit whoCrushed;
    [FormerlySerializedAs("OnlyWhenBeenHitMoreThanOnce")] public bool onlyWhenBeenHitMoreThanOnce;
    [FormerlySerializedAs("OnlyWhenAvoidMoreThanOnce")] public bool onlyWhenAvoidMoreThanOnce;
    
}
