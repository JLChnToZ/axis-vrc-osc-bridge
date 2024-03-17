using System;
using Axis.Enumerations;
using UnityEditor;
using UnityEngine;


namespace Axis.Elements.MirroredNode
{
    [ExecuteAlways]
    public class MirroredNodeVisibility : MonoBehaviour
    {
        private Transform meshes;
        public bool isVisible = false;
        public Vector3 localPositionOffset = Vector3.zero;
        public float currentScale = 1f;
        public Vector3 defaultScale = Vector3.positiveInfinity;
        public void FetchRenderers()
        {
            foreach (Transform child in transform)
            {
                if (child.parent != transform)
                {
                    break;
                }

                if(child.name == "Meshes")
                {
                    meshes = child;
                }

                

            }
        }

        private void Start()
        {
            if(defaultScale.x == Mathf.Infinity)
            {
                defaultScale = transform.localScale;
            }

            
        }

        public void UpdateLocalPositionOffset(Vector3 _localPositionOffset)
        {
            localPositionOffset = _localPositionOffset;
            meshes.transform.localPosition = localPositionOffset;
            
        }

        public void ToggleVisibility()
        {
            isVisible = !isVisible;
            SetVisibility(isVisible);
        }

        private void SetVisibility(bool isVisible)
        {
            meshes.gameObject.SetActive(isVisible);
        }

        public void SetScale(float scale)
        {
            currentScale = scale;
            meshes.localScale = new Vector3(scale, scale, scale);
            //throw new NotImplementedException();
        }

        internal void UpdateMeshAfterNodeBinding(NodeBinding nodeBinding)
        {
            
            var meshesArray = meshes.GetComponentsInChildren<Transform>();
            AxisNodeConverter.ConvertNode(meshesArray[1], meshesArray[2], nodeBinding, Vector3.zero);
        }
    }

}
