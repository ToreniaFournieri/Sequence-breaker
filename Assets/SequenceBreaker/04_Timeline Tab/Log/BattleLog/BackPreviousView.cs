using UnityEngine;

namespace SequenceBreaker._04_Timeline_Tab.Log.BattleLog
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
