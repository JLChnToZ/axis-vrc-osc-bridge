using System;

namespace Axis.Events
{
    using System.Collections.Generic;
    using Axis.DataTypes;
    using Axis.Elements;
    using Axis.Utils;
    using Enumerations;
    using UnityEngine;



    public static class AxisEvents
    {

        //This Action is raised whenever there is new data from the Axis System
        //Check AxisOutputData to understand how the data is organized.
        public static Action<AxisOutputData> OnAxisOutputDataUpdated;
        public static Action<string, Dictionary<NodeBinding, AxisNode>> OnNodeByLimbsUpdated;

        #region Commands To Nodes


        //Example OnSetNodeVibration.Invoke(nodeIndex, intensity, duration) where:
        //nodeIndex: Check Enumerations.cs for relation of nodeIndex and default positions
        //intensity -> from 0 to 1
        //duration -> in seconds
        public static Action<int, float, float> OnSetNodeVibration;

        //Example OnSetNodeLedColor.Invoke(nodeIndex, Color.red, brightness) where:
        //nodeIndex: Check Enumerations.cs for relation of nodeIndex and default positions
        //Color32 -> auto conversion from Color
        //brightness -> from 0 to 1
        public static Action<int, Color32, float> OnSetNodeLedColor;

        public static Action OnZeroAll;
        public static Action OnCalibration;
        public static Action OnStartStreaming;
        public static Action OnSinglePoseCalibration;

        #endregion

    }
}
