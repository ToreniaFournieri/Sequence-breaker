using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SequenceBreaker.GUIController
{
    public class PopupWindowSingleton : MonoBehaviour
    {
        public GameObject popUpWindowGameObject;
        public Text headerText;
        public Text contentText;
        public Button okButton;
        public Button cancelButton;


        //Singleton
        public static PopupWindowSingleton instance;       

        private void Awake()
        {
            //Debug.Log("PopupWindowController.Awake() GetInstanceID=" + this.GetInstanceID().ToString());

            if (instance == null)
            {
                instance = this;  //This is the first Singleton instance. Retain a handle to it.
            }
            else
            {
                if (instance != this)
                {
                    Destroy(this); //This is a duplicate Singleton. Destroy this instance.
                }
                else
                {
                    //Existing Singleton instance found. All is good. No change.
                }
            }

            DontDestroyOnLoad(gameObject);

            popUpWindowGameObject.SetActive(false);
        }

        public void SetPopUpwindow(string headerString, string contentString, UnityAction okAction)
        {
            headerText.text = headerString;
            contentText.text = contentString;

            okButton.onClick.AddListener(okAction);
            okButton.onClick.AddListener(CancelAction);

            cancelButton.onClick.AddListener(CancelAction);


            popUpWindowGameObject.SetActive(true);

        }


        void CancelAction()
        {
            headerText.text = "Header (dummy)";
            contentText.text = "Content (dummy)";
            okButton.onClick.RemoveAllListeners();
            popUpWindowGameObject.SetActive(false);

        }


    }

}
