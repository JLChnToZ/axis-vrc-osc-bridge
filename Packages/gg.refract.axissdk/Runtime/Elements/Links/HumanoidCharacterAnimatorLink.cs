using Axis.Enumerations;
using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.DataProcessing;


namespace Axis.Elements.AnimatorLink
{

    public class HumanoidCharacterAnimatorLink : CharacterAnimatorLink
    {
        #region Class Variables

        private Dictionary<int, Quaternion> tPoseLocalRotations;
        private Dictionary<int, Vector3> bonesInitialPositions;
        private HumanPoseHandler humanPoseHandler;


        #endregion

        #region Initialization
        public override void Initialize(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            
            //Debug.Log($"Initializing {gameObject.name}");
            transform.rotation = Quaternion.identity;
            Animator = GetComponent<Animator>();
            
            EnforceTPoseFromMannequin(bodyModelAnimatorLink);
            
            //CreateControlledBones(bodyModelAnimatorLink);
            FetchTransformsRelatedToNodeLimbs();
            SaveTransformsValues();

            base.Initialize(bodyModelAnimatorLink);
            //initialized = true;

        }

        protected override void OnEnable()
        {
            base.OnEnable();
            LoadTPoseRotations();
        }


        //This is to enforce the character on T Pose (the same as the bodyModelAnimatorLink) for then creating the Control Bones on a known orientation
        internal void EnforceTPoseFromMannequin(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            HumanPose bodyModelHumanPose = bodyModelAnimatorLink.GetHumanPose();
            HumanPose humanPoseToUpdate = bodyModelHumanPose;
            humanPoseHandler.SetHumanPose(ref humanPoseToUpdate);
        }

        //Control Bones are created and parenting right above the actual character bones. This is to solve the problem related to different characters models having 
        //different limbs orientations
        public override void CreateControlledBones(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            controlledBones = new Dictionary<HumanBodyBones, Transform>();
            InstantiateControlledBones(bodyModelAnimatorLink);
            SetParenting();
            
        }

        private void SetParenting()
        {
            foreach (var key in controlledBones.Keys)
            {
                Transform characterBone = Animator.GetBoneTransform(key);

                Quaternion rotation = characterBone.rotation;
                Vector3 position = characterBone.position;

                Animator.GetBoneTransform(key).SetParent(controlledBones[key], true);
                characterBone.position = position;
                characterBone.rotation = rotation;
            }
        }

        private void InstantiateControlledBones(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            foreach (HumanBodyBones bodyBone in Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bodyBone == HumanBodyBones.LastBone)
                {
                    continue;
                }
                Transform bodyModelBone = bodyModelAnimatorLink.Animator.GetBoneTransform(bodyBone);
                if (bodyModelBone != null)
                {
                    Transform characterBone = Animator.GetBoneTransform(bodyBone);
                    if (characterBone != null)
                    {
                        GameObject controlBone = new GameObject($"Control Bone {bodyBone}");
                        controlBone.transform.parent = characterBone.transform.parent;
                        controlBone.transform.rotation = bodyModelBone.transform.rotation;
                        controlBone.transform.position = characterBone.transform.position;
                        controlledBones.Add(bodyBone, controlBone.transform);                    
                    }
                }
            }
        }

        
        

        protected override void Awake()
        {
            base.Awake();

            NodeObjectUtility.RegisterGrabberLimb(NodeBinding.RightHand, Animator.GetBoneTransform(HumanBodyBones.RightHand), this);
            NodeObjectUtility.RegisterGrabberLimb(NodeBinding.LeftHand, Animator.GetBoneTransform(HumanBodyBones.LeftHand), this);
            humanPoseHandler = new HumanPoseHandler(Animator.avatar, transform);
        }


        #endregion

        #region Utils
        private void SaveTransformsValues()
        {
            tPoseLocalRotations = new Dictionary<int, Quaternion>();
            bonesInitialPositions = new Dictionary<int, Vector3>();
            var allTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in allTransforms)
            {
                SaveTPoseLocalRotation(t);
                SaveBoneGlobalPosition(t);
            }
        }

        private void SaveBoneGlobalPosition(Transform t)
        {
            bonesInitialPositions.Add(t.GetInstanceID(), t.position);
        }

        private void SaveTPoseLocalRotation(Transform t)
        {
            tPoseLocalRotations.Add(t.GetInstanceID(), t.localRotation);
        }


        private void LoadTPoseRotations()
        {
            if (tPoseLocalRotations == null)
            {
                return;
            }

            var allTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (tPoseLocalRotations.ContainsKey(t.GetInstanceID()))
                {
                    t.localRotation = tPoseLocalRotations[t.GetInstanceID()];
                }
            }
        }

        #endregion

        #region Updating

        public override void UpdateControlledBones(Dictionary<NodeBinding, Transform> transformsByNodeBinding)
        {    
            if (enabled == true)
            {          
                foreach (var nodeBinding in AxisDataUtility.NodeBindingInOrder)
                {
                    if (controlledBones.ContainsKey(AxisDataUtility.ConvertAxisLimbToHumanBone(nodeBinding)) && transformsByNodeBinding.ContainsKey(nodeBinding))
                    {
                        Vector3 transformedUpDirection = transform.TransformDirection(transformsByNodeBinding[nodeBinding].up);
                        Vector3 transformedForwardDirection = transform.TransformDirection(transformsByNodeBinding[nodeBinding].forward);
                        controlledBones[AxisDataUtility.ConvertAxisLimbToHumanBone(nodeBinding)].rotation = Quaternion.LookRotation(transformedForwardDirection, transformedUpDirection);
                    }

                }
            }
        }

        #endregion
    }
}


