using UnityEngine;

namespace SuperScrollView
{
    public class ListItem10 : MonoBehaviour
    {
        public ListItem9[] mItemList;

        public void Init()
        {
            foreach (ListItem9 item in mItemList)
            {
                item.Init();
            }
        }

    }



}
