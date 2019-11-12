using SequenceBreaker._01_Data._08_BattleUnitSub;
using UnityEngine;
using UnityEngine.Serialization;

namespace SequenceBreaker._01_Data._03_UnitClass.Pilot
{
    [CreateAssetMenu(fileName = "Pilot-", menuName = "Unit/Pilot", order = 3)]
    public sealed class Pilot : ScriptableObject
    {
        [FormerlySerializedAs("Name")] [SerializeField] public string name;
        [FormerlySerializedAs("PilotStyle")] [SerializeField] public PilotStyle pilotStyle;
        [FormerlySerializedAs("Race")] [SerializeField] public Race race;
        [FormerlySerializedAs("Preference")] [SerializeField] public Preference preference;
        [FormerlySerializedAs("PilotLevel")] [SerializeField] public int pilotLevel;

        [FormerlySerializedAs("AddAbility")] [SerializeField] public AbilityClass addAbility;
        
    }

    public enum PilotStyle { RemoteControling , DirectBoarding, ArtificialIntelligence }
    public enum Race { Human, Psyborg , Ai }
    public enum Preference { Offensive, Defensive, Obsessive}
}