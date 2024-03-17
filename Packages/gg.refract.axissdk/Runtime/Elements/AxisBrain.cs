using Axis.Enumerations;
using Axis.Elements.FreeNodes;
using Axis.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.DataTypes;
using Axis.Utils;
using Axis.Bindings;

namespace Axis.Elements
{

 

    //Axis Brain is the center of the Axis System. It holds references to:
    //Axis Mannequin - A reference humanoid that replicates the joints angles of the person wearing Axis
    //NodeObjectsProcessor - If you are using Axis Nodes by holding it to your hand or attaching to an object
    //Output Characters - Any character that you want to move as the Axis Mannequin + Absolute Tracking

    [RequireComponent(typeof(AxisBrainNodeBindings)), ExecuteAlways, Serializable]
    public class AxisBrain : AxisRequiringElements
    {

        #region Class Variables
        [HideInInspector] public string uniqueID = null;
        public AxisMannequin axisMannequin;
        [HideInInspector] public AxisNodesRepresentation axisNodesRepresentation;
        private NodeObjectsProcessor nodeObjectsHandler;
        private ActiveNodesDetector activeNodesDetector = new ActiveNodesDetector();
        private AxisBrainNodeBindings nodeBindings;
        public HipProvider hipProvider;
        
        [SerializeField, HideInInspector] public bool isMannequinVisible;
        [SerializeField, HideInInspector] public bool isNodesRepresentationVisible = false;
        #endregion

        #region Static Utils

        public static AxisBrain FetchBrainOnScene()
        {
            AxisBrain[] axisBrainOnScene = GameObject.FindObjectsOfType<AxisBrain>();

            if (axisBrainOnScene.Length == 1)
            {
                return axisBrainOnScene[0];
            }
            else
            {
                string warningMessage = axisBrainOnScene.Length == 0 ?
                    "Automatic brain linking failed. No axis brain found on scene" :
                    "Automatic brain linking failed. More then one Axis Brain on the scene. Please, assign mannually";

                Debug.LogWarning(warningMessage);
            }

            return null;
        }

        #endregion

        #region Initialization

        protected override void OnEnable()
        {
            base.OnEnable();

            uniqueID = string.IsNullOrEmpty(uniqueID) == true ? Guid.NewGuid().ToString() : uniqueID;
            
            GetReferences();
            RegisterCallbacks();
            ResetPosition();
            SetupAxisNodesRepresentation();
            SetupAxisMannequin();
            SetupFreeNodesHandler();
            SetVisibility();
        }


        
        private void SetupAxisNodesRepresentation()
        {
            axisNodesRepresentation.Initialize(uniqueID);
        }
        private void GetReferences()
        {
            nodeBindings = GetComponent<AxisBrainNodeBindings>();
        }

        public void SetVisibility()
        {
            axisNodesRepresentation.SetVisibility(isNodesRepresentationVisible);
            axisMannequin.SetVisibility(isMannequinVisible);
        }

        #region InitializationUtils
        private void ResetPosition()
        {
            transform.position = Vector3.zero;
        }



        #endregion

        #region ObjectsSetup
        private void SetupFreeNodesHandler()
        {
            nodeObjectsHandler = GetComponentInChildren<NodeObjectsProcessor>();
            nodeObjectsHandler.transform.parent = transform;
            nodeObjectsHandler.transform.parent = transform;
            nodeObjectsHandler.Initialize(uniqueID);
        }

        private void SetupAxisMannequin()
        {
            

            //axisMannequin.SetupOutputCharacters(OutputCharacters);
            axisMannequin.Initialize(uniqueID);
        }

        #endregion




        #endregion

        #region Callbacks

        private void RegisterCallbacks()
        {
            AxisEvents.OnAxisOutputDataUpdated += HandleOnAxisOutputDataUpdated;
        }

        private void OnDisable()
        {
            AxisEvents.OnAxisOutputDataUpdated -= HandleOnAxisOutputDataUpdated;
        }

        //This function is called whenever the Axis System updates its data, then it updates the Axis Mannequin
        //and Node Objects Handler
        private void HandleOnAxisOutputDataUpdated(AxisOutputData axisOutputData)
        {
            
            Dictionary<NodeBinding, AxisNodeData> mannequinNodesData;
            Dictionary<NodeBinding, AxisNodeData> nodeObjectsData;
           // activeNodesDetector.UpdateActiveNodes(axisOutputData);

            //Debug.Log(axisOutputData.nodesDataList[(int)NodeBinding.LeftThigh].rotation);

            mannequinNodesData = NodesDataSupplier.GetMannequinNodesData(axisOutputData, nodeBindings.nodeBindings);
            nodeObjectsData = NodesDataSupplier.GetNodeObjecstData(axisOutputData, nodeBindings.nodeBindings);
            if (mannequinNodesData != null && mannequinNodesData.Count > 0)
            {
                axisMannequin.UpdateHubData(axisOutputData.hubData, axisNodesRepresentation.nodesByLimb);              
                axisMannequin.UpdateNodesData(mannequinNodesData, axisNodesRepresentation.nodesByLimb);
                //axisMannequin.HandleNodeDataUpdated(axisNodesRepresentation.nodesByLimb);
                axisMannequin.HandleAxisDataUpdated(axisNodesRepresentation.nodesByLimb, hipProvider, axisOutputData.hubData);
                axisMannequin.UpdateTorso(axisNodesRepresentation.nodesByLimb);
                axisMannequin.UpdateHips(axisNodesRepresentation.nodesByLimb);
            }

            if (nodeObjectsData != null && nodeObjectsData.Count > 0)
            {             
                nodeObjectsHandler.UpdateNodesData(nodeObjectsData);
            }
        }

        #endregion



    }
}

