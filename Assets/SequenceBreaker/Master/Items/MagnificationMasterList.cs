using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.Items
{

    [CreateAssetMenu(fileName = "MagnificationMasterList", menuName = "Item/MagnificationMasterList", order = 1)]
    public class MagnificationMasterList : ScriptableObject
    {
        public List<MagnificationMasterClass> magnificationList;
    }
}

