using Axis.Elements.AnimatorLink;
using UnityEngine;

namespace Axis.Solvers
{
    public abstract class TorsoRotationSolver : MonoBehaviour
    {
        public abstract void SolveTorsoRotation(BodyModelAnimatorLink bodyModel, Transform hips);
    }

}
