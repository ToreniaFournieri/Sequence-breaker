using SequenceBreaker._00_System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._02_Items._01_ItemMaster
{
    [CreateAssetMenu(fileName = "AddAbility-", menuName = "Item/AddAbility", order = 7)]
    public sealed class AddAbilityClass : ScriptableObject
    {

        [FormerlySerializedAs("Ability")] [SerializeField] public Ability ability;
        [FormerlySerializedAs("ValueOfAbility")] [SerializeField] public int valueOfAbility;

    }
}
