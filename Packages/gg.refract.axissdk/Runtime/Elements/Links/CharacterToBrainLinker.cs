using System;
using System.Collections;
using System.Collections.Generic;
using Axis.DataTypes;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using UnityEngine;




namespace Axis.Elements.Linker
{
    [AddComponentMenu("Axis/Core/CharacterToBrainLinker", 0), RequireComponent(typeof(Animator))]
    public class CharacterToBrainLinker : MonoBehaviour
    {
        public AxisBrain linkedBrain;
        public CharacterAnimatorLink characterAnimatorLink;

        private void Start()
        {
            linkedBrain = linkedBrain == null ? AxisBrain.FetchBrainOnScene() : linkedBrain;        
            linkedBrain.axisMannequin.onBodyModelAnimatorLinkUpdated += HandleOnBodyAnimatorLinkUpdated;
            linkedBrain.axisMannequin.onHubDataUpdated += HandleOnHubDataUpdated;
            
            Animator characterAnimator = gameObject.GetComponent<Animator>();
            
            if(characterAnimator == null)
            {
                throw new Exception("Character must have an Animator");
            }

            characterAnimatorLink = characterAnimator.isHuman == true?
                gameObject.AddComponent<HumanoidCharacterAnimatorLink>():
                gameObject.GetComponent<GenericCharacterAnimatorLink>();
           
            characterAnimatorLink.Initialize(linkedBrain.axisMannequin.bodyModelAnimatorLink);
        
        }

        private void HandleOnHubDataUpdated(AxisHubData hubData)
        {
            characterAnimatorLink.absolutePositionSolver?.HandleOnHubDataUpdated(hubData);
        }

        private void HandleOnBodyAnimatorLinkUpdated(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            characterAnimatorLink.HandleNodeDataUpdated(bodyModelAnimatorLink);
        }

    }
}

