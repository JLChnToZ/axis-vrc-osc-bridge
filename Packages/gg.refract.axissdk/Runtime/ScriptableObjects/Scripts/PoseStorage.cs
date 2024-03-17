using System.Collections;
using System.Collections.Generic;
using Axis.Utils;
using UnityEngine;

[CreateAssetMenu]
public class PoseStorage : ScriptableObject
{
    public SerializableDictionary<HumanBodyBones, Quaternion> poseLocalRotations;
}
