using Axis.DataProcessing;
using Axis.Enumerations;
using Axis.Elements.AnimatorLink;
using System;
using System.Collections.Generic;
using UnityEngine;
using Axis.DataTypes;
using Axis.Events;
using Axis.Solvers;
using Axis.Overrides;



namespace Axis.Elements
{

    //Axis Mannequin purpose is to be a humanoid character that don't interact directly to the 
    //world, but serves as a model for any application using Axis as a body tracking system for example
    //the class CharacterAnimatorLink uses the AxisMannequin data to animate any humanoid character.
    public class AxisMannequin : NodeProcessor
    {
        #region Class Variables


        public GameObject bodyModel;        
        public BodyModelAnimatorLink bodyModelAnimatorLink;

        public Action<AxisHubData> onHubDataUpdated;
        private HipsRotationSolver hipsRotationSolver;
        private TorsoRotationSolver torsoRotationSolver;

        #endregion

        #region Initialization
        public override void Initialize(string uniqueID)
        {
            brainUniqueId = uniqueID;
            ExecuteOverrides();
            SetupBodyModelAnimationLink(bodyModelAnimatorLink);
            LoadSolvers();
        }

        private void ExecuteOverrides()
        {
            foreach (AxisExecuteOnStartOverride axisOverride in GetComponents<AxisExecuteOnStartOverride>())
            {
                axisOverride.Execute();
            }
            
        }


        //Solvers are components that use specific strategies to handle the kinematic interpretation of the Axis Nodes data
        protected virtual void LoadSolvers()
        {
            hipsRotationSolver = GetComponent<HipsRotationSolver>() == null ? 
                gameObject.AddComponent<MannequinHipsFollowSpineSolver>() : 
                GetComponent<HipsRotationSolver>();

            torsoRotationSolver = GetComponent<TorsoRotationSolver>() == null ?
                gameObject.AddComponent<TorsoRotationFromAxisSolver>() :
                GetComponent<TorsoRotationSolver>();
        }

        #region Nodes Instantiation
        

        public void SetVisibility(bool isVisible)
        {
           foreach (Renderer renderer in bodyModel.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = isVisible;
            }
        }


        #endregion


        #region ObjectsSetup
        

        public void SetupBodyModelAnimationLink(BodyModelAnimatorLink bodyModelAnimatorLink)
        {
            bodyModelAnimatorLink.Animator = bodyModelAnimatorLink.transform.GetComponent<Animator>();
            bodyModelAnimatorLink.FetchTransformsRelatedToNodeLimbs();
            bodyModelAnimatorLink.GoToTPose();
            bodyModelAnimatorLink.StoreDefaultTransformsRotations();
        }

        #endregion

        #endregion

        #region DataUpdateHandling

		
        internal void UpdateNodesData(Dictionary<NodeBinding, AxisNodeData> nodesDataDictionary, Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            foreach (var key in nodesDataDictionary.Keys)
            {
                nodesByLimb[key].SetRotation(AxisDataUtility.ConvertRotationBasedOnKey(key, nodesDataDictionary[key].rotation));
                nodesByLimb[key].SetAcceleration(nodesDataDictionary[key].accelerations); 
            }
        }

        internal void UpdateHubData(AxisHubData hubData, Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            //This callback is used by Solvers to update data
           
            onHubDataUpdated?.Invoke(hubData);
           // nodesByLimb[NodeBinding.WaistHub].SetRotation(AxisDataUtility.ConvertRotationBasedOnKey(NodeBinding.WaistHub, hubData.rotation));
               
        }
        internal void UpdateTorso(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            //nodesByLimb[NodeBinding.WaistHub].SetRotation(Quaternion.identity);


        }
        
        internal void UpdateHips(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
           
        }


        public Action<BodyModelAnimatorLink> onBodyModelAnimatorLinkUpdated;
        //After generating the pose by modifying the rotations on the BodyModelAnimatorLink, iterate through all the characters
		//and update the respectives transforms for mirroring the pose of the mannequin.
        public virtual void HandleNodeDataUpdated(Dictionary<NodeBinding, AxisNode> nodesByLimb)
        {
            AxisEvents.OnNodeByLimbsUpdated?.Invoke(brainUniqueId, nodesByLimb);
            bodyModelAnimatorLink.UpdateTransforms(nodesByLimb);    
            onBodyModelAnimatorLinkUpdated?.Invoke(bodyModelAnimatorLink);
            
        }
        public virtual void HandleAxisDataUpdated(Dictionary<NodeBinding, AxisNode> nodesByLimb,HipProvider hipProvider,AxisHubData axisHubData)
        {
            AxisEvents.OnNodeByLimbsUpdated?.Invoke(brainUniqueId, nodesByLimb);
            bodyModelAnimatorLink.UpdateTransforms(nodesByLimb);
            bodyModelAnimatorLink.UpdateHipTransform(nodesByLimb,hipProvider, axisHubData);
            onBodyModelAnimatorLinkUpdated?.Invoke(bodyModelAnimatorLink);
        }
       // public virtual void HandleHubDataUpdated()

        #endregion


    }

}



