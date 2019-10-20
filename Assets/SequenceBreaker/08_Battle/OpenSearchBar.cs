using UnityEngine;

namespace KohmaiWorks.Scroller
{
    sealed public class OpenSearchBar : MonoBehaviour
    {
        public bool IsOpen;

        private void Awake()
        {
            transform.gameObject.SetActive(false);

        }

        // Start is called before the first frame update
        void Start()
        {
        }



    }

}