using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class StartUp : MonoBehaviour
{

    //For inventory management
    public ItemDataBase itemDataBase;
    //Item inventory
    public UnitClass inventory;
    //Ally inventory
    public List<UnitClass> allyUnitList;


    //wake up all main tab
    public GameObject a1;
    public GameObject a2;
    public GameObject a3;


    // Start is called before the first frame update
    void Start()
    {
        //inventory
        inventory.itemList = itemDataBase.LoadItemList("item-" + inventory.affiliation + "-" + inventory.uniqueId);


        //ally unit load
        foreach (UnitClass unit in allyUnitList)
        {
            if (unit != null)
            {
                unit.itemList = itemDataBase.LoadItemList("item-" + unit.affiliation + "-" + unit.uniqueId);
            }
        }

        a1.SetActive(true);
        a2.SetActive(true);
        a3.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
