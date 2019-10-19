using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ActionType ActionType;
    public bool AfterAllMoved;
    public bool Counter;
    public bool Chain;
    public bool ReAttack;
    public bool Heal;
    public bool Move;
    public Range OptimumRange;
    public AttackType MajestyAttackType;
    public CriticalOrNot Critical;
    public ActorOrTargetUnit WhoCrushed;
    public bool OnlyWhenBeenHitMoreThanOnce;
    public bool OnlyWhenAvoidMoreThanOnce;
    
}
