using System;
using System.Collections.Generic;
using Axis.Enumerations;
using Axis.Utils;
using UnityEngine;

namespace Axis.Elements
{
    [ExecuteAlways]
    public class AxisNodesRepresentation : NodeProcessor
    {
        
        public Transform nodeWorldOrientations;
        public Dictionary<NodeBinding, AxisNode> nodesByLimb;
        

        public GameObject nodePrefab;
        public void SetVisibility(bool isVisible)
        {
            foreach (Renderer renderer in nodeWorldOrientations.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }
        }
        public override void Initialize(string uniqueId)
        {

            if(transform.childCount ==0)
            {
                InstantiateNodes();///
            }
            else
            {
                FetchNodesDictionary();
            }

        }

        private void FetchNodesDictionary()
        {
            AxisNode[] axisNodes = transform.GetComponentsInChildren<AxisNode>();
            nodesByLimb = new Dictionary<NodeBinding, AxisNode>();
            foreach (AxisNode axisNode in axisNodes)
            {
                nodesByLimb.Add(axisNode.NodeBinding, axisNode);
            }
        }

        private void InstantiateNodes()
        {
            nodeWorldOrientations = new GameObject("Nodes Orientation Relative To Hub").transform;
            nodeWorldOrientations.parent = transform;
            
            
            nodesByLimb = new Dictionary<NodeBinding, AxisNode>();

            foreach (NodeBinding nodeLimb in Enum.GetValues(typeof(NodeBinding)))
            {
                InstantiateNode(nodeLimb);
            }
            SetupNodesDefaultPositions(nodesByLimb);
            SetupNodesParenting(nodesByLimb);
        }


        private void InstantiateNode(NodeBinding nodeBinding)
        {
            var nodeInstance = Instantiate(nodePrefab);
            nodeInstance.name = nodeBinding.ToString() + "_Node";
            AxisNode axisNode = nodeInstance.GetComponent<AxisNode>();
            axisNode.SetNodeBinding(nodeBinding);
            nodesByLimb.Add(nodeBinding, axisNode);
        }

        private void SetupNodesDefaultPositions(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            nodesByLimb[NodeBinding.LeftThigh].transform.localPosition = new Vector3(-1, -1, 0) * 0.1f;
            nodesByLimb[NodeBinding.RightThigh].transform.localPosition = new Vector3(1, -1, 0) * 0.1f;
            nodesByLimb[NodeBinding.LeftCalf].transform.localPosition = new Vector3(-1, -2, 0) * 0.1f;
            nodesByLimb[NodeBinding.RightCalf].transform.localPosition = new Vector3(1, -2, 0) * 0.1f;
            nodesByLimb[NodeBinding.LeftUpperArm].transform.localPosition = new Vector3(-2, 1, 0) * 0.1f;
            nodesByLimb[NodeBinding.LeftForeArm].transform.localPosition = new Vector3(-2, 0, 0) * 0.1f;
            nodesByLimb[NodeBinding.RightUpperArm].transform.localPosition = new Vector3(2, 1, 0) * 0.1f;
            nodesByLimb[NodeBinding.RightForeArm].transform.localPosition = new Vector3(2, 0, 0) * 0.1f;
        }

        private void SetupNodesParenting(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            foreach (var node in nodesByLimb)
            {
                node.Value.transform.parent = nodeWorldOrientations;
            }
        }
    }
}

