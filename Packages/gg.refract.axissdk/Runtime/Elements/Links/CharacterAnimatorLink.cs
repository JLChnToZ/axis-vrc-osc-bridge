using Axis.Enumerations;
using System.Collections.Generic;
using UnityEngine;
using Axis.Solvers;
using Axis.Overrides;

namespace Axis.Elements.AnimatorLink
{
    public abstract class CharacterAnimatorLink : AxisAnimatorLink
    {

        public abstract void CreateControlledBones(BodyModelAnimatorLink bodyModelAnimatorLink);
        public abstract void UpdateControlledBones(Dictionary<NodeBinding, Transform> transformsByNodeBinding);
        
        public Dictionary<HumanBodyBones, Transform> controlledBones;
        [HideInInspector] public Vector3 initialPosition = Vector3.positiveInfinity;
        private CharacterRotationOverride rotationOverride = null;
        public void HandleNodeDataUpdated(BodyModelAnimatorLink bodyModel)
        {
            UpdateControlledBones(bodyModel.transformsByNodeLimbs);
            
            if (absolutePositionSolver != null)
            {
                absolutePositionSolver.UpdateModelsData(bodyModel, this);
            
            }
        }

        public virtual void Initialize(BodyModelAnimatorLink bodyModelAnimatorLink) 
        {
            bodyModelAnimatorLink.GoToTPose();
            LoadOverrides();
            CreateControlledBones(bodyModelAnimatorLink);           
        }

        protected virtual void OnEnable()
        {
            LoadSolvers();           
        }

        private void LoadSolvers()
        {
            absolutePositionSolver = GetComponent<AbsolutePositionSolver>();
            if (absolutePositionSolver == null)
            {
                absolutePositionSolver = gameObject.AddComponent<AxisHubPositionSolver>();
            }
        }

        private void LateUpdate()
        {
            absolutePositionSolver.SolveAbsolutePosition(transform);           
            if (rotationOverride != null)
            {
                Vector3 rotation = new Vector3(
                    rotationOverride.xOffset,
                    rotationOverride.yOffset,
                    rotationOverride.zOffset);
            
                transform.rotation = Quaternion.Euler(rotation);
            }
            StoreInitialPosition();
        }

        private void StoreInitialPosition()
        {
            initialPosition = initialPosition.x == Mathf.Infinity ? 
                initialPosition = transform.position : initialPosition;
        }

        protected void LoadOverrides()
        {
            rotationOverride = GetComponent<CharacterRotationOverride>();
        }

    }
}


