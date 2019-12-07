using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker.GUIController
{
    public sealed class TransparentMessageController : MonoBehaviour
    {
        public GameObject transparentMessage;
        public Text transparentText;

        public void CloseMessage()
        {
            transparentText.text = null;
            transparentMessage.SetActive(false);

        }

        public void AddTextAndActive(string message)
        {
            if (transparentMessage.activeInHierarchy == false)
            {
                transparentText.text = null;
            }
            else
            {
                transparentText.text += "\n";
            }

            transparentText.text += message;

            transparentMessage.SetActive(true);


        }
    }
}
