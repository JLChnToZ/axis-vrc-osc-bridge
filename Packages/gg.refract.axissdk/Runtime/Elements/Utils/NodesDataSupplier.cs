using Axis.Enumerations;
using System;
using System.Collections.Generic;


using Axis.DataTypes;
using Axis.DataProcessing;
using UnityEngine;

namespace Axis.Utils
{
    public static class NodesDataSupplier
    {
        public static Dictionary<NodeBinding, AxisNodeData> GetDefaultNodeDataDictionary(AxisOutputData nodesDataList)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = new Dictionary<NodeBinding, AxisNodeData>();
            for (int i = 0; i < nodesDataList.nodesDataList.Count; i++)
            {
                nodesDataByNodeLimbs.Add((NodeBinding)i, nodesDataList.nodesDataList[i]);
            }
            return nodesDataByNodeLimbs;
        }

        public static Dictionary<NodeBinding, AxisNodeData> GetLeftUpperArmAsHeadNodeDataDictionary(AxisOutputData nodesData)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = new Dictionary<NodeBinding, AxisNodeData>();
            for (int i = 0; i < nodesData.nodesDataList.Count; i++)
            {
                NodeBinding nodeLimb = (NodeBinding)i;
                if (nodeLimb == NodeBinding.LeftUpperArm)
                {
                    nodeLimb = NodeBinding.Head;
                }
                nodesDataByNodeLimbs.Add(nodeLimb, nodesData.nodesDataList[i]);
            }
            return nodesDataByNodeLimbs;
        }

        public static Dictionary<NodeBinding, AxisNodeData> GetRightUpperArmAsHeadNodeDictionary(AxisOutputData nodesDataList)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = NodesDataSupplier.GetDefaultNodeDataDictionary(nodesDataList);
            nodesDataByNodeLimbs.Add(NodeBinding.Head, nodesDataList.nodesDataList[(int)NodeBinding.RightUpperArm]);
            nodesDataByNodeLimbs.Remove(NodeBinding.RightUpperArm);
            return nodesDataByNodeLimbs;
        }

        public static Dictionary<NodeBinding, AxisNodeData> GetForearmAsFeetNodeDataDictionary(AxisOutputData nodesDataList)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = new Dictionary<NodeBinding, AxisNodeData>();
            for (int i = 0; i < nodesDataList.nodesDataList.Count; i++)
            {
                NodeBinding nodeLimb = (NodeBinding)i;

                if (nodeLimb == NodeBinding.LeftForeArm)
                {
                    nodeLimb = NodeBinding.LeftFoot;
                }

                if (nodeLimb == NodeBinding.RightForeArm)
                {
                    nodeLimb = NodeBinding.RightFoot;
                }

                nodesDataByNodeLimbs.Add(nodeLimb, nodesDataList.nodesDataList[i]);
            }

            return nodesDataByNodeLimbs;
        }

        public static List<AxisNodeData> GetLeftForeArmAsGrabbableNodeList(AxisOutputData nodesData)
        {
            List<AxisNodeData> freeNodesData = new List<AxisNodeData>();
            int indexOfForeArm = (int)NodeBinding.LeftUpperArm;
            freeNodesData.Add(nodesData.nodesDataList[indexOfForeArm]);
            return freeNodesData;
        }

        internal static Dictionary<NodeBinding, AxisNodeData> GetNodeObjecstData(AxisOutputData axisOutputData, List<NodeBinding> nodeBindings)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = new Dictionary<NodeBinding, AxisNodeData>();

            for (int i = 0; i < axisOutputData.nodesDataList.Count; i++)
            {
                if(i < nodeBindings.Count)
                {
                    if (AxisDataUtility.IsNodeObjectBinding(nodeBindings[i]))
                    {
                        nodesDataByNodeLimbs.Add(nodeBindings[i], axisOutputData.nodesDataList[i]);
                    }
                }
                
            }

            return nodesDataByNodeLimbs;
        }

        internal static Dictionary<NodeBinding, AxisNodeData> GetMannequinNodesData(AxisOutputData axisOutputData, List<NodeBinding> nodeBindings)
        {
            Dictionary<NodeBinding, AxisNodeData> nodesDataByNodeLimbs = new Dictionary<NodeBinding, AxisNodeData>();
            for (int i = 0; i < axisOutputData.nodesDataList.Count; i++)
            {
                if(i < nodeBindings.Count)
                {
                    if (AxisDataUtility.IsMannequinBinding(nodeBindings[i]))
                    {

                        nodesDataByNodeLimbs.Add(nodeBindings[i], axisOutputData.nodesDataList[i]);
                    }
                }
                           
            }            

            return nodesDataByNodeLimbs;
        }
    }

}

