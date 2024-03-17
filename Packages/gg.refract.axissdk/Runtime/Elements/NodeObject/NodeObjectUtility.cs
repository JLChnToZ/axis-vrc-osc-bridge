using Axis.Enumerations;
using Axis.Elements.AnimatorLink;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.Elements
{
    public static class NodeObjectUtility
    {
        private static List<GrabbableObjectLimbRelation> grabberLimbs = new List<GrabbableObjectLimbRelation>();

        public static GrabbableObjectLimbRelation GetGrabberLimbByNodeBinding(NodeBinding nodeBinding)
        {
            if(grabberLimbs.Count != 2)
            {
                return null;
            }

            if(nodeBinding == NodeBinding.RightHand)
            {
                return grabberLimbs[0];
            }

            if(nodeBinding == NodeBinding.LeftHand)
            {
                return grabberLimbs[1];
            }

            return null;
        }


        public static void RegisterGrabberLimb(NodeBinding nodeLimb, Transform grabberLimb, HumanoidCharacterAnimatorLink characterAnimatorLink)
        {
            GrabbableObjectLimbRelation grabbableObjectLimbRelation = new GrabbableObjectLimbRelation() {
                limbTransform = grabberLimb,
                characterAnimatorLink = characterAnimatorLink,
                nodeLimb = nodeLimb
            
            };

            
            grabberLimbs.Add(grabbableObjectLimbRelation);
        }

        public static GrabbableObjectLimbRelation GetClosestGrabberLimb(Vector3 position)
        {

            float closestDistance = Mathf.Infinity;
            int indexOfTheClosestGrabberLimb = -1;
            for (int i = 0; i < grabberLimbs.Count; i++)
            {
                float distance = Vector3.Distance(grabberLimbs[i].limbTransform.position, position);
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    indexOfTheClosestGrabberLimb = i;
                }
            }
            

            return indexOfTheClosestGrabberLimb != -1? grabberLimbs[indexOfTheClosestGrabberLimb] : null;
        }
    }
}

