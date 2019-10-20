using UnityEngine;

namespace SequenceBreaker._05_Unit_Calculation
{
    [System.Serializable]
    public sealed class SwitchLayer : MonoBehaviour
    {
        public GameObject translucentLayer;
        public void SwitchLayerToFront (GameObject activeLayer)
        {
            translucentLayer.transform.SetAsLastSibling();
            activeLayer.transform.SetAsLastSibling();
        }


    }
}
