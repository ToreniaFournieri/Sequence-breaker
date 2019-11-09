using UnityEngine;

namespace SequenceBreaker._10_Global
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

