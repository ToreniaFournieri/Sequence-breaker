using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data.Skills.Subclass
{
    [CreateAssetMenu(fileName = "triggerTarget", menuName = "Skill/SubClass/TriggerTargetClass", order = 6)]
    public sealed class TriggerTargetClass : ScriptableObject
    {


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
}
