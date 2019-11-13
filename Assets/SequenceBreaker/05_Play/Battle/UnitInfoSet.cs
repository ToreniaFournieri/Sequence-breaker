using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SequenceBreaker._05_Play.Battle
{
    public sealed class UnitInfoSet : MonoBehaviour
    {
        //public GameObject iconMask;
        //public Image unitIcon;
        public Image hPBar;
        public Image shieldBar;
        [FormerlySerializedAs("UnitInfoText")] public Text unitInfoText;
        public GameObject barrierObject;
        public Text barrierRemains;


        // Start is called before the first frame update
        void Start()
        {
            transform.localScale = new Vector3(1f, 1f, 1f);


        }
    }
}
