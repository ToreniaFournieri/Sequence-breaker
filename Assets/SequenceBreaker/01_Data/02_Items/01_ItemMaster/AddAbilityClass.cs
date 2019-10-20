using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AddAbility-", menuName = "Item/AddAbility", order = 7)]
sealed public class AddAbilityClass : ScriptableObject
{

    [FormerlySerializedAs("Ability")] [SerializeField] public Ability ability;
    [FormerlySerializedAs("ValueOfAbility")] [SerializeField] public int valueOfAbility;

}
