using System;
using System.Collections.Generic;
using SequenceBreaker.Master.Items;
using SequenceBreaker.Master.Units;
using UnityEngine;

namespace SequenceBreaker.Drop
{
    public sealed class DropEngine
    {
        // This value is temporary.
        private double _itemDropRatio = 0.100;
        private float _enhancedSigma = 2.0f;

        // Internal information
        //private List<UnitClass> _enemyUnitList;
        private List<Item> _dropItemList;

        public List<Item> GetDroppedItems(List<UnitClass> enemyUnitList)
        {
            _dropItemList = new List<Item>();

            //1.Correct all item of enemy units
            foreach (UnitClass unit in enemyUnitList)
            {

                unit.UpdateItemCapacity();

                int _itemCapacityCount = 0;

                foreach (Item item in unit.itemList)
                {

                    if (_itemCapacityCount >= unit.itemCapacity)
                    {
                        continue;
                    }
                    _itemCapacityCount++;

                    //2.[UNIMPLEMENTED] Remove undroppable item if it is.

                    int _enhancedMaxValue = item.enhancedValue;
                    float _enhancedMu = item.enhancedValue - 3.0f + (float)unit.level / 40.0f ;

                    //3.Drop judgement using seed. NextDouble range is between 0.0 to 1.0.
                    float randomItemDrop = UnityEngine.Random.Range(0, 1.0f);
                    if (_itemDropRatio >= randomItemDrop)
                    {

                        Item _itemDrop = ScriptableObject.CreateInstance<Item>();
                        //4. judge the enhanced value.

                        float _totalValue = 0.0f;
                            List<float> stepValue = new List<float>();
                            for (int i = _enhancedMaxValue; i >= -3; i--)
                            {
                                float value = (1 / (Mathf.Pow((2.0f * Mathf.PI), 1.0f / 2.0f) * _enhancedSigma)
                                    * Mathf.Exp(-Mathf.Pow((i - _enhancedMu), 2.0f) / 2.0f / Mathf.Pow(_enhancedSigma, 2.0f)));

                                _totalValue += value;
                                stepValue.Add(_totalValue);
                            }


                            double randomValue = UnityEngine.Random.Range(0, 1.0f);
                            int enhancedValue = _enhancedMaxValue;
                            foreach (float value in stepValue)
                            {
                                if ((value / _totalValue) >= randomValue)
                                {
                                //Debug.Log("randomItemDrop:" + randomItemDrop + " randomValue:" + randomValue + " enhancedValue: "
                                //    + enhancedValue + " value: " + value + " total:" + _totalValue);
                                _itemDrop = item.Copy();

                                _itemDrop.enhancedValue = enhancedValue;
                                    continue;
                                }
                                enhancedValue--;

                            }


                        _dropItemList.Add(_itemDrop);
                    }
                }
            }

            return _dropItemList;
        }

    }
}
