using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pilot-", menuName = "Unit/Pilot", order = 3)]
public class Pilot : ScriptableObject
{
    public string Name;
    public PilotStyle PilotStyle;
    public Race Race;
    public Preference Preference;


}

public enum PilotStyle { remoteControling , directBoarding, artificialIntelligence }
public enum Race { human, AI }
public enum Preference { offensive, defensive, obsessive}



