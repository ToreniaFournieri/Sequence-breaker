using UnityEngine;

namespace SequenceBreaker._04_Home.EquipView
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
