using UnityEditor;
using UnityEngine;

namespace Axis.Elements.MirroredNode.CustomMenu
{

    public class AddMirroredNodeCustomMenu
    {
        public AddMirroredNodeCustomMenu()
        {
            Debug.Log("I'm a constructor!");
        }

        // Adding a new context menu item
        [MenuItem("GameObject/Axis/Add Node To Control Transform", true)]
        static bool ValidateTransformSelection()
        {
            // disable menu item if no transform is selected.
            return Selection.activeTransform != null;
        }

        [MenuItem("GameObject/Axis/Remove Node To Control Transform", true)]
        static bool ValidateTransformWithNodeControlledSelection()
        {
            return (Selection.activeTransform != null && Selection.activeTransform.GetComponent<MirroredNode>() != null);
        }

        [MenuItem("GameObject/Axis/Remove Node To Control Transform", false, 0)] //10
        private static void RemoveNodeToControlledTransform(MenuCommand menuCommand)
        {
            GameObject mirroredNode = Selection.activeObject as GameObject;

            Transform[] childTransforms = mirroredNode.transform.GetComponentsInChildren<Transform>();

            foreach (var childTransform in childTransforms)
            {
                if (childTransform.parent != mirroredNode.transform)
                {
                    continue;
                }

                if (childTransform.name != "Meshes")
                {
                    GameObjectUtility.SetParentAndAlign(childTransform.gameObject, mirroredNode.transform.parent.gameObject);
                }
            }

            GameObject.DestroyImmediate(mirroredNode);

        }

        [MenuItem("GameObject/Axis/Add Node To Control Transform", false, 0)] //10
        private static void AddNodeToControlledTransform(MenuCommand menuCommand)
        {

            GameObject selected = Selection.activeObject as GameObject;
            AssetLoader assetLoader = FetchAssetLoader();
            InstantiateAndParent(assetLoader.mirroredNodePrefab, selected);

        }

        private static AssetLoader FetchAssetLoader()
        {
            string[] assets = AssetDatabase.FindAssets("t:AssetLoader");
            string path = AssetDatabase.GUIDToAssetPath(assets[0]);
            AssetLoader assetLoader = AssetDatabase.LoadAssetAtPath<AssetLoader>(path);
            return assetLoader;
        }

        private static void AddAxisBrainToScene(MirroredNode mirroredNode)
        {
            AssetLoader assetLoader = FetchAssetLoader();
            InstantiateAndAddAsReference(assetLoader.axisBrainPrefab, mirroredNode);
        }

        private static void InstantiateAndAddAsReference(GameObject axisBrainPrefab, MirroredNode mirroredNode)
        {
            GameObject axisBrain = GameObject.Instantiate(axisBrainPrefab);
            mirroredNode.connectedBrain = axisBrain.GetComponent<AxisBrain>();

        }

       
        private static void InstantiateAndParent(GameObject controlNodePrefab, GameObject selected)
        {

            if (PrefabUtility.IsPartOfAnyPrefab(selected) == true)
            {

                bool shouldUnpack = EditorUtility.DisplayDialog("Prefab Warning", "You are adding a Control Node to a prefab. In order to do so, you need to unpack it first. " +
                    "\n\nIf you want, you can create another prefab later", "Yes, unpack for me", "Cancel");

                if (shouldUnpack == true)
                {
                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(selected);
                    PrefabUtility.UnpackPrefabInstance(selected.transform.root.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                }
                else
                {
                    return;
                }

            }

            GameObject controlNode = GameObject.Instantiate(controlNodePrefab);
            controlNode.name = $"Node Controlling {selected.transform.name}";
            // adjust hierarchy accordingly

            int siblingIndex = selected.transform.GetSiblingIndex();

            GameObjectUtility.SetParentAndAlign(controlNode, selected.transform.parent.gameObject);
            controlNode.transform.position = selected.transform.position;

            controlNode.GetComponent<MirroredNode>().defaultRotation = selected.transform.localRotation;
            GameObjectUtility.SetParentAndAlign(selected, controlNode);

            controlNode.transform.SetSiblingIndex(siblingIndex);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(controlNode, "Parented " + controlNode.name);

            AttachOnSceneAxisBrain(controlNode.GetComponent<MirroredNode>());
            Selection.activeObject = controlNode;
        }

        private static void AttachOnSceneAxisBrain(MirroredNode mirroredNode)
        {
            AxisBrain[] axisBrainOnScene = GameObject.FindObjectsOfType<AxisBrain>();

            if (axisBrainOnScene.Length == 1)
            {
                mirroredNode.connectedBrain = axisBrainOnScene[0];
            }
            else
            {
                if (axisBrainOnScene.Length == 0)
                {
                    bool shouldAddAxisBrain = EditorUtility.DisplayDialog("No AxisBrain Found!", "No Axis Brain was found on the scene. We can add one and link to the Node, or you could add it later", "Yes, add and reference for me.", "No, I'll add and reference one later.");

                    if (shouldAddAxisBrain == true)
                    {
                        AddAxisBrainToScene(mirroredNode);
                    }

                }
                else
                {
                    EditorUtility.DisplayDialog("Multiple Axis Brain on the Scene", "Multiple Axis Brain were found on the scene, can't assign the Axis Brain to the Controlling Node automatically. \n\nPlease assign mannualy.", "Ok");
                }
            }

        }


    }

}
