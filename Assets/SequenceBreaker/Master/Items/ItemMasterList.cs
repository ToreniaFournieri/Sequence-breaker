using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Master.Skills;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{ 
    [CreateAssetMenu(fileName = "ItemMasterList", menuName = "Item/ItemMasterList", order = 1)]
    public class ItemMasterList : ScriptableObject
    {
        public List<SkillsMasterClass> skillsList;
        public List<AddAbilityClass> abilityList;
        public List<CombatClass> combatList;
        public List<MagnificationMasterClass> magnificationList;

    }
}

