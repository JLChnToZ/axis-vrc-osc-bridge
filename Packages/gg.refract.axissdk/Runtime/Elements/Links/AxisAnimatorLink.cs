using Axis.DataProcessing;
using Axis.Enumerations;
using Axis.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.Elements.AnimatorLink
{
    public class AxisAnimatorLink : MonoBehaviour
    {
        public Dictionary<NodeBinding, Transform> transformsByNodeLimbs;    
        private Animator animator;
        [HideInInspector] public AbsolutePositionSolver absolutePositionSolver;
        public Animator Animator
        {
            get
            {
                return animator;
            }
            set
            {
                animator = value;
            }
        }

        protected virtual void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        public void FetchTransformsRelatedToNodeLimbs()
        {         
            transformsByNodeLimbs = new Dictionary<NodeBinding, Transform>();
       
            foreach (NodeBinding nodeLimb in Enum.GetValues(typeof(NodeBinding)))
            {
                if(AxisDataUtility.IsMannequinBinding(nodeLimb))
                {
                    Animator = GetComponent<Animator>();
                    Transform animatorBone = Animator.GetBoneTransform(AxisDataUtility.ConvertAxisLimbToHumanBone(nodeLimb));
                    transformsByNodeLimbs.Add(nodeLimb, animatorBone);
                }
            
            }
        }
    }
}


