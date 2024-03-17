using System;
using Axis._Editor.Styling;
using Axis.Communication;
using Axis.Elements;
using Axis.Enumerations;
using UnityEditor;
using UnityEngine;

namespace Axis._Editor
{
    [CustomEditor(typeof(AxisBrain))]
    public class AxisBrainEditor: Editor
    {
        private bool isPrefabAsset = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorUiElementsStyling.InitStyles();

            if((PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Regular || PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.Variant) && PrefabUtility.GetPrefabInstanceStatus(target) == PrefabInstanceStatus.NotAPrefab)
            {
                isPrefabAsset = true;
            } 

            DrawConnectToAxisButton(isPrefabAsset);

            HandleToggleMannequinVisibility();
            HandleToggleNodesRepresentationVisibility();

            
            //Repaint();
            //FindObjectOfType<AxisRuntimeUdpSocketEditor>().DrawConnectToAxisButton();
            //AxisRuntimeUdpSocketEditor.DrawSomething();
        }

        private void HandleToggleNodesRepresentationVisibility()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            AxisBrain _target = (AxisBrain)target;
            //bool isNodesRepresentationVisible = _target.isNodesRepresentationVisible;
            SerializedProperty isNodesRepresentationVisibileProperty = serializedObject.FindProperty("isNodesRepresentationVisible");

            GUIStyle buttonStyle = isNodesRepresentationVisibileProperty.boolValue == true ? EditorUiElementsStyling.activeButtonStyle : EditorUiElementsStyling.notActiveButtonStyle;
            string buttonText = isNodesRepresentationVisibileProperty.boolValue == true ? "Hide Nodes" : "Show Nodes";
            string tooltipText = isPrefabAsset == false ? "Show/Hide the mannequin representation" : "Only prefab instance can Show/Hide";
            GUI.enabled = !isPrefabAsset;
            if (GUILayout.Button(new GUIContent(buttonText, tooltipText), buttonStyle, GUILayout.Width(200f)))
            {
                isNodesRepresentationVisibileProperty.boolValue = !isNodesRepresentationVisibileProperty.boolValue;
                _target.axisNodesRepresentation.SetVisibility(isNodesRepresentationVisibileProperty.boolValue);
            }
            GUI.enabled = true;
            serializedObject.ApplyModifiedProperties();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

        }

        private void HandleToggleMannequinVisibility()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            AxisBrain _target = (AxisBrain)target;

            SerializedProperty isMannequinVisibibleProperty = serializedObject.FindProperty("isMannequinVisible");

            bool isMannequinVisible = isMannequinVisibibleProperty.boolValue;
            GUIStyle buttonStyle = isMannequinVisible == true ? EditorUiElementsStyling.activeButtonStyle : EditorUiElementsStyling.notActiveButtonStyle;

            string buttonText = isMannequinVisible == true ? "Hide Mannequin" : "Show Mannequin";

            GUI.enabled = !isPrefabAsset;
            string tooltipText = isPrefabAsset == false ? "Show/Hide the mannequin representation" : "Only prefab instance can Show/Hide";
            if (GUILayout.Button(new GUIContent(buttonText, tooltipText), buttonStyle, GUILayout.Width(200f)))
            {
                isMannequinVisibibleProperty.boolValue = !isMannequinVisibibleProperty.boolValue;
                //_target.isMannequinVisible = !_target.isMannequinVisible;
                _target.axisMannequin.SetVisibility(isMannequinVisibibleProperty.boolValue);
                
                serializedObject.ApplyModifiedProperties();
                
            }

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
        }

        public static void DrawConnectToAxisButton(bool isPrefabAsset)
        {
            //SerializedProperty connectToAxisProperty = AxisRuntimeUdpSocket.shouldConnectToAxis;
            //SerializedProperty isConnectedToAxisProperty = serializedObject.FindProperty("isConnectedToAxis");

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            string buttonText = AxisRuntimeUdpSocket.IsConnectedToAxis == false ? "Connect" : "Disconnect";
            string tooltipText = isPrefabAsset == false ? "Connect/Disconnect to Axis on Edit Mode, it will auto connect when on play mode" : "Only prefab instance can connect to Axis";
            GUILayout.FlexibleSpace();
            GUI.enabled = Application.isPlaying == false && isPrefabAsset == false;

            

            if (GUILayout.Button(new GUIContent(buttonText, tooltipText),
                EditorUiElementsStyling.GetButtonStyleFromConnectionStatus(AxisRuntimeUdpSocket.IsConnectedToAxis),
                GUILayout.MaxWidth(EditorUiElementsStyling.connectButtonWidth)))
                {
                                  
                    AxisRuntimeUdpSocket.ShouldConnectToAxis = !AxisRuntimeUdpSocket.ShouldConnectToAxis;             
                }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

        }
    }
}

