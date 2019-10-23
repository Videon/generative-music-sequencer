using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools
{
    public double RandomDoubleMinMax(double min, double max)
    {
        lock (this)
        {
            System.Random rand = new System.Random();
            return rand.NextDouble() * (max - min) + min;
        }
        
    }
}