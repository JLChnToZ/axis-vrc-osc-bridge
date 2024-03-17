using System.Collections;
using System.Collections.Generic;
using Axis._Editor;
using Axis.Bindings;
using Axis.Elements;
using Axis.Elements.AnimatorLink;
using Axis.Elements.Linker;
using Axis.Enumerations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CharacterToBrainLinker))]
public class CharacterToBrainLinkerEditor : Editor
{
    GUIStyle propertyLabelStyle;
    bool stylesInitialized = false;
    private ReorderableList nodeBindingsList;
    private bool isNodeBindingListConfigured = false;
    #region Initialization

    private void OnEnable()
    {
        isNodeBindingListConfigured = false;
        //ConfigureNodesBindingsList();

    }

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

    

    #endregion

    #region Drawing
    private void DrawNodeBindingElement(Rect rect, int index)
    {
        var element = nodeBindingsList.serializedProperty.GetArrayElementAtIndex(index);


        EditorGUI.LabelField(
            new Rect(rect.x, rect.y, 250, EditorGUIUtility.singleLineHeight),
            $"Mannequin Node {(NodeBinding)element.intValue} binded to ");

        EditorGUI.BeginChangeCheck();
        var currentValue = (NodeBinding)element.intValue;
        var nodeBindingsStoredValues = NodeBindingEditorUtils.GetIntArrayFromSerializedProperty(nodeBindingsList.serializedProperty);

        GenericCharacterAnimatorLink genericCharacterAnimatorLink = _target.characterAnimatorLink as GenericCharacterAnimatorLink;

        Transform transform = genericCharacterAnimatorLink.characterBindedTransforms.bindedTransforms[index];
        //var t = EditorGUI.ObjectField("root", root, typeof(Transform), true) as Transform;

        //root = (Transform)EditorGUILayout.ObjectField(root, typeof(Transform), true);

        transform = (Transform)EditorGUI.ObjectField(new Rect(rect.x + 220, rect.y, 150, EditorGUIUtility.singleLineHeight), transform, typeof(Transform), true);
        //EditorGUI.PropertyField(
        //    new Rect(rect.x + 220, rect.y, 150, EditorGUIUtility.singleLineHeight),
        //    element, GUIContent.none);
        
        

        if (EditorGUI.EndChangeCheck())
        {
            if(transform.IsChildOf(_target.transform) == true)
            {
                genericCharacterAnimatorLink.characterBindedTransforms.bindedTransforms[index] = transform;
            } else
            {
                EditorUtility.DisplayDialog("Error", "Transform must be a child of the Character", "Ok");
            }
                                   
        }

        serializedObject.ApplyModifiedProperties();

        //DrawVibrateButton(rect, index);
    }

    #endregion

    public CharacterToBrainLinker _target { get { return (CharacterToBrainLinker)target; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!stylesInitialized)
        {
            InitializeStyles();
        }

        serializedObject.Update();
        
        if (_target.GetComponent<Animator>().isHuman)
        {
            GUILayout.Label("Humanoid Character To Link");
        
        } else
        {
            if(_target.GetComponent<GenericCharacterAnimatorLink>() == null)
            {
                
                if (GUILayout.Button("Add Generic Character Linker", GUILayout.Width(200)))
                {
                    _target.characterAnimatorLink = _target.gameObject.AddComponent<GenericCharacterAnimatorLink>();
                    
                    Debug.Log("Will add binding!");
                }
            }
            else
            {
                GUILayout.Label("Generic Character To Link");
                HandleGenericAnimatorLink();
                
            }
                        
        
            //GUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void HandleGenericAnimatorLink()
    {
        AxisBrain linkedBrain = _target.linkedBrain;
        //CharacterToBrainLinker characterToBrainLinker = _target.GetComponent<CharacterToBrainLinker>();
        
        if(linkedBrain == null)
        {
            //if(_target.)
            GUILayout.Label("Must reference a brain");
        } else
        {
            GenericCharacterAnimatorLink characterAnimatorLink = (GenericCharacterAnimatorLink)_target.characterAnimatorLink;

            serializedCharacterAnimatorLink = new SerializedObject((GenericCharacterAnimatorLink)_target.characterAnimatorLink);

            if (characterAnimatorLink == null)
            {
                characterAnimatorLink = _target.gameObject.AddComponent<GenericCharacterAnimatorLink>();
            }
            
            if (characterAnimatorLink.BrainNodeBindings == null)
            {
                

                characterAnimatorLink.BrainNodeBindings = linkedBrain.gameObject.GetComponent<AxisBrainNodeBindings>();
                
            } else
            {
                if(isNodeBindingListConfigured == false)
                {
                    ConfigureNodesBindingsList(characterAnimatorLink);
                    isNodeBindingListConfigured = true;
                }

                //Debug.Log(nodeBindingsList.count);
                nodeBindingsList.DoLayoutList();
                serializedCharacterAnimatorLink.ApplyModifiedProperties();
                //Debug.Log("-----------------");
                for (int i = 0; i < characterAnimatorLink.BrainNodeBindings.nodeBindings.Count; i++)
                {
                    //Debug.Log($"[{i}] - {characterAnimatorLink.brainNodeBindings.nodeBindings[i]}");
                }



            }
        }      
    }


    SerializedObject serializedCharacterAnimatorLink;
    private void ConfigureNodesBindingsList(GenericCharacterAnimatorLink characterAnimatorLink)
    {
        serializedCharacterAnimatorLink = new SerializedObject(characterAnimatorLink);
        var brainNodeBindings = characterAnimatorLink.BrainNodeBindings;
        var brainNodeBindingsSerializedObject = new SerializedObject(brainNodeBindings);

        //var nodeBindingProperty = serializedCharacterAnimatorLink.FindProperty("brainNodeBindings");

        //Debug.Log($"Name = {nodeBindingProperty.name} ");

        var nodeBindingListProperty = brainNodeBindingsSerializedObject.FindProperty("nodeBindings");

        //Debug.Log($"Name = {nodeBindingListProperty.name} ");
        if (nodeBindingListProperty.isArray == true)
        {
            //Debug.Log("Is array!");
        }
        //Debug.Log($"Type {nodeBindingListProperty.GetType()}");

        


        nodeBindingsList = new ReorderableList(serializedCharacterAnimatorLink,
                        nodeBindingListProperty,
                        false, true, false, false);
        //Debug.Log($"Type {nodeBindingsList.count}");
        nodeBindingsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Mannequin Nodes -> Generic Character Transforms", propertyLabelStyle);
            //DrawResetNodeBindingsButton(rect);
        };

        nodeBindingsList.drawElementCallback =
        (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            DrawNodeBindingElement(rect, index);
        };

    }
}
