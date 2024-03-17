
using System;
using System.Collections.Generic;
using Axis.Bindings;
using Axis.DataProcessing;
using Axis.Elements;
using Axis.Enumerations;
using Axis.Events;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Axis._Editor
{
    public static class NodeBindingEditorUtils
    {
        public static int[] GetIntArrayFromSerializedProperty(SerializedProperty serializedProperty)
        {
            int[] array = new int[serializedProperty.arraySize];
            for (int i = 0; i < serializedProperty.arraySize; i++)
            {
                array[i] = serializedProperty.GetArrayElementAtIndex(i).intValue;
            }

            return array;
        }

        public static bool IsNodeBindingRepeated(int[] storedValues, NodeBinding newValue)
        {
            for (int i = 0; i < storedValues.Length; i++)
            {
                if (storedValues[i] == (int)newValue)
                {
                    EditorUtility.DisplayDialog("Error", "Can't assign two nodes to the same binding", "Ok");

                    return true;
                }
            }

            Debug.Log("Not repeated");
            return false;
        }

    }



    [CustomEditor(typeof(AxisBrainNodeBindings))]
    public class AxisBrainNodeBindingEditor : Editor
    {
        
        private ReorderableList nodeBindingsList;
        private AxisBrainNodeBindings brainNodeBindings;
        bool stylesInitialized = false;
        GUIStyle propertyLabelStyle;
        private int indexSelected = -1;

        #region Initialization
        private void InitializeStyles()
        {
            stylesInitialized = true;

            propertyLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(),
                padding = new RectOffset(),
                fontSize = 15,
                fontStyle = FontStyle.Bold,


            };

            propertyLabelStyle.normal.textColor = Color.cyan;
        }

        private void OnEnable()
        {
            indexSelected = -1;          
            brainNodeBindings = ((AxisBrainNodeBindings)target);
            ConfigureNodesBindingsList();
        }

        private void ConfigureNodesBindingsList()
        {
            var configurationSerializedObject = new SerializedObject(brainNodeBindings);
            var nodeBindingProperty = serializedObject.FindProperty("nodeBindings");

            nodeBindingsList = new ReorderableList(configurationSerializedObject,
                            nodeBindingProperty,
                            false, true, false, false);

            nodeBindingsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Node Bindings", propertyLabelStyle);
                DrawResetNodeBindingsButton(rect);
            };

            nodeBindingsList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                DrawNodeBindingElement(rect, index);
            };

        }




        #endregion

        public override void OnInspectorGUI()
        {
            
            if (!stylesInitialized)
            {
                InitializeStyles();
            }

            serializedObject.Update();
            //outputCharactersList.DoLayoutList();
            nodeBindingsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            

            var assetObject = new SerializedObject(brainNodeBindings);
            var nodeBindingProperty = serializedObject.FindProperty("nodeBindings");

            if (nodeBindingProperty.isArray)
            {
                
                int arrayLength = 0;

                nodeBindingProperty.Next(true); // skip generic field
                nodeBindingProperty.Next(true); // advance to array size field

                // Get the array size
                arrayLength = nodeBindingProperty.intValue;
                nodeBindingProperty.Next(true); // advance to first array index

                List<NodeBinding> nodeBindings = new List<NodeBinding>(arrayLength);
                int lastIndex = arrayLength - 1;

                for (int i = 0; i < arrayLength; i++)
                {
                    nodeBindings.Add((NodeBinding)nodeBindingProperty.intValue); // copy the value to the list
                    if (i < lastIndex) nodeBindingProperty.Next(false); // advance without drilling into children
                }

            }

            if (indexSelected != -1)
            {
                HandleShowConfigButtonPressed();
            }
            serializedObject.ApplyModifiedProperties();
        }
        #region Drawing

        private void DrawNodeBindingElement(Rect rect, int index)
        {
            var element = nodeBindingsList.serializedProperty.GetArrayElementAtIndex(index);


            EditorGUI.LabelField(
                new Rect(rect.x, rect.y, 250, EditorGUIUtility.singleLineHeight),
                $"Node {index} | default {(NodeBinding)index} binded to");

            EditorGUI.BeginChangeCheck();
            var currentValue = (NodeBinding)element.intValue;
            var nodeBindingsStoredValues = NodeBindingEditorUtils.GetIntArrayFromSerializedProperty(nodeBindingsList.serializedProperty);
            EditorGUI.PropertyField(
                new Rect(rect.x + 220, rect.y, 150, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                //Debug.Log($"Element of index {index} value was {currentValue} is {(NodeBinding)element.intValue}");

                //Debug.Log("Changed");
                //var newValue = (NodeBinding)element.intValue;
                //element.intValue = NodeBindingEditorUtils.IsNodeBindingRepeated(nodeBindingsStoredValues, newValue) ? (int)currentValue : (int)newValue;
                brainNodeBindings.nodeBindings[index] = (NodeBinding)element.intValue;
                Debug.Log("Changed");

                Debug.Log(brainNodeBindings.nodeBindings[index]);
            }
            
            //serializedObject.ApplyModifiedProperties();

            DrawCalibrateButton(rect, index);
        }

        private static void DrawCalibrateButton(Rect rect, int index)
        {
            //GUI.enabled = Application.isPlaying;
            if (GUI.Button(new Rect(rect.x + 380, rect.y, 80, EditorGUIUtility.singleLineHeight),
                new GUIContent("Calibrate", "Reset Node Calibration")))
            {
                Debug.Log($"Will calibrate {index} {(NodeBinding)index}");
                //AxisEvents.OnZero(index);
                AxisEvents.OnCalibration();
            }
            //GUI.enabled = true;
        }

        private void DrawResetNodeBindingsButton(Rect rect)
        {
            if (GUI.Button(new Rect(rect.x + 150, rect.y, 80,
            EditorGUIUtility.singleLineHeight),
            "Reset"))
            {
                ResetNodeBindingsToDefault();
                serializedObject.ApplyModifiedProperties();
            }
            
        }

        private void HandleShowConfigButtonPressed()
        {
            var outputCharactersProperty = serializedObject.FindProperty("OutputCharacters");
            if (outputCharactersProperty.isArray)
            {

                int arrayLength = 0;

                outputCharactersProperty.Next(true); // skip generic field
                outputCharactersProperty.Next(true); // advance to array size field

                // Get the array size
                arrayLength = outputCharactersProperty.intValue;

                outputCharactersProperty.Next(true); // advance to first array index

                // Write values to list
                List<Animator> outputCharacters = new List<Animator>(arrayLength);
                int lastIndex = arrayLength - 1;

                for (int i = 0; i < arrayLength; i++)
                {
                    outputCharacters.Add((Animator)outputCharactersProperty.objectReferenceValue); // copy the value to the list
                    if (i < lastIndex) outputCharactersProperty.Next(false); // advance without drilling into children
                }

                if (outputCharacters[indexSelected] != null)
                {
                    EditorGUILayout.LabelField(outputCharacters[indexSelected].ToString());
                }


            }
        }

        #endregion

        #region Utils

        

        

        private void ResetNodeBindingsToDefault()
        {
            Debug.Log("Reset");
            var defaultNodeLimbs = AxisDataUtility.GetDefaultNodeArrangement();

            for (int i = 0; i < defaultNodeLimbs.Count; i++)
            {
                nodeBindingsList.serializedProperty.GetArrayElementAtIndex(i).intValue = (int)defaultNodeLimbs[i];
            }
            brainNodeBindings.nodeBindings = AxisDataUtility.GetDefaultNodeArrangement();
        }
        #endregion















    }
}


