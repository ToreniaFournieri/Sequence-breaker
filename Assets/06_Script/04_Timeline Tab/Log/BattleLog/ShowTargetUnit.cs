using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    public class ShowTargetUnit : MonoBehaviour
    {
        public SimpleObjectPool unitIconObjectPool;

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

                bool isSwitchedAffiliation = false;

                for (int i = 0; i < battleUnits.Count; i++)
                {
                    // Display vs words just before enemy start.
                    if (battleUnits[i].Affiliation == Affiliation.enemy && isSwitchedAffiliation == false)
                    {
                        GameObject vsIcon = unitIconObjectPool.GetObject();
                        vsIcon.GetComponent<UnitInfoSet>().UnitInfoText.text = " vs ";
                        vsIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false);

                        vsIcon.transform.parent = this.transform;
                        isSwitchedAffiliation = true;
                    }

                    float shieldRatio = ((float)battleUnits[i].Combat.ShieldCurrent / (float)battleUnits[i].Combat.ShieldMax);
                    float hPRatio = ((float)battleUnits[i].Combat.HitPointCurrent / (float)battleUnits[i].Combat.HitPointMax);

                    string deteriorationText = null;
                    if ((int)(battleUnits[i].Deterioration * 100) >= 1)
                    {
                        deteriorationText = "(↓" + (int)(battleUnits[i].Deterioration * 100) + "%) ";
                    }

                    GameObject unitIcon = unitIconObjectPool.GetObject();

                    // set barrier remains icon
                    unitIcon.GetComponent<UnitInfoSet>().barrierRemains.text = battleUnits[i].Buff.BarrierRemaining.ToString();
                    if (battleUnits[i].Buff.BarrierRemaining > 0)
                    { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(true); }
                    else { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false); }

                    //                unitIcon.GetComponent<UnitInfoSet>().UnitInfoText.text = "<b>" + battleUnits[i].Name + "</b> [" + battleUnits[i].Combat.ShiledCurrent +
                    //"(" + (int)(shiledRatio * 100) + "%)+"
                    //+ battleUnits[i].Combat.HitPointCurrent + "(" + (int)(hPRatio * 100) + "%)]"
                    //+ deteriorationText;

                    unitIcon.GetComponent<UnitInfoSet>().UnitInfoText.text = "<b>" + battleUnits[i].Name + "</b> [" + battleUnits[i].Combat.ShieldCurrent
                        + "+" + battleUnits[i].Combat.HitPointCurrent + "] " + deteriorationText;
                    unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = shieldRatio;
                    unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = hPRatio;
                    //unitIcon.tag = "UnitHorizontalInfo";
                    unitIcon.transform.parent = this.transform;
                }

            }
        }

    }

}
