using System.Collections;
using System.Collections.Generic;
using SequenceBreaker.Environment;
using SequenceBreaker.Master.Items;
using UnityEngine;
using UnityEngine.Events;

namespace SequenceBreaker.GUIController
{
    public class ResetGameProgress : MonoBehaviour
    {
        public StartUp startUp;
        private UnityAction unityAction;

        private void Start()
        {
            unityAction += ResetGame;
        }

        public void Reset()
        {
            PopupWindowSingleton.instance.SetPopUpwindow("Reset Game Progress?"
                        , "Delete all of your progress? This action cannot be undone.", unityAction);
    
        }


        void ResetGame()
        {

            Debug.Log("Reset has been pressed!");
            ItemDataBase.instance.DeleteAllSavedFiles(true);
            startUp.InitializeInventory();

        }


    }

}