using Axis.Enumerations;
using Axis.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Axis.Communication.Commander
{
    public enum CommandType
    {
        MainHeader,
        ImuZero,
        Buzz,
        LedColor,
        ResetIMU,
        SetMode,
        Calibration,
        StartStreaming,
        StopStreaming,
        SetStreamingMode, 
        SinglePoseCalibration
    }

    [ExecuteAlways]
    public class AxisRuntimeCommander : MonoBehaviour
    {

        AxisRuntimeUdpSocket axisUdpSocket;

        private Dictionary<CommandType, byte[]> commandTypeToBytes = new Dictionary<CommandType, byte[]>() {
            { CommandType.MainHeader, new byte[] { 0x00, 0xEF, 0xAC, 0xEF, 0xAC } },
            { CommandType.ImuZero, new byte[] { 0x16,0x02,0x01 } },
            { CommandType.Buzz, new byte[] { 0x80 } },
            { CommandType.LedColor, new byte[] { 0x81 } },
            { CommandType.SetMode, new byte[] { 0x21 } },
            { CommandType.Calibration, new byte[] { 0x16,0x01,0x00 } },
            { CommandType.StartStreaming, new byte[] { 0x01, 0xE0 } },
            { CommandType.SetStreamingMode, new byte[] { 0x01, 0xE2, 0x01, 0x00} },
            { CommandType.StopStreaming, new byte[] { 0x01, 0xE1 } },
            { CommandType.SinglePoseCalibration, new byte[] { 0x16, 0x00, 0x01 } }
        };

        private void OnEnable()
        {
            
            AxisEvents.OnSetNodeLedColor += HandleOnSetNodeLedColor;
            AxisEvents.OnSetNodeVibration += HandleOnSetNodeVibration;      
            AxisEvents.OnCalibration += HandleOnReboot;
            AxisEvents.OnZeroAll += HandleOnZeroAll;
            AxisEvents.OnStartStreaming += HandleOnStartStreaming;
            AxisEvents.OnSinglePoseCalibration += HandleOnSingleCalibration;

            axisUdpSocket = GetComponent<AxisRuntimeUdpSocket>();
        }

        private void HandleOnStartStreaming()
        {
            IEnumerable<byte> data = commandTypeToBytes[CommandType.StartStreaming];
            axisUdpSocket.SendData(data.ToArray());
            data = commandTypeToBytes[CommandType.SetStreamingMode];
            axisUdpSocket.SendData(data.ToArray());

        }

        private void HandleOnReboot()
        {
            //byte rebootAllPaired = 0x00;
            // byte[] commandData = new byte[] {
            //   rebootAllPaired };

            IEnumerable<byte> data = commandTypeToBytes[CommandType.MainHeader]
                .Concat(commandTypeToBytes[CommandType.Calibration]);
               // .Concat(commandData);
            axisUdpSocket.SendData(data.ToArray());
            //Debug.Log("Reboot");
        }

        private void HandleOnTurnOffStream()
        {
            IEnumerable<byte> data = commandTypeToBytes[CommandType.StopStreaming];
            axisUdpSocket.SendData(data.ToArray());
        }

        private void HandleOnZeroAll()
        {
            // byte[] commandData = new byte[] {
            //   BitConverter.GetBytes(nodeIndex)[0] };

            IEnumerable<byte> data = commandTypeToBytes[CommandType.MainHeader]
                .Concat(commandTypeToBytes[CommandType.ImuZero]);
               // .Concat(commandData);

            axisUdpSocket.SendData(data.ToArray());
            //Debug.Log($"Command Data {commandData[0]}");
        }
        private void HandleOnSetNodeVibration(int nodeIndex, float intensity, float durationSeconds)
        {         

            byte[] commandData = new byte[] {
                BitConverter.GetBytes(nodeIndex)[0],
                GetByteFromNormalizedFloat(intensity),
                GetByteFromNormalizedFloat(durationSeconds/25.5f) };

            IEnumerable<byte> data = commandTypeToBytes[CommandType.MainHeader]
                .Concat(commandTypeToBytes[CommandType.Buzz])
                .Concat(commandData);

            axisUdpSocket.SendData(data.ToArray());
        }

        private static byte GetByteFromNormalizedFloat(float normalizedFloat)
        {
            return BitConverter.GetBytes(Mathf.RoundToInt(normalizedFloat * 255))[0];
        }

        private void HandleOnSetNodeLedColor(int nodeIndex, Color32 color, float brightness)
        {
            brightness = brightness > 1f ? 1f/3f : brightness/3f;

            byte[] commandData = new byte[] {
                BitConverter.GetBytes(nodeIndex)[0],
                color.r,
                color.b,
                color.g,
                GetByteFromNormalizedFloat(brightness)
                };

            IEnumerable<byte> data = commandTypeToBytes[CommandType.MainHeader]
                .Concat(commandTypeToBytes[CommandType.LedColor])
                .Concat(commandData);           

            axisUdpSocket.SendData(data.ToArray());
        }
        private void HandleOnSingleCalibration()
        {
            IEnumerable<byte> data = commandTypeToBytes[CommandType.MainHeader]
                .Concat(commandTypeToBytes[CommandType.SinglePoseCalibration]);
            axisUdpSocket.SendData(data.ToArray());
        }

    }
}

