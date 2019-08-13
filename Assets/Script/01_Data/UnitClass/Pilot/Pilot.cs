using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pilot-", menuName = "Unit/Pilot", order = 3)]
public class Pilot : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public PilotStyle PilotStyle;
    [SerializeField] public Race Race;
    [SerializeField] public Preference Preference;
    [SerializeField] public int PilotLevel;

    [SerializeField] public AbilityClass AddAbility;

    //[SerializeField] public int AddPower;
    //[SerializeField] public int AddGeneration;
    //[SerializeField] public int AddStability;
    //[SerializeField] public int AddResponsiveness;
    //[SerializeField] public int AddPrecision;
    //[SerializeField] public int AddIntelligence;
    //[SerializeField] public int AddLuck;



}

public enum PilotStyle { remoteControling , directBoarding, artificialIntelligence }
public enum Race { human, psyborg , AI }
public enum Preference { offensive, defensive, obsessive}



