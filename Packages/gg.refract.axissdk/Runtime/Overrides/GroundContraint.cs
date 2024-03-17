using System;
using UnityEditor;
using UnityEngine;

namespace Axis.Constrains
{
    [AddComponentMenu("Axis/Constrains/Ground Constraint")]
    public class GroundContraint : MonoBehaviour
    {

        private const float timeToResetVerticalZero = 0.5f;
        private Transform[] transformsToContraint;
        [Tooltip("Drag here the transform of the ground")]public Collider contrainReference;

        public bool generateFeetOffsetAutomatically;
        public Action<float> OnSetVerticalZeroPosition;
        float timeAboveGround = -1f;

        private const float groundedThreshold = 0.001f;

        [Range(-1, 1)] public float VerticalDistanceFromFeatSurfaceToJoint;

        #region Initialization
        private void Start()
        {
            PopulateTransformsToContraint();
        }

        private void PopulateTransformsToContraint()
        {
            Animator animator = GetComponent<Animator>();
            transformsToContraint = new Transform[2];
            transformsToContraint[0] = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            transformsToContraint[1] = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }

        #endregion

        #region Utils

        private Transform GetLowestTransform()
        {
            Transform lowestTransform = null;
            float lowestTransformHeight = Mathf.Infinity;
            foreach (Transform t in transformsToContraint)
            {
                if (t.position.y < lowestTransformHeight)
                {
                    lowestTransformHeight = t.position.y;
                    lowestTransform = t;
                }
            }
            return lowestTransform;
        }
        private float GetMeshLowestPoint()
        {
            SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            Vector3 lowestVertex = Vector3.positiveInfinity;
            foreach (var skinnedMesh in skinnedMeshRenderers)
            {
                Vector3[] vertices = skinnedMesh.sharedMesh.vertices;
                foreach (Vector3 vertex in vertices)
                {
                    if (vertex.y < lowestVertex.y)
                    {
                        lowestVertex = vertex;
                    }
                }
            }

            Matrix4x4 localToWorld = transform.localToWorldMatrix;
            lowestVertex = localToWorld.MultiplyPoint3x4(lowestVertex);

            return lowestVertex.y;


        }
        private void CalculateVerticalDifferenceFromFeetSurfaceToJoint()
        {
            float meshLowestPoint = GetMeshLowestPoint();
            Transform footTransform = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightFoot);
            VerticalDistanceFromFeatSurfaceToJoint = footTransform.position.y - meshLowestPoint;
        }

        private static bool isVerticalDifferencePositive(float verticalDiference)
        {
            return verticalDiference > 0;
        }

        private bool isVerticalDifferenceNegligeble(float verticalDiference)
        {
            return Mathf.Abs(verticalDiference) < groundedThreshold;
        }

        private bool IsTimeAboveGroundExcessive()
        {
            return Time.time - timeAboveGround > timeToResetVerticalZero;
        }

        private bool IsTimeAboveGroundReseted()
        {
            return timeAboveGround == -1;
        }

        private void ResetTimeAboveGround()
        {
            timeAboveGround = -1f;
        }
        #endregion



        public void Contraint()
        {
            if (enabled == true)
            {
                Transform lowestTransform = GetLowestTransform();
                if (lowestTransform != null)
                {                                      
                    //if there is a contrain referece, should check distance from lowest transform
                    if (contrainReference != null)
                    {
                        //Cast a ray from high altitude to get ground top surface
                        Vector3 lowestTransfromOnHighAltitude = new Vector3(lowestTransform.position.x, 100000, lowestTransform.position.z);                                          
                        Vector3 hitPosition = GetHitPositionOnGroundFromLowestTransform(lowestTransfromOnHighAltitude);

                        if (hitPosition.x == Mathf.Infinity)
                        {
                            //Not above ground

                        }
                        else
                        {
                            
                            Vector3 consideredFootPosition = lowestTransform.position - new Vector3(0, VerticalDistanceFromFeatSurfaceToJoint, 0f);
                            float verticalDiference = consideredFootPosition.y - hitPosition.y;

                            if (isVerticalDifferenceNegligeble(verticalDiference))
                            {
                                ResetTimeAboveGround();
                            }
                            //Above ground, timed correction for account for jumps
                            else if (isVerticalDifferencePositive(verticalDiference))
                            {
                                if (IsTimeAboveGroundReseted())
                                {
                                    timeAboveGround = Time.time;
                                }
                                else
                                {
                                    HandleLowestTransformAboveGround(verticalDiference);
                                }

                            }
                            //Bellow ground, instantaneous correction
                            else
                            {
                                HandleLowestTransformBellowGround(verticalDiference);
                            }
                        }
                    }
                }
            }



        }

        private void HandleLowestTransformAboveGround(float verticalDiference)
        {
            if (IsTimeAboveGroundExcessive())
            {
                //Set Zero to fix error
                OnSetVerticalZeroPosition?.Invoke(-verticalDiference);
            }
        }

        private void HandleLowestTransformBellowGround(float verticalDiference)
        {
            ResetTimeAboveGround();
            transform.position -= new Vector3(0f, verticalDiference, 0f);
            OnSetVerticalZeroPosition?.Invoke(-verticalDiference);
        }

        private Vector3 GetHitPositionOnGroundFromLowestTransform(Vector3 lowestTransfromOnHighAltitude)
        {
            Ray ray = new Ray(lowestTransfromOnHighAltitude, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            Vector3 hitPosition = Vector3.positiveInfinity;

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == contrainReference)
                {
                    hitPosition = hit.point;
                }
            }

            return hitPosition;
        }



        #region DrawingGizmos

        [Tooltip("Draw a ruler close to the right feet. The green line is the surface considered for floor contraining")] public bool showFeetYOffset = false;
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                GameObject footGameObject = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightFoot).gameObject;
                if (generateFeetOffsetAutomatically == true)
                {
                    CalculateVerticalDifferenceFromFeetSurfaceToJoint();
                }

                if (showFeetYOffset == true)
                {
                    DrawFeetRuler(footGameObject);
                }
            }
        }

        private void DrawFeetRuler(GameObject footGameObject)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(footGameObject.transform.position + new Vector3(0.30f, 0.3f, 0f), footGameObject.transform.position + new Vector3(0.30f, -0.3f, 0f));
            Gizmos.color = Color.red;
            Vector3 footLine = new Vector3(footGameObject.transform.position.x + 0.35f, footGameObject.transform.position.y, footGameObject.transform.position.z);
            Gizmos.DrawRay(footLine, -Vector3.right * 0.1f);
            Gizmos.color = Color.green;
            Vector3 offsetedFootLine = new Vector3(footLine.x, footLine.y - VerticalDistanceFromFeatSurfaceToJoint, footLine.z);
            Gizmos.DrawRay(offsetedFootLine, -Vector3.right * 0.1f);
        }

        #endregion
    }
}

