using System.Collections.Generic;
using Axis.Bindings;
using Axis.DataProcessing;
using Axis.Elements.AnimatorLink;
using Axis.Enumerations;
using UnityEngine;

namespace Axis.Elements.AnimatorLink
{
    [DisallowMultipleComponent]
    public class GenericCharacterAnimatorLink : CharacterAnimatorLink
    {
        [HideInInspector] public CharacterTransformsToMannequinBindings characterBindedTransforms;

        public AxisBrainNodeBindings BrainNodeBindings
        {
            get { return characterBindedTransforms.axisBrainNodeBindings; }
            set
            {
                characterBindedTransforms.axisBrainNodeBindings = value;
                if (characterBindedTransforms.bindedTransforms.Count == 0)
                {
                    characterBindedTransforms.bindedTransforms = new List<Transform>();
                    for (int i = 0; i < characterBindedTransforms.axisBrainNodeBindings.nodeBindings.Count; i++)
                    {
                        characterBindedTransforms.bindedTransforms.Add(null);
                    }
                }
            }
        }

        public override void CreateControlledBones(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            Debug.Log(bodyModelAnimatorLink.gameObject.name);
            controlledBones = new Dictionary<HumanBodyBones, Transform>();
            int index = 0;
            Debug.Log(bodyModelAnimatorLink.transformsByNodeLimbs.Count);
            foreach (NodeBinding key in bodyModelAnimatorLink.transformsByNodeLimbs.Keys)
            {
                HumanBodyBones bodyBone = AxisDataUtility.ConvertAxisLimbToHumanBone(key);
                Transform bodyModelBone = bodyModelAnimatorLink.Animator.GetBoneTransform(bodyBone);

                if (index < characterBindedTransforms.bindedTransforms.Count && characterBindedTransforms.bindedTransforms[index] != null)
                {

                    Transform characterBone = characterBindedTransforms.bindedTransforms[index];
                    Transform controlBone = new GameObject($"Manequin {key} Control Transform {characterBone.name}").transform;

                    controlBone.transform.parent = characterBone.transform.parent;
                    controlBone.transform.rotation = bodyModelBone.transform.rotation;
                    controlBone.transform.position = characterBone.transform.position;
                    characterBone.parent = controlBone;
                    controlledBones.Add(bodyBone, controlBone.transform);

                }
                index++;
            }

        }

        public override void Initialize(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            base.Initialize(bodyModelAnimatorLink);
            transform.rotation = Quaternion.identity;
        }

        public override void UpdateControlledBones(Dictionary<NodeBinding, Transform> transformsByNodeBinding)
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
}

