using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.Serialization;


namespace Axis.Communication
{
    public class UdpSocket : MonoBehaviour
    {
        [HideInInspector] public bool isTxStarted = false;

        [SerializeField] private string commandIP = "127.0.0.1"; // local host
        [SerializeField] private int rxPort = 45069; // port to receive data from Python on
        [SerializeField] private int txPort = 45068; // port to send data to Python on


        // Create necessary UdpClient objects
        private UdpClient _commandClient;
        private IPEndPoint _commandRemoteEndPoint;
        
        protected Thread RawDataReceiveThread; // Receiving Thread
        protected byte[] DataInBytes;
        protected bool DataWaitingForProcessing = false;
        private UdpClient _dataClient;
        private CancellationTokenSource _threadCancellation;

        protected Thread MessageReceiveThread;
        protected byte[] MessageInBytes;
        protected bool MessageWaitingForProcessing = false;

        public void SendData(byte[] data)
        {
            try
            {
                _commandClient?.Send(data, data.Length, _commandRemoteEndPoint);
            }
            catch (Exception err)
            {
                if (err is ObjectDisposedException)
                {
                    //Debug.Log("Got it");
                }
                else
                {
                    print(err.ToString());
                }
            }
        }
        [SerializeField] int multicastPort = 45071;
        //int _rawDataPort = 45071;
        IPEndPoint _rawDataIp;
        IPEndPoint _messageDataIp;

        protected void StartReceiveThread()
        {
            
            CreateCommandUpdClient();
            // Create a new thread for reception of incoming messages
            var rawDataSocket = CreateRawDataSocket();
            StartRawDataReceiveThread(rawDataSocket);

            var messageSocket = CreateMessageReceivingSocket();
            StartMessageReceiveThread(messageSocket);
        }
        private void CreateCommandUpdClient()
        {
            _commandRemoteEndPoint = new IPEndPoint(IPAddress.Parse(commandIP), txPort);
            _commandClient = new UdpClient();
            _commandClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }
       
        private void StartRawDataReceiveThread(Socket rawDataSocket)
        {
            if (_threadCancellation == null) _threadCancellation = new CancellationTokenSource();
            RawDataReceiveThread = new Thread(() => ReceiveRawData(rawDataSocket, _threadCancellation.Token));
            RawDataReceiveThread.Start();
        }
        private void StartMessageReceiveThread(Socket messageSocket)
        {
            if (_threadCancellation == null) _threadCancellation = new CancellationTokenSource();
            MessageReceiveThread = new Thread(() => ReceiveMessage(messageSocket, _threadCancellation.Token));
            MessageReceiveThread.Start();
        }
        private Socket CreateMessageReceivingSocket()
        {
            var messageAddress = IPAddress.Parse("239.255.239.174");
            _messageDataIp = new IPEndPoint(IPAddress.Any, rxPort);
            var messageSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            messageSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            messageSocket.Bind(_messageDataIp);
            messageSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(messageAddress, IPAddress.Any));
            messageSocket.Blocking = false;
            return messageSocket;

        }
        private Socket CreateRawDataSocket()
        {
            var rawDataGroupAddress = IPAddress.Parse("239.255.239.172");
            _rawDataIp = new IPEndPoint(IPAddress.Any, multicastPort);
            var rawDataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            rawDataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            rawDataSocket.Bind(_rawDataIp);
            rawDataSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(rawDataGroupAddress, IPAddress.Any));
            rawDataSocket.Blocking = false;
            return rawDataSocket;
        }
        
        // Receive data, update packets received
        private void ReceiveRawData(Socket socket, CancellationToken cancelToken)
        {
            var buffer = new byte[1024];
           // Debug.Log("Test");
            while (!cancelToken.IsCancellationRequested)
            {            
                try
                {                   
                    EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);

                    if (socket.Available <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    var bytesRead = socket.ReceiveFrom(buffer, ref remoteEp);

                    cancelToken.ThrowIfCancellationRequested();
                    
                    if (bytesRead <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    
                    DataInBytes = new byte[bytesRead];
                    Array.Copy(buffer, DataInBytes, bytesRead);
                    DataWaitingForProcessing = true;

                    if (!isTxStarted) // First data arrived so tx started
                    {
                        isTxStarted = true;
                    }

                    //ProcessInput(dataInBytes);
                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception err)
                {
                    Debug.LogException(err);
                }
            }
        }
        private void ReceiveMessage(Socket socket, CancellationToken cancelToken)
        {
            var buffer = new byte[1024];
            while (!cancelToken.IsCancellationRequested)
            {
                //try
                //{
                //    EndPoint remoteEp = new IPEndPoint(IPAddress.Parse("239.255.239.174"), rxPort);
                //    var debugBuffer = new byte[] { 0, 1, 2, 3, 4, 5 };
                //    socket.SendTo(debugBuffer, remoteEp);
                //}
                //catch (Exception err)
                //{
                //    if (err is ThreadAbortException)
                //    {
                //    }
                //    else
                //    {
                //        print(err.ToString());
                //    }
                //}
                try
                {
                    EndPoint remoteEp = new IPEndPoint(IPAddress.Any, 0);

                    if (socket.Available <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }

                    var bytesRead = socket.ReceiveFrom(buffer, ref remoteEp);

                    cancelToken.ThrowIfCancellationRequested();

                    //Debug.Log("Getting message");
                    if (bytesRead <= 0)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    
                    MessageInBytes = new byte[bytesRead];
                    Array.Copy(buffer, MessageInBytes, bytesRead);
                    MessageWaitingForProcessing = true;

                }
                catch (ThreadAbortException)
                {
                    Thread.ResetAbort();
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception err)
                {
                    Debug.LogException(err);
                }
            }
        }
      
        //Prevent crashes - close clients and threads properly!
        protected void StopReceivingThread()
        {
            if (_threadCancellation != null)
            {
                _threadCancellation.Cancel();
                _threadCancellation = null;
            }
            if (RawDataReceiveThread != null)
            {
                RawDataReceiveThread.Abort();
            }
            if (MessageReceiveThread != null)
            {
                MessageReceiveThread.Abort();
            }
        
            if (_commandClient != null)
            {
                _commandClient.Close();
            }
          
        }
    }
}