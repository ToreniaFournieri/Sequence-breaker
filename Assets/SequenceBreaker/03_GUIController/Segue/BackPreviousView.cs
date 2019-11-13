using UnityEngine;

namespace SequenceBreaker._03_GUIController.Segue
{
    public  sealed class BackPreviousView : MonoBehaviour
    {
        public GameObject currentView;
        public GameObject previousView;


        public void Close()
        {
            previousView.SetActive(true);
            currentView.SetActive(false);
        }

    }
}

