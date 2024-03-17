using Axis.DataTypes;
using Axis.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Axis.Communication
{
    
    [ExecuteAlways, Serializable]
    public class AxisRuntimeUdpSocket : UdpSocket
    {
        
        public static AxisRuntimeUdpSocket Instance;
        private AxisOutputData _axisOutputData = new();

        [SerializeField] public static bool ShouldConnectToAxis = false;
        [SerializeField] public static bool IsConnectedToAxis = false;
        
        //Data Packet Characteristics
        private const int DataStartOffset = 6;
        private const int NodeIndexOffset = 1;
        private const int DataSize = 15;
        private const int DataPacketSizeInBytes = 290;

        protected void Awake()
        {

            ShouldConnectToAxis = false;
            if (Instance == null)
            {
                transform.parent = Application.isPlaying == true ? null : transform.parent;              
                Instance = this;
                if (Application.isPlaying == true)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                if(Application.isPlaying == true)
                {
                    Destroy(gameObject);
                } else
                {
                    DestroyImmediate(gameObject);
                }
            }
        }

        private void HandleUpdate()
        {
#if UNITY_EDITOR
            // Ensure continuous Update calls.
            if (Application.isPlaying) return;
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
#endif
        }

        private void Update()
        {
            NullThreadIfNotAlive();
            
            if (CheckIfNeedsToConnect())
            {
                StopReceivingThread();
                StartReceiveThread();
                IsConnectedToAxis = true;
                AxisEvents.OnStartStreaming.Invoke();

            }
            else if (CheckIfNeedsToDisconnect())
            {
                StopReceivingThread();
                IsConnectedToAxis = false;
            }

            if (IsConnectedToAxis != true) return;
            
            ProcessDataIfReceived();
            if (DataWaitingForProcessing == true)
            {
                DataWaitingForProcessing = false;
            }
        }

        private void NullThreadIfNotAlive()
        {
            RawDataReceiveThread = RawDataReceiveThread == null ? null : RawDataReceiveThread.IsAlive == true ? RawDataReceiveThread : null;
        }

        private bool CheckIfNeedsToDisconnect()
        {
            return ShouldConnectToAxis == false && IsConnectedToAxis == true;
        }

        private bool CheckIfNeedsToConnect()
        {
            ShouldConnectToAxis = Application.isPlaying && RawDataReceiveThread == null || ShouldConnectToAxis;
            return (ShouldConnectToAxis == true && RawDataReceiveThread == null);
        }
        
        private void ProcessDataIfReceived()
        {
            if (DataInBytes == null || DataInBytes.Length != DataPacketSizeInBytes) return;
            
                
            GetDataFromHub(_axisOutputData);
            GetDataFromNodes(_axisOutputData);
            AxisEvents.OnAxisOutputDataUpdated?.Invoke(_axisOutputData);
        }
         
        private void GetDataFromNodes(AxisOutputData axisOutputData)
        {
            axisOutputData.nodesDataList = new List<AxisNodeData>();

            for (var i = 0; i < AxisOutputData.NodesCount; i++)
            {

                var x = (BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 0 + NodeIndexOffset)) * 0.00006103f;
                var z = (BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 2 + NodeIndexOffset)) * 0.00006103f;
                var y = (BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 4 + NodeIndexOffset)) * 0.00006103f;
                var w = (BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 6 + NodeIndexOffset)) * 0.00006103f;

                var xAccel = BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 8 + NodeIndexOffset) * 0.00390625f;
                var yAccel = BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 10 + NodeIndexOffset) * 0.00390625f;
                var zAccel = BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize) + 12 + NodeIndexOffset) * 0.00390625f;
                var nodeQuaternion = new Quaternion(x, y, z, w);
                var acceleration = new Vector3(xAccel, yAccel, zAccel);

                //var active = Convert.ToBoolean(BitConverter.ToInt16(DataInBytes, DataStartOffset + (i * DataSize)) & 0xFF00);
                bool active = Convert.ToBoolean(DataInBytes[DataStartOffset + (i * DataSize)] & 0b10000000);
                var axisNodeData = new AxisNodeData
                {
                    isActive = active
                };
                
                if (axisNodeData.isActive)
                {
                    axisNodeData.rotation = nodeQuaternion;
                    axisNodeData.accelerations = acceleration;
                }
                else
                {
                   
                    axisNodeData.rotation = Quaternion.identity;
                    axisNodeData.accelerations = Vector3.zero;
                }

                
                axisOutputData.nodesDataList.Add(axisNodeData);
            }
        }

        private void GetDataFromHub(AxisOutputData axisOutputData)
        {
            var hubDataStartingPosition = DataInBytes.Length - 28;

            var x = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition);
            var y = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 4);
            var z = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 8);
            var w = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 12);
            var rotation = new Quaternion(x, y, z, w);

            var xPos = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 16);
            var yPos = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 20);
            var zPos = BitConverter.ToSingle(DataInBytes, hubDataStartingPosition + 24);

  
            axisOutputData.hubData.absolutePosition = new Vector3(-xPos, yPos, zPos);          
            axisOutputData.hubData.rotation = rotation;

        }

        private void OnEnable()
        {


#if UNITY_EDITOR
            EditorApplication.update -= HandleUpdate;
            EditorApplication.update += HandleUpdate;
#endif
        }

        protected virtual void OnDisable()
        {
            if(Application.isPlaying == true)
            {
                ShouldConnectToAxis = false;
            }
            StopReceivingThread();
        }

     
    }
}
