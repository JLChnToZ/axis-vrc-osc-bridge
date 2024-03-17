using Axis.Enumerations;
using Axis.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nodes Calibration Offsets")]
public class NodeCalibrationOffsets : ScriptableObject
{
    public SerializableDictionary<NodeBinding, Vector3> rotationOffsets;

    public Vector3 absolutePositionOffset;


    private void OnEnable()
    {
        if(rotationOffsets == null)
        {
            Debug.Log("Was null");
            rotationOffsets = new SerializableDictionary<NodeBinding, Vector3>();
        }

        foreach (NodeBinding key in Enum.GetValues(typeof(NodeBinding)))
        {
            if(rotationOffsets.ContainsKey(key) == false)
            {
                Debug.Log($"Didnt contain {key}");
                rotationOffsets.Add(key, Vector3.zero);
            }
        }

        if(rotationOffsets.Count == 0)
        {
            Debug.Log($"Rotation offsets was zero");
            rotationOffsets = new SerializableDictionary<NodeBinding, Vector3>();
            foreach (NodeBinding nodeLimb in Enum.GetValues(typeof(NodeBinding)))
            {
                rotationOffsets.Add(nodeLimb, Vector3.zero);
            }
        }
        
    }
}


