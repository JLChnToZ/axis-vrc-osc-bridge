using UnityEngine;


using Axis.DataTypes;
using Axis.Enumerations;

namespace Axis.Utils
{
    public class ActiveNodesDetector
    {
        bool firstRun = true;
        public bool[] activeNodes = new bool[AxisOutputData.NodesCount];
        public Quaternion[] rotationsFromLastOutputData = new Quaternion[AxisOutputData.NodesCount];
        public Vector3[] accelerationsFromLastOutputData = new Vector3[AxisOutputData.NodesCount];

        public Quaternion hubRotationFromLastOutputData;
        private Vector3 hubPositionFromLastOuputData;

        public void UpdateActiveNodes(AxisOutputData axisOutputData)
        {
            if (firstRun == true)
            {
                firstRun = false;
                StoreLastOutputData(axisOutputData);
            }

            for (int i = 0; i < AxisOutputData.NodesCount; i++)
            {
                //float rotationActivity = axisOutputData.nodesDataList[i] 
                if (axisOutputData.nodesDataList[i].isActive == false)
                {
                   
                    if(Equals(axisOutputData.nodesDataList[i].rotation, rotationsFromLastOutputData[i]) == false)
                    {
                        
                        axisOutputData.nodesDataList[i].isActive = true;
                        

                    }
                }

                
            }


            if (axisOutputData.hubData.isActive == false)
            {
                if(axisOutputData.hubData.absolutePosition != hubPositionFromLastOuputData)
                {
                    axisOutputData.hubData.isActive = true;
                }

                if (Mathf.Abs(Quaternion.Angle(axisOutputData.hubData.rotation, hubRotationFromLastOutputData)) > 0.001 == true)
                {
                    float angle = Quaternion.Angle(axisOutputData.hubData.rotation, hubRotationFromLastOutputData);
                    axisOutputData.hubData.isActive = true;
                }
            }

            StoreLastOutputData(axisOutputData);
        }

        private void StoreLastOutputData(AxisOutputData axisOutputData)
        {


            for (int i = 0; i < axisOutputData.nodesDataList.Count; i++)
            {
                rotationsFromLastOutputData[i] = axisOutputData.nodesDataList[i].rotation;
                accelerationsFromLastOutputData[i] = axisOutputData.nodesDataList[i].accelerations;
            }

            //Debug.Log($"Last Output {hubRotationFromLastOutputData} {axisOutputData.hubData.rotation}");

            hubRotationFromLastOutputData = axisOutputData.hubData.rotation;
            hubPositionFromLastOuputData = axisOutputData.hubData.absolutePosition;
        }
    }


}
