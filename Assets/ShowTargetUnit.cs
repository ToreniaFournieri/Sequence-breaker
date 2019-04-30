using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    public class ShowTargetUnit : MonoBehaviour
    {
        //public int numberOfTargetUnit;
        //public GameObject unitIconPrefabs;
        public SimpleObjectPool unitIconObjectPool;
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
                List<GameObject> children = new List<GameObject>();

                foreach (Transform child in this.transform)
                {
                    // [1]This didn't work.
                    //GameObject toReturn = new GameObject();
                    // toReturn = child.gameObject;
                    //unitIconObjectPool.ReturnObject(toReturn);

                    // [2]This also didn't work.
                    //unitIconObjectPool.ReturnObject(child.gameObject);

                    children.Add(child.gameObject);
                }

                // [3]Somehow, this works.
                foreach (GameObject child in children)
                {
                    unitIconObjectPool.ReturnObject(child);
                }

                for (int i = 0; i < battleUnits.Count; i++)
                {


                    float shiledRatio = ((float)battleUnits[i].Combat.ShiledCurrent / (float)battleUnits[i].Combat.ShiledMax);
                    float hPRatio = ((float)battleUnits[i].Combat.HitPointCurrent / (float)battleUnits[i].Combat.HitPointMax);

                    string deteriorationText = null;
                    if ((int)(battleUnits[i].Deterioration * 100) >= 1)
                    {
                        deteriorationText = "(Deterioration:" + (int)(battleUnits[i].Deterioration * 100) + "%) ";
                    }

                    GameObject unitIcon = unitIconObjectPool.GetObject();
                    //GameObject unitIcon = Instantiate(unitIconPrefabs);
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