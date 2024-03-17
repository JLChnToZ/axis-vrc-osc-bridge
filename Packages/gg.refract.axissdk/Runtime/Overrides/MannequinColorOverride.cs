using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axis.Overrides
{

    [AddComponentMenu("Axis/Overrides/MannequinColorOverride")]
    public class MannequinColorOverride : AxisExecuteOnStartOverride
    {
        public Color color;

        public override void Execute()
        {
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (skinnedMeshRenderer.gameObject.name == "Alpha_Surface")
                {
                    skinnedMeshRenderer.sharedMaterials[0].color = color;
                }
            }
        }
    }
}

