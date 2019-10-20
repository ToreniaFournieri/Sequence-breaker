using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Pilot-", menuName = "Unit/Pilot", order = 3)]
sealed public class Pilot : ScriptableObject
{
    [FormerlySerializedAs("Name")] [SerializeField] public string name;
    [FormerlySerializedAs("PilotStyle")] [SerializeField] public PilotStyle pilotStyle;
    [FormerlySerializedAs("Race")] [SerializeField] public Race race;
    [FormerlySerializedAs("Preference")] [SerializeField] public Preference preference;
    [FormerlySerializedAs("PilotLevel")] [SerializeField] public int pilotLevel;

    [FormerlySerializedAs("AddAbility")] [SerializeField] public AbilityClass addAbility;

    //[SerializeField] public int AddPower;
    //[SerializeField] public int AddGeneration;
    //[SerializeField] public int AddStability;
    //[SerializeField] public int AddResponsiveness;
    //[SerializeField] public int AddPrecision;
    //[SerializeField] public int AddIntelligence;
    //[SerializeField] public int AddLuck;



}

public enum PilotStyle { RemoteControling , DirectBoarding, ArtificialIntelligence }
public enum Race { Human, Psyborg , Ai }
public enum Preference { Offensive, Defensive, Obsessive}



