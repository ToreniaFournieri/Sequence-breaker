using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._05_Play.MissionView
{
    public sealed class MissionUpdateText : MonoBehaviour
    {
        public Text levelOfMissionText;
        public Slider levelOfMissionSlider;


        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            levelOfMissionText.text = "lv: " + (int)levelOfMissionSlider.value;

        }
    }
}
