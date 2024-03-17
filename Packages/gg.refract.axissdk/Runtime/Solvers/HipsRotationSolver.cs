using Axis.Elements.AnimatorLink;
using UnityEngine;

namespace Axis.Solvers
{
    public abstract class HipsRotationSolver : MonoBehaviour
    {
        public abstract void SolveHipsRotation(BodyModelAnimatorLink bodyModel, Transform hips);
    }
}

