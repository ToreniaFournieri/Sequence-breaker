using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit-", menuName = "Unit/Unit", order = 3)]
public class UnitClass : ScriptableObject
{
    [SerializeField] public int UniqueID;
    [SerializeField] public string Name;
    [SerializeField] public Affiliation Affiliation;
    [SerializeField] public UnitType UnitType;
    //[SerializeField] public AbilityClass InitialAbility;
    [SerializeField] public int ItemCapacity;

    [SerializeField] public List<Item> itemList;

    public AbilityClass Ability { get; set; }
    [SerializeField] public CoreFrame CoreFrame;
    [SerializeField] public Pilot Pilot;
    [SerializeField] public int Level;
    [SerializeField] public int Experience;
    public int ToNextLevel;

    [SerializeField] public List<SkillsMasterClass> skillsMaster;


    public void GainExperience (int _experienceAdd)
    {
        Experience += _experienceAdd;
        CalculateLevel();
    }


    private void CalculateLevel ()
    {
        int _level = 1;
        int _remainder = Experience;
        while (_remainder > 0)
        {

            //Sample implement.
            int _step = (int)(Mathf.Pow(1.26f - 0.001f * _level, _level) + 100 + _level * 20) ;
            ToNextLevel = _step - _remainder;
            _remainder -= _step;

            if (_remainder > 0)
            {
                _level++;
            }
            
        }

        Level = _level;
    }

    public int ExperienceFromBeaten()
    {
        //Sample implement.
        return (int)((Mathf.Pow(1.26f - 0.001f * Level, Level) + 100 + Level * 20 )/30);

    }


}



