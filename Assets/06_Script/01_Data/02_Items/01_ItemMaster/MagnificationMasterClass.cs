using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magnification-", menuName = "Item/MagnificationMaster", order = 5)]
public class MagnificationMasterClass : ScriptableObject
{
    //Offense or defense
    [SerializeField] public OffenseOrDefense OffenseOrDefense = OffenseOrDefense.none;
    //2/3, x1.05, +12%...  etc
    [SerializeField] public MagnificationType MagnificationType = MagnificationType.none;

    // Critical, Kinetic, vsRobot... etc
    [SerializeField] public MagnificationTarget MagnificationTarget = MagnificationTarget.none;


    // Value itself. [CAUTIONS] it depend which value is used by MagnificationType.
    [SerializeField] public MagnificationFixedRatio MagnificationFixedRatio = MagnificationFixedRatio.oneOverOne;
    [Range(0.5f, 3.0f)] [SerializeField] public double MagnificationRatio = 1.0f;
    [SerializeField] public MagnificationPercent MagnificationPercent = MagnificationPercent.zero;


}
