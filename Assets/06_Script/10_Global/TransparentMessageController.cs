using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparentMessageController : MonoBehaviour
{
    public GameObject transparentMessage;
    public Text transparentText;

    public void UpdateMessage(string message)
    {
        transparentText.text += message;
        //transparentMessage.GetComponent<Text>().text += message;
    }

    public void CloseMessage()
    {
        transparentMessage.SetActive(false);

        transparentText.text = null;
        //transparentMessage.GetComponentInChildren<Text>().text = null;
    }


}
