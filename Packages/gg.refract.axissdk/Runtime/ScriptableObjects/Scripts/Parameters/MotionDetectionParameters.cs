using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MotionDetectionParameters")]
    public class MotionDetectionParameters : ScriptableObject
    {
        [Range(0, 0.01f)] public float rotationThreshold = 0f;
        public int noRotationCounterThreshold = 1000;

    }
}

