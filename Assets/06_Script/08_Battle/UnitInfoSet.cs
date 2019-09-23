using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoSet : MonoBehaviour
{
    //public GameObject iconMask;
    //public Image unitIcon;
    public Image hPBar;
    public Image shieldBar;
    public Text UnitInfoText;
    public GameObject barrierObject;
    public Text barrierRemains;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(1f, 1f, 1f);


    }

    private void Awake()
    {

    }

}
