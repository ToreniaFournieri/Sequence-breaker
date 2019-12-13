using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SequenceBreaker.Master.Units
{

    public class CalculateItemCapacity : MonoBehaviour
    {
        public static CalculateItemCapacity instance;

        public List<int> itemCapacityAddinglevelList;
        private void Awake()
        {
            Debug.Log("CalculateItemCapacity.Awake() GetInstanceID=" + this.GetInstanceID().ToString());

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
        }






    }
}