using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KohmaiWorks.Scroller
{
    sealed public class OpenSearchBar : MonoBehaviour
    {
        public bool IsOpen = false;

        private void Awake()
        {
            this.transform.gameObject.SetActive(false);

        }

        // Start is called before the first frame update
        void Start()
        {
        }



    }

}