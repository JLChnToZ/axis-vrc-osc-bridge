using System.Collections;
using System.Collections.Generic;
using Axis.Elements;
using UnityEngine;

namespace Axis.Overrides
{
    [AddComponentMenu("Axis/Overrides/MannequinPositionOverride")]
    public class MannequinPositionOverride : AxisExecuteOnStartOverride
    {
        public Vector3 position;
        public bool generateOffsetFromStartPosition;


        public override void Execute()
        {

            transform.position = position;
        }

        private void Awake()
        {
            if (generateOffsetFromStartPosition == true)
            {
                position = transform.position;
            }
        }
        //Helper for setting Character Bot Color
        private void Start()
        {

            if (GetComponent<AxisMannequin>() == null)
            {
                Execute();
            }
        }
    }

}
