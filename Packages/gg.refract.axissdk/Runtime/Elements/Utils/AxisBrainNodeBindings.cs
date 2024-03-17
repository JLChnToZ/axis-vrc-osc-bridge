using System;
using System.Collections.Generic;
using Axis.Enumerations;
using UnityEngine;

namespace Axis.Bindings
{
    [DisallowMultipleComponent, AddComponentMenu("Axis/Overrides/AxisBrainNodeBindings"), Serializable]
    public class AxisBrainNodeBindings : MonoBehaviour
    {
        public List<NodeBinding> nodeBindings = new List<NodeBinding>
            {
                NodeBinding.RightThigh,
                NodeBinding.RightCalf,
                NodeBinding.LeftThigh,
                NodeBinding.LeftCalf,
                NodeBinding.RightUpperArm,
                NodeBinding.RightForeArm,
                NodeBinding.LeftUpperArm,
                NodeBinding.LeftForeArm,
                NodeBinding.Chest,
                NodeBinding.RightFoot,
                NodeBinding.LeftFoot,
                NodeBinding.RightHand,
                NodeBinding.LeftHand,
                NodeBinding.RightShoulder,
                NodeBinding.LeftShoulder,
                NodeBinding.Head,
                NodeBinding.Hips
            };
    }

    [DisallowMultipleComponent, AddComponentMenu("Axis/Overrides/AxisBrainNodeBindings"), Serializable]
    public class CharacterTransformsToMannequinBindings
    {
        public AxisBrainNodeBindings axisBrainNodeBindings;
        public List<Transform> bindedTransforms = new List<Transform>();


    }
}

