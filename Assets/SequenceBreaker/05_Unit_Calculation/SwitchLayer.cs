using UnityEngine;

[System.Serializable]
sealed public class SwitchLayer : MonoBehaviour
{
    public GameObject translucentLayer;
    public void SwitchLayerToFront (GameObject activeLayer)
    {
        translucentLayer.transform.SetAsLastSibling();
        activeLayer.transform.SetAsLastSibling();
    }


}
