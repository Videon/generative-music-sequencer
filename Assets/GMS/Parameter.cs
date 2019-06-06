using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Parameter
{
    public string paramName;
    public float inputVal, outputVal;
    public float minNormal, maxNormal;

    public void SetValue(float pInputVal)
    {
        inputVal = pInputVal;
    }

    public void SetValueNormalize(float pInputVal)
    {
        if (Math.Abs(minNormal - maxNormal) > 0)
            outputVal = (inputVal - minNormal) / (maxNormal - minNormal);
    }

    public float GetOutputVal()
    {
        return outputVal;
    }
}