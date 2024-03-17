using Axis.Elements.AnimatorLink;
using UnityEngine;
using Axis.Utils;
using Axis.ScriptableObjects;
using Axis.Enumerations;

namespace Axis.Elements.FreeNodes
{
    public enum AttachmentMode
    {
        AttachedToRightHand,
        AttachedToLeftHand,
        Dynamic
    }
    public class NodeObject : AxisNode
    {

        //[Range(0, 0.01f)] public float rotationThreshold = 0f;
        float previousRotation;
        int noRotationCounter = 0;
        //public int noRotationCounterThreshold = 1000; 
        private MeshRenderer meshRenderer;
        internal MotionDetectionParameters motionDetectionParameters;

        private Transform defaultParent;

        public AttachmentMode attachmentMode;
        Transform bindedLimb;

        private void Awake()
        {
            meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            
        }

        private void Start()
        {
            defaultParent = transform.parent;
        }

        public override void SetRotation(Quaternion rotation)
        {
            base.SetRotation(rotation);

            switch (attachmentMode)
            {
                case AttachmentMode.AttachedToRightHand:
                    {
                        GrabbableObjectLimbRelation grabberLimb = NodeObjectUtility.GetGrabberLimbByNodeBinding(NodeBinding.RightHand);
                        bindedLimb = grabberLimb.limbTransform;
                        UpdateObjectPosition();
                        UpdateLimbRotation();
                    }
                    break;
                case AttachmentMode.AttachedToLeftHand:
                    {
                        GrabbableObjectLimbRelation grabberLimb = NodeObjectUtility.GetGrabberLimbByNodeBinding(NodeBinding.LeftHand);
                        bindedLimb = grabberLimb.limbTransform;
                        UpdateObjectPosition();
                        UpdateLimbRotation();
                    }
                    break;
                case AttachmentMode.Dynamic:

                    if (IsObjectStatic())
                    {

                        meshRenderer.material.color = Color.red;
                    }
                    else
                    {
                        Debug.Log("Not static");
                        if (IsDefaultParent())
                        {
                            GrabbableObjectLimbRelation grabbableObjectLimbRelation = NodeObjectUtility.GetClosestGrabberLimb(transform.position);
                            //transform.parent = grabbableObjectLimbRelation.limbTransform;                    

                            bindedLimb = grabbableObjectLimbRelation.limbTransform;



                            UpdateObjectPosition();
                            UpdateLimbRotation();


                            //transform.localPosition = Vector3.zero;
                        }
                        else
                        {
                            Debug.Log("not default parent");
                            UpdateObjectPosition();
                            UpdateLimbRotation();
                        }


                        meshRenderer.material.color = Color.green;
                    }

                    break;
                default:
                    break;
            }

            


            //previousRotation = transform.rotation.x;

        }

        private void UpdateObjectPosition()
        {
            transform.position = bindedLimb.TransformPoint(new Vector3(0f, 0.05f, 0f));
        }

        private void UpdateLimbRotation()
        {
            bindedLimb.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            //Debug.Log($"Must update {bindedLimb.name}".Color("aqua"));
        }

        private bool IsDefaultParent()
        {
            return transform.parent == defaultParent;
        }

        private bool IsObjectStatic()
        {
            var rotationDifference = Mathf.Abs(transform.rotation.x - previousRotation);
            previousRotation = transform.rotation.x;
            //Debug.Log("Inside is Object Static".Color("orange"));

            //Debug.Log($"Rotation Difference {rotationDifference} Rotation Diff Threshold {motionDetectionParameters.rotationThreshold}".Color("white"));

            if (rotationDifference < motionDetectionParameters.rotationThreshold)
            {
                noRotationCounter++;
                //Debug.Log("Reset Rotation".Color("yellow"));
                
                //return false;
            }
            else
            {
                //Debug.Log($"Rotation Counter {noRotationCounter}".Color("green"));
                noRotationCounter = 0;
            }
            //Grapher.Log(rotationDifference * 100f, "Rotation Difference", Color.cyan);

            //Debug.Log($"No Rotation Counter {noRotationCounter} No Rotation Counter Threshold {motionDetectionParameters.noRotationCounterThreshold}".Color("lime"));

            return noRotationCounter > motionDetectionParameters.noRotationCounterThreshold;
        }
    }
}


