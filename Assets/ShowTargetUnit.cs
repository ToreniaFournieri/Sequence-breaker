using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTargetUnit : MonoBehaviour
{
    public int numberOfTargetUnit;
    public GameObject unitIconPrefabs;
    public List<IconSet> IconSetInfo;

    void Start()
    {
        if (unitIconPrefabs != null)
        {

            for (int i = 0; i < numberOfTargetUnit; i++)
            {
                GameObject unitIcon = Instantiate(unitIconPrefabs);
                unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = 0.4f;
                unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = 0.7f;

                unitIcon.transform.parent = this.transform;
            }

        }
    }

}
