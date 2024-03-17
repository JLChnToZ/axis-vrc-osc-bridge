using System;
using Axis._Editor.Styling;
using Axis.Elements.MirroredNode;
using Axis.Enumerations;
using UnityEditor;
using UnityEngine;

namespace Axis._Editor
{

    [CustomEditor(typeof(MirroredNode))]
    public class MirroredNodeEditor : Editor
    {
        public MirroredNode _target { get { return (MirroredNode)target; }}
        
        MirroredNodeVisibility nodeVisibility;
        


        private void InitStyles()
        {
            EditorUiElementsStyling.InitStyles();
            
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            InitStyles();
            HandleAddingNodeVisibilityComponent();
            HandleBindedToNodeDropdown();
            GUILayout.Space(10);
            HandleRotationOffsetChange();
            HandleToggleMeshVisibility(); 
            HandleMeshCustomization();
        }

        private void HandleSetNodeZero()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle buttonStyle = EditorUiElementsStyling.notActiveButtonStyle;
            string buttonText = "Set Node Zero";
            if (GUILayout.Button(new GUIContent(buttonText, "Reset Node Zero (will lose previous calibration)"), buttonStyle, GUILayout.Width(200f)))
            {
                Debug.Log("Will reset node");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }

        private void HandleAddingNodeVisibilityComponent()
        {
            
            nodeVisibility = _target.GetComponent<MirroredNodeVisibility>() == null ?
                            _target.gameObject.AddComponent<MirroredNodeVisibility>() :
                            _target.GetComponent<MirroredNodeVisibility>();
            nodeVisibility.hideFlags = HideFlags.HideInInspector;
            nodeVisibility.FetchRenderers();
            _target.nodeVisibility = nodeVisibility;
        }

        private void HandleBindedToNodeDropdown()
        {
            EditorGUI.BeginChangeCheck();
            
            var nodeBinding = (NodeBinding)EditorGUILayout.EnumPopup("Binded to Node on:", _target.BindedToNode);
            if (EditorGUI.EndChangeCheck())
            {
                _target.transform.name = $"{_target.BindedToNode} Mirrored Node";
                _target.BindedToNode = nodeBinding;
            }
        
        }
        
        private void HandleToggleMeshVisibility()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle buttonStyle = nodeVisibility.isVisible == true ? EditorUiElementsStyling.activeButtonStyle : EditorUiElementsStyling.notActiveButtonStyle;
            string buttonText = nodeVisibility.isVisible == true ? "Hide Mesh" : "Show Mesh";
            if (GUILayout.Button(new GUIContent(buttonText, "Show/Hide the node mesh for better visualization"), buttonStyle, GUILayout.Width(200f)))
            {
                nodeVisibility.ToggleVisibility();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        
        }
        

        
        private void HandleMeshCustomization()
        {
           
            if (nodeVisibility.isVisible)
            {
                HandleScaleChange();
                HandlePositionOffsetChange();
                
            }
        }

        private void HandleRotationOffsetChange()
        {
            GUILayout.Label(new GUIContent("Node Rotation Offset", "Use this for correction of rotation offsets"), EditorUiElementsStyling.centeredTitle);

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            SerializedProperty rotationOffsetProperty = serializedObject.FindProperty("rotationOffset");
            
            float xOffset = DrawRotationSlider(0, rotationOffsetProperty.vector3Value.x, -180f, 180f);
            float yOffset = DrawRotationSlider(1, rotationOffsetProperty.vector3Value.y, -180f, 180f);
            float zOffset = DrawRotationSlider(2, rotationOffsetProperty.vector3Value.z, -180f, 180f);
            //var xOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);
            //var yOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);
            //var zOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);

            if (EditorGUI.EndChangeCheck())
            {
                rotationOffsetProperty.vector3Value = new Vector3(xOffset, yOffset, zOffset);
                serializedObject.ApplyModifiedProperties();
                //nodeVisibility.UpdateLocalPositionOffset(new Vector3(xOffset, yOffset, zOffset));
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            //HandleResetRotationOffsets();
            GUILayout.Space(10);
        }

        

        private void HandleScaleChange()
        {
        
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent($"Node Mesh Visualization Scale", "For better visualization, you can change the mesh scale. No modification besides visualization"), EditorUiElementsStyling.centeredTitle);
            
            EditorGUI.BeginChangeCheck();
        
            GUILayout.FlexibleSpace();//
            float scale = DrawSimpleSlider("Scale:      ", nodeVisibility.currentScale, 0f, 5f, 1f);
            GUILayout.FlexibleSpace();
            if(EditorGUI.EndChangeCheck()) 
            {
                nodeVisibility.SetScale(scale);
            }
        
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }
        
