using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPreviousView : MonoBehaviour
{
    public GameObject currentView;
    public GameObject previousView;


    public void Close()
    {
        previousView.SetActive(true);
        currentView.SetActive(false);
    }

}
