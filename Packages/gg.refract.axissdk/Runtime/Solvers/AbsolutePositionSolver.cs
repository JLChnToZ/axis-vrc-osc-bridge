using System;
using System.Collections.Generic;
using Axis.Constrains;
using Axis.DataTypes;
using Axis.Elements.AnimatorLink;
using Axis.Overrides;
using UnityEngine;

namespace Axis.Solvers
{

    public abstract class AbsolutePositionSolver : MonoBehaviour
    {
        [Range(-10, 10)] public float xOffset;
        [Range(-10, 10)] public float zOffset;

        public bool GenerateOffsetFromStartingPosition = false;
        public virtual void HandleOnHubDataUpdated(AxisHubData hubData) { }
        public virtual void UpdateModelsData(BodyModelAnimatorLink bodyModel, AxisAnimatorLink characterAnimatorLink) { }
        public virtual void SolveAbsolutePosition(Transform character) { }

        public GroundContraint[] groundContraints;
        
        public Vector3 GetOffset()
        {
            return new Vector3(xOffset, 0f, zOffset);
        }

        protected virtual void Awake()
        {
            groundContraints = gameObject.GetComponents<GroundContraint>();
            
            if (GenerateOffsetFromStartingPosition == true)
            {
                xOffset = transform.position.x;
                zOffset = transform.position.z;

                
            }
        }

        

        protected void ApplyOffset()
        {
            transform.position += new Vector3(xOffset, 0f, zOffset);
        }


    }
}



