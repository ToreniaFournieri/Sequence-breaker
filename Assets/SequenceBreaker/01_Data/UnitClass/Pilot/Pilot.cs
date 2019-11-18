using SequenceBreaker._01_Data.BattleUnit;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data.UnitClass.Pilot
{
    [CreateAssetMenu(fileName = "Pilot-", menuName = "Unit/Pilot", order = 3)]
    public sealed class Pilot : ScriptableObject
    {
        [FormerlySerializedAs("Name")] [SerializeField] public new string name;
        [FormerlySerializedAs("PilotStyle")] [SerializeField] public PilotStyle pilotStyle;
        [FormerlySerializedAs("Race")] [SerializeField] public Race race;
        [FormerlySerializedAs("Preference")] [SerializeField] public Preference preference;
        [FormerlySerializedAs("PilotLevel")] [SerializeField] public int pilotLevel;

        [FormerlySerializedAs("AddAbility")] [SerializeField] public AbilityClass addAbility;
        
    }

    public enum PilotStyle { RemoteControlling , DirectBoarding, ArtificialIntelligence }
    public enum Race { Human, Cyborg , Ai }
    public enum Preference { Offensive, Defensive, Obsessive}
}