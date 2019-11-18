using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data.BattleUnit;
using SequenceBreaker._01_Data.Items.Item;
using UnityEngine;

namespace SequenceBreaker._01_Data.UnitClass
{
    
    [CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
    public sealed class UnitClass : ScriptableObject
    {
        public int uniqueId;
        public new string name;
        public Affiliation affiliation;
        public UnitType unitType;
        public int itemCapacity;
        public List<Item> itemList;

        public AbilityClass Ability { get; set; }
        public CoreFrame coreFrame;
        public Pilot.Pilot pilot;
        public int level;
        public int experience;
        public int toNextLevel;
         
        private int _levelUpAmount;

        public void Copy(UnitClass unit)
        {
            uniqueId = unit.uniqueId;
            name = unit.name;
            affiliation = unit.affiliation;
            unitType = unit.unitType;
            itemCapacity = unit.itemCapacity;
            itemList = unit.itemList;
            coreFrame = unit.coreFrame;
            pilot = unit.pilot;
            level = unit.level;
            experience = unit.experience;
            toNextLevel = unit.toNextLevel;
        }

        public int GainExperience (int experienceAdd)
        {
            experience += experienceAdd;

            CalculateLevel();

            return _levelUpAmount;

        }

        public string TrueName()
        {
            // Short Name of tuningStyle
            string tuningStyleFirstLetter = coreFrame.tuningStyle.ToString().Substring(0,1);
            
            

                return "[" + tuningStyleFirstLetter + "]"
                       + name 
                       +"[" + unitType + "]"
                       + " (Lv:" + level + ")";
            
        }
        
        
        // don't use itemList.Count. Because item may have multiple amount. use this.
        public int GetItemAmount()
        {
            int totalAmount = 0;
            if (itemList != null)
            {
                foreach (var item in itemList)
                {
                    if (item != null)
                    {
                        totalAmount += item.amount;
                    }
                }
            }

            return totalAmount;
        }

        public void CalculateLevel ()
        {
            int internalLevel = 1;
            int remainder = experience;
            _levelUpAmount = 1 - level;
            while (remainder > 0)
            {

                //Sample implement.
                int step = (int)(Mathf.Pow(1.26f - 0.001f * internalLevel, internalLevel) - 10 + internalLevel * 20) ;
                toNextLevel = step - remainder;
                remainder -= step;

                if (remainder > 0)
                {
                    internalLevel++;
                    _levelUpAmount++;
                }
            
            }

            level = internalLevel;
        }

        public int ExperienceFromBeaten()
        {
            //Sample implement.
            return (int)((Mathf.Pow(1.26f - 0.001f * level, level) + 10 + level * 20 )/30);

        }



    }
}



