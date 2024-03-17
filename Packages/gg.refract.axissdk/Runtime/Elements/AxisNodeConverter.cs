using Axis.Enumerations;
using UnityEngine;


namespace Axis.Elements
{
    public static class AxisNodeConverter
    {
        public static void ConvertNode(Transform nodeMesh, Transform led, NodeBinding nodeBinding, Vector3 rotationOffset)
        {
            //Debug.Log($"Must Convert {nodeMesh.transform.name} {led.transform.name}");

            switch (nodeBinding)
            {
                
                case NodeBinding.LeftThigh:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0f, 0.02f, 0.002f);
                    break;

                case NodeBinding.LeftCalf:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0f, 0.02f, 0.002f);
                    break;

                case NodeBinding.RightThigh:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0f, 0.02f, 0.002f);
                    break;

                case NodeBinding.RightCalf:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0f, 0.02f, 0.002f);
                    break;

                case NodeBinding.RightUpperArm:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0.02f, 0f, -0.002f);
                    break;

                case NodeBinding.RightForeArm:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0.02f, 0f, -0.002f);
                    break;

                case NodeBinding.LeftUpperArm:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0.02f, 0f, 0.002f);
                    break;

                case NodeBinding.LeftForeArm:
                    nodeMesh.localScale = new Vector3(0.1f, 0.1f, 0.01f);
                    led.localPosition = new Vector3(0.02f, 0f, 0.002f);
                    break;

                case NodeBinding.Chest:
                    nodeMesh.localScale = new Vector3(0.1f, 0.01f, 0.1f);
                    led.localPosition = new Vector3(0.02f, 0.002f, 0f);
                    break;

                default:
                    break;

                
            }
        }
    }

}
