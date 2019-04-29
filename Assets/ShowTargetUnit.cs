using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    public class ShowTargetUnit : MonoBehaviour
    {
        //public int numberOfTargetUnit;
        public GameObject unitIconPrefabs;
        //public List<IconSet> IconSetInfo;

        //void Start()
        //{
        //    if (unitIconPrefabs != null)
        //    {

        //        for (int i = 0; i < numberOfTargetUnit; i++)
        //        {
        //            GameObject unitIcon = Instantiate(unitIconPrefabs);
        //            unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = 0.4f;
        //            unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = 0.7f;

        //            unitIcon.transform.parent = this.transform;
        //        }

        //    }
        //}

        public void SetUnitInfo(List<BattleUnit> battleUnits)
        {
            if (battleUnits != null)
            {
                Debug.Log("SetUnitInfo battleUnits count:" + battleUnits.Count);
                //GameObject[] children = this.gameObject.GetComponentsInChildren<GameObject>(tag = "UnitHorizontalInfo");
                GameObject[] children = GameObject.FindGameObjectsWithTag("UnitHorizontalInfo");

                for (int i = 0; i < children.Length; i++)
                {
                    Destroy(children[i]);
                }
                //foreach (GameObject child in children)
                //{
                //    DestroyImmediate(child);
                //}

                for (int i = 0; i < battleUnits.Count; i++)
                {


                    float shiledRatio = ((float)battleUnits[i].Combat.ShiledCurrent / (float)battleUnits[i].Combat.ShiledMax);
                    float hPRatio = ((float)battleUnits[i].Combat.HitPointCurrent / (float)battleUnits[i].Combat.HitPointMax);

                    string deteriorationText = null;
                    if ((int)(battleUnits[i].Deterioration * 100) >= 1)
                    {
                        deteriorationText = "(Deterioration:" + (int)(battleUnits[i].Deterioration * 100) + "%) ";
                    }
                    GameObject unitIcon = Instantiate(unitIconPrefabs);
                    unitIcon.GetComponent<UnitInfoSet>().UnitInfoText.text = "<b>" + battleUnits[i].Name + "</b> [" + battleUnits[i].Combat.ShiledCurrent +
                        "(" + (int)(shiledRatio * 100) + "%)+"
                        + battleUnits[i].Combat.HitPointCurrent + "(" + (int)(hPRatio * 100) + "%)]"
                        + deteriorationText;
                    unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = shiledRatio;
                    unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = hPRatio;
                    unitIcon.tag = "UnitHorizontalInfo";
                    unitIcon.transform.parent = this.transform;
                }

            }
        }

    }

}