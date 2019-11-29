using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{ 
    public class ItemPresetList : ScriptableObject
    {
        public List<ItemPreset> itemPresetList;



        static ItemPreset _instance;
        public static ItemPreset Get
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ItemPreset>();
                }
                return _instance;
            }

        }

    }

}
