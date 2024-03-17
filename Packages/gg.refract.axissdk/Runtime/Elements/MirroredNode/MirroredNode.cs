using Axis.Enumerations;
using Axis.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.Elements.MirroredNode
{
    [ExecuteAlways, Serializable]
    public class MirroredNode : MonoBehaviour
    {
        public AxisBrain connectedBrain;
        private string brainUniqueId;
        public bool updateFromAxis = true;
        public MirroredNodeBindingMode bindingSpace = MirroredNodeBindingMode.Local;
       

        [HideInInspector] public Vector3 rotationOffset;
        [HideInInspector] public MirroredNodeVisibility nodeVisibility;

        [HideInInspector] public Quaternion defaultRotation = Quaternion.identity;

        [SerializeField, HideInInspector] private NodeBinding bindedToNode;

        
        public NodeBinding BindedToNode
        {
            get
            {
                return bindedToNode;
            }
            set
            {

                bindedToNode = value;
                
                nodeVisibility.UpdateMeshAfterNodeBinding(bindedToNode);
            }
        }


        private void Start()
        {
            connectedBrain = connectedBrain == null ? AxisBrain.FetchBrainOnScene() : connectedBrain;
            brainUniqueId = connectedBrain.uniqueID;
            nodeVisibility = nodeVisibility == null ? gameObject.AddComponent<MirroredNodeVisibility>() : nodeVisibility;
            
            //Transform controlledTransform = transform.GetChild(1);
            //defaultRotation = controlledTransform.localRotation;
            
        }

        


        private void OnEnable()
        {
            AxisEvents.OnNodeByLimbsUpdated += HandleOnNodeByLimbsUpdated;

        }

        private void OnDisable()
        {
            AxisEvents.OnNodeByLimbsUpdated -= HandleOnNodeByLimbsUpdated;
            transform.localRotation = defaultRotation;
        }

        private void HandleOnNodeByLimbsUpdated(string _brainUniqueId, Dictionary<NodeBinding, AxisNode> nodesByLimbs)
        {
            if (IsFromConnectedBrain(_brainUniqueId))
            {

                if (nodesByLimbs.ContainsKey(bindedToNode))
                {
                    if(updateFromAxis == true)
                    {
                        if (bindingSpace == MirroredNodeBindingMode.Local)
                        {
                            

                            Vector3 transformedUpDirection = transform.root.TransformDirection(nodesByLimbs[bindedToNode].transform.up);
                            Vector3 transformedForwardDirection = transform.root.TransformDirection(nodesByLimbs[bindedToNode].transform.forward);


                            transform.rotation = Quaternion.LookRotation(transformedForwardDirection, transformedUpDirection);
                            transform.localRotation = transform.localRotation * Quaternion.Euler(rotationOffset);


                        }
                        else
                        {
                            transform.rotation = nodesByLimbs[bindedToNode].transform.localRotation * Quaternion.Euler(rotationOffset);
                        }
                    }
                    else
                    {
                        transform.localRotation = defaultRotation;
                    }
                    
                }
            }
        }



        private bool IsFromConnectedBrain(string _brainUniqueId)
        {
            return string.Equals(brainUniqueId, _brainUniqueId);
        }
    }

}
