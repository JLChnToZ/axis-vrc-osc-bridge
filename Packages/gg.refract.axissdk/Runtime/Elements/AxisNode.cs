using Axis.Enumerations;
using System;
using UnityEngine;




namespace Axis.Elements
{


    public class AxisNode : AxisRequiringElements
    {
        [HideInInspector] public NodeCalibrationOffsets runtimeCorrectionOffsets;
        public Action<Vector3> OnAccelerationUpdated;

        public Vector3 Accelerations { get; set; }
        public Action<Quaternion> OnRotationUpdated;
        [HideInInspector] public Transform led;
        [HideInInspector] public Transform nodeMesh;
        [field: SerializeField] public NodeBinding NodeBinding { get; protected set; }

        
        public void SetNodeBinding(NodeBinding nodeBinding)
        {
            NodeBinding = nodeBinding;
            AxisNodeConverter.ConvertNode(nodeMesh, led, NodeBinding, runtimeCorrectionOffsets.rotationOffsets[NodeBinding]);
        }

        public virtual void SetRotation(Quaternion rotation)
        {
            Vector3 runtimeCorrectionEuler = runtimeCorrectionOffsets.rotationOffsets[NodeBinding];
            Quaternion runtimeCorrectionOffset = Quaternion.Euler(runtimeCorrectionEuler.x, runtimeCorrectionEuler.y, runtimeCorrectionEuler.z);
            // transform.localRotation = rotation * runtimeCorrectionOffset;
            transform.rotation = rotation;
            
        }

        public void SetAcceleration(Vector3 accelerations)
        {
            Accelerations = accelerations;
            OnAccelerationUpdated?.Invoke(accelerations);
        }         
    }

}
