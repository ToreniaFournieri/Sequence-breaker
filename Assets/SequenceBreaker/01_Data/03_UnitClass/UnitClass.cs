using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data._01_Skills;
using SequenceBreaker._01_Data._02_Items.Item;
using SequenceBreaker._01_Data._08_BattleUnitSub;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._03_UnitClass
{
    [CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
    public sealed class UnitClass : ScriptableObject
    {
        [SerializeField] public int uniqueId;
        [SerializeField] public string name;
        [SerializeField] public Affiliation affiliation;
        [SerializeField] public UnitType unitType;
        [SerializeField] public int itemCapacity;

        [SerializeField] public List<Item> itemList;

        public AbilityClass Ability { get; set; }
        [SerializeField] public CoreFrame coreFrame;
        [SerializeField] public Pilot.Pilot pilot;
        [SerializeField] public int level;
        [SerializeField] public int experience;
         public int toNextLevel;

        [SerializeField] public List<SkillsMasterClass> skillsMaster;


        public void GainExperience (int experienceAdd)
        {
            experience += experienceAdd;
            CalculateLevel();
        }

        
        // don't use itemList.Count. Because item may have multiple amount. use this.
        public int GetItemAmount()
        {
            int totalAmount = 0;
            foreach (var item in itemList)
            {
                if (item != null)
                {
                    totalAmount += item.amount;
                }
            }

            return totalAmount;
        }

        private void CalculateLevel ()
        {
            int level = 1;
            int remainder = experience;
            while (remainder > 0)
            {

                //Sample implement.
                int step = (int)(Mathf.Pow(1.26f - 0.001f * level, level) + 100 + level * 20) ;
                toNextLevel = step - remainder;
                remainder -= step;

                if (remainder > 0)
                {
                    level++;
                }
            
            }

            level = level;
        }

        public int ExperienceFromBeaten()
        {
            //Sample implement.
            return (int)((Mathf.Pow(1.26f - 0.001f * level, level) + 100 + level * 20 )/30);

        }



    }
}



