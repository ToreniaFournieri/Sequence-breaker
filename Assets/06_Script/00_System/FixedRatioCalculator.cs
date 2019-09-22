using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRatioCalculator 
{
    // return value
    public double value = 1.0;

    public FixedRatioCalculator(MagnificationFixedRatio magnificationFixedRatio)
    {
        switch (magnificationFixedRatio)
        {
            case MagnificationFixedRatio.oneOverOne:
                value = 1.0;
                break;
            case MagnificationFixedRatio.twoOverThree:
                value = 2.0 / 3.0;
                break;
            case MagnificationFixedRatio.threeOverFour:
                value = 3.0 / 4.0;
                break;
            case MagnificationFixedRatio.fourOverFive:
                value = 4.0 / 5.0;
                break;
            case MagnificationFixedRatio.fiveOverSix:
                value = 5.0 / 6.0;
                break;
            case MagnificationFixedRatio.sixOverFive:
                value = 6.0 / 5.0;
                break;
            case MagnificationFixedRatio.fiveOverFour:
                value = 5.0 / 4.0;
                break;
            case MagnificationFixedRatio.fourOverThree:
                value = 4.0 / 3.0;
                break;
            case MagnificationFixedRatio.threeOverTwo:
                value = 3.0 / 2.0;
                break;
            case MagnificationFixedRatio.twoOverOne:
                value = 2.0 / 1.0;
                break;
            case MagnificationFixedRatio.oneOverTen:
                value = 1.0 / 10.0;
                break;
            case MagnificationFixedRatio.oneOverHundred:
                value = 1.0 / 100.0;
                break;
            default:
                Debug.LogError(" unexpected MagnificationFixedRatio:" + magnificationFixedRatio);
                break;

        }

    }


}
