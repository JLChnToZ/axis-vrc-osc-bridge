using UnityEngine;
using Axis.Communication;



namespace Axis.Elements
{
    public class AxisRequiringElements : MonoBehaviour
    {
        [HideInInspector] public GameObject runtimeUdpSocketPrefab;

        protected virtual void OnEnable()
        {
            InstantiateAxisRuntimeData();
        }

        private void InstantiateAxisRuntimeData()
        {
            if (FindObjectOfType<AxisRuntimeUdpSocket>() != null) return;
            
            var udpSocket = Instantiate(runtimeUdpSocketPrefab);
            udpSocket.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}