        private void HandlePositionOffsetChange()
        {
            GUILayout.Label(new GUIContent("Mesh Position Offset", "For better visualization, you can change the mesh position offset. No modification besides visualization"), EditorUiElementsStyling.centeredTitle);
        
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
        
            float xOffset = DrawSimpleSlider("X Offset: ", nodeVisibility.localPositionOffset.x, -1f, 1f);
            float yOffset = DrawSimpleSlider("Y Offset: ", nodeVisibility.localPositionOffset.y, -1f, 1f);
            float zOffset = DrawSimpleSlider("Z Offset: ", nodeVisibility.localPositionOffset.z, -1f, 1f);
            //var xOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);
            //var yOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);
            //var zOffset = GUILayout.HorizontalSlider(0f, -1f, 1f);
        
            if (EditorGUI.EndChangeCheck())
            {
                nodeVisibility.UpdateLocalPositionOffset(new Vector3(xOffset, yOffset, zOffset));
            }
        
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            
        }

        private float DrawSimpleSlider(string label, float value, float minValue, float maxValue, float resetToValue = 0f, bool closeVertical = true)
        {
            GUILayout.BeginVertical();
            var positionOffset = GUILayout.HorizontalSlider(value, minValue, maxValue);
            GUILayout.Space(10f);
            GUILayout.FlexibleSpace();

            if(string.IsNullOrEmpty(label) == false)
            {
                if (GUILayout.Button(new GUIContent(label + positionOffset.ToString("F1"), "Press to Reset"), EditorUiElementsStyling.GetBtnAsLabelStyle()))
                {
                    positionOffset = resetToValue;
                }
            }
            
            if(closeVertical == true)
            {
                GUILayout.EndVertical();
            }
            
            return positionOffset;
        }
        private float DrawRotationSlider(int vectorIndex, float value, float minValue, float maxValue)
        {
            string label = GetLabelFromVectorIndex(vectorIndex);

            var positionOffset = DrawSimpleSlider("", value, minValue, maxValue, 0f, false);
            //GUILayout.BeginVertical();
            //var positionOffset = GUILayout.HorizontalSlider(value, minValue, maxValue);
            //GUILayout.Space(10f);
            //GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(new GUIContent(label + positionOffset.ToString("F1"), "Press to Reset"), EditorUiElementsStyling.GetBtnAsLabelStyle()))
            {               
                positionOffset = 0f;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            //SerializedProperty invertAxisProperty = serializedObject.FindProperty("invertAxis");
            //var element = invertAxisProperty.GetArrayElementAtIndex(vectorIndex);
            //var isFliped = element.boolValue;
            //if (GUILayout.Button(new GUIContent("Flip", "Click to Flip Rotation"), isFliped == true ? EditorUiElementsStyling.activeButtonStyle : EditorUiElementsStyling.notActiveButtonStyle))
            //{
            //    
            //    
            //    
            //    
            //    
            //
            //    element.boolValue = !isFliped;
            //
            //    serializedObject.ApplyModifiedProperties();
            //    //Debug.Log("Should flip");
            //}

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();       
            return positionOffset;
        }

        private string GetLabelFromVectorIndex(int vectorIndex)
        {
            if (vectorIndex == 0) return "X Angle: ";
            if (vectorIndex == 1) return "Y Angle: ";
            if (vectorIndex == 2) return "Z Angle: ";

            throw new Exception($"Unexpected Vector Index {vectorIndex}");
             
        }
    }

}
