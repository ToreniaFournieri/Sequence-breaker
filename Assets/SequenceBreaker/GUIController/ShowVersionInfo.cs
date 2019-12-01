using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.GUIController
{
    public class ShowVersionInfo : MonoBehaviour
    {
        public Text versionText;
    
        // Start is called before the first frame update
        void Start()
        {
            versionText.text = "Ver." + Application.version ;

        }

        // Update is called once per frame
    }
}
