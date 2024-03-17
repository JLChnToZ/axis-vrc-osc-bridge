using UnityEngine;

namespace Axis.Overrides
{
    [DisallowMultipleComponent]
    public class CharacterRotationOverride : AxisOverrides
    {
        //public Vector3 rotation;
        [Range(0, 360)] public float xOffset;
        [Range(0, 360)] public float yOffset;
        [Range(0, 360)] public float zOffset;

        public bool generateOffsetFromStartingPosition = false;

        private void Awake()
        {
            if (generateOffsetFromStartingPosition == true)
            {
                xOffset = transform.eulerAngles.x;
                yOffset = transform.eulerAngles.y;
                zOffset = transform.eulerAngles.z;
            }
        }
    }

}
