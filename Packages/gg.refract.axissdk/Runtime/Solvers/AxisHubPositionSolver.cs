using Axis.Constrains;
using Axis.DataTypes;
using Axis.Elements.AnimatorLink;
using Axis.Overrides;
using UnityEngine;



namespace Axis.Solvers
{
    [AddComponentMenu("Axis/Solvers/AxisHubPositionSolver")]
    public class AxisHubPositionSolver : AbsolutePositionSolver
    {
        bool initialValueSet = false;
        public Vector3 hubZeroPosition = Vector3.positiveInfinity;
        public Vector3 hubAbsolutePosition;

        protected override void Awake()
        {
            base.Awake();

            foreach (GroundContraint groundContraint in groundContraints)
            {
                groundContraint.OnSetVerticalZeroPosition += HandleOnSetVerticalZeroPosition;
            }          
        }

        private void HandleOnSetVerticalZeroPosition(float yZeroPosition)
        {
            hubZeroPosition.y += yZeroPosition;
        }
        public override void HandleOnHubDataUpdated(AxisHubData hubData)
        {
            hubAbsolutePosition = hubData.absolutePosition;
            
            if (initialValueSet == false)
            {               
                if (hubZeroPosition.x == Mathf.Infinity)
                {
                    hubZeroPosition = hubAbsolutePosition;
                    initialValueSet = true;
                }
            }
            
            
        }

        public override void SolveAbsolutePosition(Transform character)
        {
            if(initialValueSet == true)
            {
                character.position = Vector3.zero;


                Vector3 differenceFromStartingPosition = hubAbsolutePosition - hubZeroPosition;
                Vector3 calculatedPosition =
                character.right * hubAbsolutePosition.x +
                character.up * hubAbsolutePosition.y +
                character.forward * hubAbsolutePosition.z;
                //character.right * differenceFromStartingPosition.x +
                //character.up * differenceFromStartingPosition.y +
                //character.forward * differenceFromStartingPosition.z;

                character.position = hubZeroPosition + calculatedPosition;

                ApplyOffset();

                foreach (GroundContraint groundContraint in groundContraints)
                {
                    groundContraint.Contraint();
                }
                
                
            }
            
        }

        public override void UpdateModelsData(BodyModelAnimatorLink bodyModel, AxisAnimatorLink characterAnimatorLink) { }

    }

}
