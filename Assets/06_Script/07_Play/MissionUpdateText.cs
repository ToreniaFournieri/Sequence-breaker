using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUpdateText : MonoBehaviour
{
    public Text levelOfMissionText;
    public Slider levelOfMissionSlider;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        levelOfMissionText.text = "lv: " + (int)levelOfMissionSlider.value;

    }
}
