using Axis.Enumerations;
using UnityEngine;

namespace Axis.Elements
{
    public abstract class NodeProcessor: MonoBehaviour
    {

        [HideInInspector] public string brainUniqueId;
        public abstract void Initialize(string uniqueId);
    }

}



