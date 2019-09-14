using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SwitchLayer : MonoBehaviour
{
    public GameObject translucentLayer;
    public void SwitchLayerToFront (GameObject activeLayer)
    {
        translucentLayer.transform.SetAsLastSibling();
        activeLayer.transform.SetAsLastSibling();
    }


}
