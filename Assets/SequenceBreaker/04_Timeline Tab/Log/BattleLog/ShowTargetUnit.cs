using System.Collections.Generic;
using SequenceBreaker._00_System;
using SequenceBreaker._01_Data;
using SequenceBreaker._08_Battle;
using UnityEngine;

namespace SequenceBreaker._04_Timeline_Tab.Log.BattleLog
{
    public sealed class ShowTargetUnit : MonoBehaviour
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

                    //child.transform.localScale = new Vector3(1f, 1f, 1f);

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
                    if (battleUnits[i].affiliation == Affiliation.Enemy && isSwitchedAffiliation == false)
                    {
                        GameObject vsIcon = unitIconObjectPool.GetObject();
                        vsIcon.GetComponent<UnitInfoSet>().unitInfoText.text = " vs ";
                        vsIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = 0f;

                        vsIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false);

                        vsIcon.transform.parent = this.transform;
                        isSwitchedAffiliation = true;
                    }

                    float shieldRatio = ((float)battleUnits[i].combat.shieldCurrent / (float)battleUnits[i].combat.shieldMax);
                    float hPRatio = ((float)battleUnits[i].combat.hitPointCurrent / (float)battleUnits[i].combat.hitPointMax);

                    string deteriorationText = null;
                    if ((int)(battleUnits[i].Deterioration * 100) >= 1)
                    {
                        deteriorationText = "(↓" + (int)(battleUnits[i].Deterioration * 100) + "%) ";
                    }

                    GameObject unitIcon = unitIconObjectPool.GetObject();

                    // set barrier remains icon
                    unitIcon.GetComponent<UnitInfoSet>().barrierRemains.text = battleUnits[i].buff.BarrierRemaining.ToString();
                    if (battleUnits[i].buff.BarrierRemaining > 0)
                    { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(true); }
                    else { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false); }

                    //                unitIcon.GetComponent<UnitInfoSet>().UnitInfoText.text = "<b>" + battleUnits[i].Name + "</b> [" + battleUnits[i].Combat.ShiledCurrent +
                    //"(" + (int)(shiledRatio * 100) + "%)+"
                    //+ battleUnits[i].Combat.HitPointCurrent + "(" + (int)(hPRatio * 100) + "%)]"
                    //+ deteriorationText;

                    unitIcon.GetComponent<UnitInfoSet>().unitInfoText.text = "<b>" + battleUnits[i].name + "</b> [" + battleUnits[i].combat.shieldCurrent
                        + "+" + battleUnits[i].combat.hitPointCurrent + "] " + deteriorationText;
                    unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = shieldRatio;
                    unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = hPRatio;
                    //unitIcon.tag = "UnitHorizontalInfo";
                    //unitIcon.transform.localScale = new Vector3(1f, 1f, 1f); // didnt work
                    unitIcon.transform.parent = this.transform;
                }

            }
        }

    }

}
