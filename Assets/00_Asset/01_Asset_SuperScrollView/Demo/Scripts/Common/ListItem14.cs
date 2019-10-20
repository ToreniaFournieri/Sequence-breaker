using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
    public class ListItem14Elem
    {
        public GameObject MRootObj;
        public Image MIcon;
        public Text MName;
    }
    public class ListItem14 : MonoBehaviour
    {

        public List<ListItem14Elem> mElemItemList = new List<ListItem14Elem>();

        public void Init()
        {
            int childCount = transform.childCount;
            for(int i= 0;i<childCount;++i)
            {
                Transform tf = transform.GetChild(i);
                ListItem14Elem elem = new ListItem14Elem();
                elem.MRootObj = tf.gameObject;
                elem.MIcon = tf.Find("ItemIcon").GetComponent<Image>();
                elem.MName = tf.Find("ItemIcon/name").GetComponent<Text>();
                mElemItemList.Add(elem);
            }
        }

    }
}
