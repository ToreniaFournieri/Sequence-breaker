using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyStatusIcons : MonoBehaviour
{
    public List<BattleUnit> partyBattleUnitList;
    public SimpleObjectPool allyUnitIconObjectPool;
    public Transform contentPanel;

    private void Awake()
    {

    }

    private void Start()
    {




    }

    public void UpdateStatus()
    {

        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            allyUnitIconObjectPool.ReturnObject(toRemove);
        }

        foreach (BattleUnit _battleUnit in partyBattleUnitList)
        {
            GameObject newPanel = allyUnitIconObjectPool.GetObject();
            newPanel.transform.SetParent(contentPanel);
            newPanel.transform.localScale = new Vector3(1f, 1f, 1f);


            //three
            Image[] _imageArray = newPanel.GetComponentsInChildren<Image>();


            // 1 is Shield bar
            _imageArray[1].fillAmount = (float)_battleUnit.Combat.ShieldCurrent / (float)_battleUnit.Combat.ShieldMax;
            // 2 is HP bar
            _imageArray[2].fillAmount = (float)_battleUnit.Combat.HitPointCurrent / (float)_battleUnit.Combat.HitPointMax;

            // 3 is icon?



        }
    }

}
