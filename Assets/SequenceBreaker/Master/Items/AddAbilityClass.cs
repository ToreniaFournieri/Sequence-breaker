using SequenceBreaker.Environment;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker.Master.Items
{
    [CreateAssetMenu(fileName = "AddAbility-", menuName = "Item/AddAbility", order = 7)]
    public sealed class AddAbilityClass : ScriptableObject
    {

        [FormerlySerializedAs("Ability")] [SerializeField] public Ability ability;
        [FormerlySerializedAs("ValueOfAbility")] [SerializeField] public int valueOfAbility;

    }
}
