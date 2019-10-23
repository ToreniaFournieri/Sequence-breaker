using System.Collections.Generic;
using UnityEngine;

namespace _00_Asset._01_Asset_SuperScrollView.Demo.Scripts.Common
{
    public class ListItem15 : MonoBehaviour
    {
        public List<ListItem16> mItemList;

        public void Init()
        {
            foreach (ListItem16 item in mItemList)
            {
                item.Init();
            }
        }

    }



}
