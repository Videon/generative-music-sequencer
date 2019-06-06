using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosToCurve : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animationCurve;
    private Keyframe[] _keyframes;

    // Start is called before the first frame update
    void Start()
    {
        _keyframes = new Keyframe[3];
        _keyframes[0].time = 0.0f;
        _keyframes[1].time = 0.5f;
        _keyframes[2].time = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        var position = transform.position;
        _keyframes[0].value = position.x;
        _keyframes[1].value = position.y;
        _keyframes[2].value = position.z;
        _animationCurve.keys = _keyframes;
    }
}