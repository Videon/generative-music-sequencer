using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public class ParamInterfacer : MonoBehaviour
    {
        public Parameter[] parameters;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public float GetParamValue(string paramName)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (paramName == parameters[i].name)
                    return parameters[i].GetValue();
            }

            return 0.0f;
        }
    }

    public class Parameter : PropertyAttribute
    {
        [TextArea(1, 1)] public string name;
        private float _value;
        public float minNormal, maxNormal;

        public void SetValue(float inputVal)
        {
            _value = inputVal;
        }

        public void SetValueNormalize(float inputVal)
        {
            _value = (inputVal - minNormal) / (maxNormal - minNormal);
        }

        public float GetValue()
        {
            return _value;
        }
    }
}