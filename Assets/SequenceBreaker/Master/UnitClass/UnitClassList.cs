using System.Collections.Generic;
using UnityEngine;

namespace SequenceBreaker.Master.UnitClass
{
    [CreateAssetMenu(fileName = "UnitClassList-", menuName = "Unit/UnitClassList", order = 24)]
    public class UnitClassList : ScriptableObject
    {
        [SerializeField] public List<UnitClass> unitList;


        public void Copy(UnitClassList unitClassList)
        {

            foreach (var unit in unitClassList.unitList)
            {
                unitList.Add(unit);
            }



        }
    }
}
