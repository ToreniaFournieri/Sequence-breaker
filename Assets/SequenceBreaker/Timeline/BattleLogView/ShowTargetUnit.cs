using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.BattleUnit;
using SequenceBreaker.Play.Battle;
using UnityEngine;

namespace SequenceBreaker.Timeline.BattleLogView
{
    public sealed class ShowTargetUnit : MonoBehaviour
    {
        public SimpleObjectPool unitIconObjectPool;


        private void Awake()
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.parent.transform.localScale = new Vector3(1, 1, 1);
        }

        private void Start()
        {
            //transform.localScale = new Vector3(1, 1, 1);

        }

        public void SetUnitInfo(List<BattleUnit> battleUnits)
        {
            if (battleUnits != null)
            {
                gameObject.SetActive(false);


                List<GameObject> children = new List<GameObject>();

                foreach (Transform child in transform)
                {
                    child.transform.localScale = new Vector3(1, 1, 1);
                    children.Add(child.gameObject);

                    //unitIconObjectPool.ReturnObject(child.gameObject);

                }

                // Somehow, this works.
                foreach (GameObject child in children)
                {
                    //child.transform.localScale = new Vector3(1, 1, 1);
                    unitIconObjectPool.ReturnObject(child);
                }

                bool isSwitchedAffiliation = false;


                foreach (var unit in battleUnits)
                {
                    // Display vs words just before enemy start.
                    if (unit.affiliation == Affiliation.Enemy && isSwitchedAffiliation == false)
                    {
                        GameObject vsIcon = unitIconObjectPool.GetObject();
                        vsIcon.GetComponent<UnitInfoSet>().unitInfoText.text = " vs ";
                        vsIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().previousShieldBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().previousHpBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().healedShieldBar.fillAmount = 0f;
                        vsIcon.GetComponent<UnitInfoSet>().healedHpBar.fillAmount = 0f;

                        vsIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false);

                        vsIcon.transform.parent = transform;
                        vsIcon.transform.localScale = new Vector3(1, 1, 1);
                        isSwitchedAffiliation = true;
                    }

                    float previousTurnShieldRatio = (unit.previousTurnShield / (float)unit.combat.shieldMax);
                    float shieldRatio = (unit.combat.shieldCurrent / (float)unit.combat.shieldMax);

                    float previousTurnHPRatio = (unit.previousTurnHp / (float)unit.combat.hitPointMax);
                    float hPRatio = (unit.combat.hitPointCurrent / (float)unit.combat.hitPointMax);

                    string deteriorationText = null;
                    if ((int)(unit.Deterioration * 100) >= 1)
                    {
                        deteriorationText = "(↓" + (int)(unit.Deterioration * 100) + "%) ";
                    }

                    GameObject unitIcon = unitIconObjectPool.GetObject();
                    //unitIcon.transform.localScale = new Vector3(1, 1, 1);

                    // set barrier remains icon
                    unitIcon.GetComponent<UnitInfoSet>().barrierRemains.text = unit.buff.BarrierRemaining.ToString();
                    if (unit.buff.BarrierRemaining > 0)
                    { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(true); }
                    else { unitIcon.GetComponent<UnitInfoSet>().barrierObject.SetActive(false); }
                    

                    unitIcon.GetComponent<UnitInfoSet>().unitInfoText.text =  unit.longName + " [" + unit.combat.shieldCurrent
                                                                             + "+" + unit.combat.hitPointCurrent + "] " + deteriorationText;

                    unitIcon.GetComponent<UnitInfoSet>().previousShieldBar.fillAmount = previousTurnShieldRatio;
                    unitIcon.GetComponent<UnitInfoSet>().previousHpBar.fillAmount = previousTurnHPRatio;


                    unitIcon.GetComponent<UnitInfoSet>().healedShieldBar.gameObject.SetActive(false);
                    unitIcon.GetComponent<UnitInfoSet>().healedHpBar.gameObject.SetActive(false);


                    if (previousTurnShieldRatio >= shieldRatio)
                    {
                        // damaged
                        unitIcon.GetComponent<UnitInfoSet>().shieldBar.gameObject.SetActive(true);
                        unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = shieldRatio;
                        unitIcon.GetComponent<UnitInfoSet>().previousShieldBar.gameObject.SetActive(true);

                    }
                    else
                    {
                        // healed
                        unitIcon.GetComponent<UnitInfoSet>().shieldBar.gameObject.SetActive(false);
                        unitIcon.GetComponent<UnitInfoSet>().previousShieldBar.gameObject.SetActive(false);

                        unitIcon.GetComponent<UnitInfoSet>().healedShieldBar.gameObject.SetActive(true);
                        unitIcon.GetComponent<UnitInfoSet>().healedShieldBar.fillAmount = shieldRatio;
                        unitIcon.GetComponent<UnitInfoSet>().shieldBar.gameObject.SetActive(true);

                        unitIcon.GetComponent<UnitInfoSet>().shieldBar.fillAmount = previousTurnShieldRatio;


                    }

                    if (previousTurnHPRatio >= hPRatio)
                    {
                        // damaged
                        unitIcon.GetComponent<UnitInfoSet>().hPBar.gameObject.SetActive(true);
                        unitIcon.GetComponent<UnitInfoSet>().hPBar.fillAmount = hPRatio;
                        unitIcon.GetComponent<UnitInfoSet>().previousHpBar.gameObject.SetActive(true);

                    }
                    else
                    {
                        // healed
                        unitIcon.GetComponent<UnitInfoSet>().hPBar.gameObject.SetActive(false);
                        unitIcon.GetComponent<UnitInfoSet>().healedHpBar.gameObject.SetActive(true);
                        unitIcon.GetComponent<UnitInfoSet>().healedHpBar.fillAmount = hPRatio;
                        unitIcon.GetComponent<UnitInfoSet>().previousHpBar.gameObject.SetActive(false);


                    }




                    unitIcon.transform.parent = transform;
                    unitIcon.transform.localScale = new Vector3(1, 1, 1);


                }

                gameObject.SetActive(true);

            }
        }

    }

}
