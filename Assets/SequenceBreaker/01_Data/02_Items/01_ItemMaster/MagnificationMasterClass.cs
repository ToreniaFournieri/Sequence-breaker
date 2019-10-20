using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "", menuName = "Item/MagnificationMaster", order = 5)]
sealed public class MagnificationMasterClass : ScriptableObject
{
    //Offense or defense
    [FormerlySerializedAs("OffenseOrDefense")] [SerializeField] public OffenseOrDefense offenseOrDefense = OffenseOrDefense.None;
    //2/3, x1.05, +12%...  etc
    [FormerlySerializedAs("MagnificationType")] [SerializeField] public MagnificationType magnificationType = MagnificationType.None;

    // Critical, Kinetic, vsRobot... etc
    [FormerlySerializedAs("MagnificationTarget")] [SerializeField] public MagnificationTarget magnificationTarget = MagnificationTarget.None;


    // Value itself. [CAUTIONS] it depend which value is used by MagnificationType.
    [FormerlySerializedAs("MagnificationRatio")] [Range(0.1f, 3.0f)] [SerializeField] public double magnificationRatio = 1.0f;
    [FormerlySerializedAs("MagnificationPercent")] [SerializeField] public MagnificationPercent magnificationPercent = MagnificationPercent.Zero;


}
