using Axis.DataProcessing;
using Axis.DataTypes;
using Axis.Enumerations;
using Axis.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Axis.Elements.AnimatorLink
{

    [ExecuteAlways]
    public class BodyModelAnimatorLink : AxisAnimatorLink
    {
        public PoseStorage tPoseLocalRotations;
        
        private Dictionary<NodeBinding, Quaternion> defaultLimbsRotations;
        private Dictionary<NodeBinding, Quaternion> lastLimbRotations;

        private HumanPose humanPose;
        private HumanPoseHandler humanPoseHandler;

        [HideInInspector] public Rigidbody[] limbsRigidBodies;
        [HideInInspector] public bool enforceDefaultRotation;
        [HideInInspector] public bool storeTPoseRotations;


        public void GoToTPose()
        {
            foreach (HumanBodyBones humanBodyBone in tPoseLocalRotations.poseLocalRotations.Keys)
            {
                Animator.GetBoneTransform(humanBodyBone).localRotation = tPoseLocalRotations.poseLocalRotations[humanBodyBone];
            }
        }

        protected override void Awake()
        {
            base.Awake();

            DisableColliders();
           
            humanPoseHandler = new HumanPoseHandler(Animator.avatar, transform);
            limbsRigidBodies = GetComponentsInChildren<Rigidbody>();
        }

        private void DisableColliders()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }
        internal bool CheckNeedToSkipWaistBinding(NodeBinding limb, HipProvider hipProvider)
        {
            
            switch (hipProvider)
            {
                case HipProvider.Hub:
                    return true;
            
                case HipProvider.Node:
                    return false;
            
                default: 
                    return false;
            }
           
        }
        internal bool CheckIfSkipHip(NodeBinding limb)
        {
            return limb == NodeBinding.Hips;
        }
        public virtual void UpdateTransforms(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            foreach (NodeBinding limb in AxisDataUtility.NodeBindingInOrder)
            {
                if (transformsByNodeLimbs.ContainsKey(limb) == false) continue;
                if (CheckIfSkipHip(limb) == true) continue;

                var nodeTransform = nodesByLimb[limb].transform;

                
                transformsByNodeLimbs[limb].rotation = nodeTransform.rotation * defaultLimbsRotations[limb];
                         

                if (enforceDefaultRotation == true)
                {
                    GoToTPose();                   
                }                      
                lastLimbRotations[limb] = transformsByNodeLimbs[limb].rotation;                     
                             
            }
        }
        public virtual void UpdateHipTransform(Dictionary<NodeBinding, AxisNode> nodesByLimb, HipProvider hipProvider,AxisHubData axisHubData)
        {
            var hip = NodeBinding.Hips;
            switch (hipProvider)
            {
                case HipProvider.Hub:
                    transformsByNodeLimbs[hip].rotation = AxisDataUtility.ConvertHubRotationToUnitySpace(axisHubData) * defaultLimbsRotations[hip];
                    break;
                case HipProvider.Node:
                    transformsByNodeLimbs[hip].rotation = nodesByLimb[hip].transform.rotation * defaultLimbsRotations[hip];
                    break;
                default: 
                    break;
            }

        }
        internal void StoreDefaultTransformsRotations()
        {
            lastLimbRotations = new Dictionary<NodeBinding, Quaternion>();
            defaultLimbsRotations = new Dictionary<NodeBinding, Quaternion>();
            foreach (var limb in transformsByNodeLimbs.Keys)
            {
                defaultLimbsRotations.Add(limb, transformsByNodeLimbs[limb].rotation);
                lastLimbRotations.Add(limb, transformsByNodeLimbs[limb].rotation);
            }
        }      

        internal HumanPose GetHumanPose()
        {
            if(humanPoseHandler == null)
            {
                if(Animator == null)
                {
                    Animator = GetComponent<Animator>();
                }
                humanPoseHandler = new HumanPoseHandler(Animator.avatar, transform);
            }
            humanPoseHandler.GetHumanPose(ref humanPose);
            return humanPose;
        }

    }
}


