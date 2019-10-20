using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public class ListItem6 : MonoBehaviour
    {
        public List<ListItem5> mItemList;

        public void Init()
        {
            foreach (ListItem5 item in mItemList)
            {
                item.Init();
            }
        }

    }


   
}
