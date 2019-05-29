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
        inputVal = inputVal;
    }

    public void SetValueNormalize(float pInputVal)
    {
        inputVal = (inputVal - minNormal) / (maxNormal - minNormal);
    }

    public float GetOutputVal()
    {
        return outputVal;
    }
}