using UnityEngine;
using UnityEngine.UI;

namespace SequenceBreaker._03_GUIController
{
    public sealed class TransparentMessageController : MonoBehaviour
    {
        public GameObject transparentMessage;
        public Text transparentText;

        public void CloseMessage()
        {
            transparentMessage.SetActive(false);
            transparentText.text = null;

        }


    }
}
