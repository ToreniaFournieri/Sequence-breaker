using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

sealed public class PartyStatusIcons : MonoBehaviour
{
    public MissionController missionController;
    public List<BattleUnit> partyBattleUnitList;
    public SimpleObjectPool allyUnitIconObjectPool;
    public Transform contentPanel;

    private void Awake()
    {

    }

    private void Start()
    {
        missionController.UpdatePartyStatus();
        UpdateStatus();



    }

    public void UpdateStatus()
    {

        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            allyUnitIconObjectPool.ReturnObject(toRemove);
        }

        foreach (BattleUnit battleUnit in missionController.allyCurrentBattleUnitList)
            //foreach (BattleUnit _battleUnit in partyBattleUnitList)
        {
            GameObject newPanel = allyUnitIconObjectPool.GetObject();
            newPanel.transform.SetParent(contentPanel);
            newPanel.transform.localScale = new Vector3(1f, 1f, 1f);


            //three
            Image[] imageArray = newPanel.GetComponentsInChildren<Image>();


            // 1 is Shield bar
            imageArray[1].fillAmount = (float)battleUnit.combat.shieldCurrent / (float)battleUnit.combat.shieldMax;
            // 2 is HP bar
            imageArray[2].fillAmount = (float)battleUnit.combat.hitPointCurrent / (float)battleUnit.combat.hitPointMax;

            // 3 is icon?



        }
    }

}
