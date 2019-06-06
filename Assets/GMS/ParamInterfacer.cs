﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public class ParamInterfacer : MonoBehaviour
    {
        public int paramCount;
        [SerializeField] public List<Parameter> parameters;

        public void InitParameters(int pParamCount)
        {
            if (pParamCount < 0 || pParamCount > 64 || pParamCount == paramCount) return;
            if (pParamCount > paramCount)
                for (int i = paramCount; i < pParamCount; i++)
                    parameters.Add(new Parameter());
            else
                parameters.RemoveRange(pParamCount, paramCount - pParamCount);
            paramCount = pParamCount;
        }

        public float GetParamValue(string paramName)
        {
            Parameter tempParam = NameToParam(paramName);
            return tempParam?.GetOutputVal() ?? 0.0f; //Is tempParam null? yes = return 0.0f; no = return GetOutputVal
        }

        public void SetParamValue(string paramName, float inputVal)
        {
            if (NameToParam(paramName) != null)
                NameToParam(paramName).SetValueNormalize(inputVal);
        }

        private Parameter NameToParam(string paramName)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                if (paramName == parameters[i].paramName)
                    return parameters[i];
            }

            return null;
        }
    }
}