using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparentMessageController : MonoBehaviour
{
    public GameObject transparentMessage;

    public void UpdateMessage(string message)
    {
        transparentMessage.GetComponent<Text>().text += message;
    }

    public void CloseMessage()
    {
        transparentMessage.SetActive(false);
        transparentMessage.GetComponentInChildren<Text>().text = null;

    }


}
