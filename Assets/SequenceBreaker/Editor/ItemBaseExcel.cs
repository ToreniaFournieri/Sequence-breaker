using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Editor
{
    [Serializable]
    public class ItemBaseExcel
    {
        public string itemName;
        public int itemId;

        public string itemDescription;
        //public string icon;

        //1. CombatBaseValues
        public string combatBaseValue;

        // item level like 2, 3...
        public int level;

        //2. Skill add
        public string skill1;
        public string skill2;
        public string skill3;

        //3. Ability add value
        public string addAbility1;
        public string addAbility2;
        public string addAbility3;

        //4. Offense or defense magnification set
        public string magnific1;
        public string magnific2;
        public string magnific3;
        public string magnific4;
        public string magnific5;
        public string magnific6;


    }

}