using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
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
