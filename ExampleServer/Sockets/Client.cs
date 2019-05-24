using ExampleDataTransferObjects;
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExampleServer.Sockets
{
    public delegate void OnExampleDTOReceived(ExampleDTO eDTO);

    public class Client
    {
        Socket _Socket;
        public Client(Socket socket)
        {
            _Socket = socket;
        }

        public void Start()
        {
      //datayı dinlemeye başla
            _Socket.BeginReceive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None, OnBeginReceiveCallback, null);
        }

        public OnExampleDTOReceived _OnExampleDTOReceived;
         
        SocketError socketError;
        byte[] tempBuffer = new byte[1024]; 
        void OnBeginReceiveCallback(IAsyncResult asyncResult)
        {
            int receivedDataLength = _Socket.EndReceive(asyncResult, out socketError);

            if (receivedDataLength <= 0 && socketError != SocketError.Success)
            {
              
                return;
            }
            byte[] resizedBuffer = new byte[receivedDataLength];

            Array.Copy(tempBuffer, 0, resizedBuffer, 0, resizedBuffer.Length);
            
            HandleReceivedData(resizedBuffer);
            
            _Socket.BeginReceive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None, OnBeginReceiveCallback, null);
        }
        void HandleReceivedData(byte[] resizedBuffer)
        {
            if (_OnExampleDTOReceived != null)
            {
                using (var ms = new MemoryStream(resizedBuffer))
                {
                    ExampleDTO exampleDTO = new BinaryFormatter().Deserialize(ms) as ExampleDTO;

                    _OnExampleDTOReceived(exampleDTO);
                }
            }
        }
 
    }
}