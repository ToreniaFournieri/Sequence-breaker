using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
sealed public class UnitClass : ScriptableObject
{
    [FormerlySerializedAs("UniqueID")] [SerializeField] public int uniqueId;
    [FormerlySerializedAs("Name")] [SerializeField] public string name;
    [FormerlySerializedAs("Affiliation")] [SerializeField] public Affiliation affiliation;
    [FormerlySerializedAs("UnitType")] [SerializeField] public UnitType unitType;
    //[SerializeField] public AbilityClass InitialAbility;
    [FormerlySerializedAs("ItemCapacity")] [SerializeField] public int itemCapacity;

    [SerializeField] public List<Item> itemList;

    public AbilityClass Ability { get; set; }
    [FormerlySerializedAs("CoreFrame")] [SerializeField] public CoreFrame coreFrame;
    [FormerlySerializedAs("Pilot")] [SerializeField] public Pilot pilot;
    [FormerlySerializedAs("Level")] [SerializeField] public int level;
    [FormerlySerializedAs("Experience")] [SerializeField] public int experience;
    [FormerlySerializedAs("ToNextLevel")] public int toNextLevel;

    [SerializeField] public List<SkillsMasterClass> skillsMaster;


    public void GainExperience (int experienceAdd)
    {
        experience += experienceAdd;
        CalculateLevel();
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



