using ExampleDataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExampleClient.Sockets
{
    public class ExampleSocket
    {
        Socket _Socket;
        IPEndPoint _IPEndPoint;

        public ExampleSocket(IPEndPoint ipEndPoint)
        {
            _IPEndPoint = ipEndPoint;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        SocketError socketError;
        byte[] tempBuffer = new byte[1024];
        public void Start()
        {
            _Socket.BeginConnect(_IPEndPoint, OnBeginConnect, null);
        }
        void OnBeginConnect(IAsyncResult asyncResult)
        {
            try
            {
                _Socket.EndConnect(asyncResult);
                _Socket.BeginReceive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None, OnBeginReceive, null);
            }
            catch (SocketException ex)
            {

                Console.WriteLine("Servera bağlanılamıyor!");
            }
        }

        void OnBeginReceive(IAsyncResult asyncResult)
        {

            int receivedDataLength = _Socket.EndReceive(asyncResult, out socketError);

            if (receivedDataLength <= 0 || socketError != SocketError.Success)
            {

                Console.WriteLine("Server bağlantısı koptu!");
                return;
            }
            _Socket.BeginReceive(tempBuffer, 0, tempBuffer.Length, SocketFlags.None, OnBeginReceive, null);
        }
        public void SendData(ExampleDTO exampleDTO)
        {
            using (var ms = new MemoryStream())
            {

                new BinaryFormatter().Serialize(ms, exampleDTO);
                IList<ArraySegment<byte>> data = new List<ArraySegment<byte>>();

                data.Add(new ArraySegment<byte>(ms.ToArray()));


                _Socket.BeginSend(data, SocketFlags.None, out socketError,
                    (asyncResult) =>
                {
                    int length = _Socket.EndSend(asyncResult, out socketError);

                    if (length <= 0 || socketError != SocketError.Success)
                    {
                        Console.WriteLine("Server bağlantısı koptu!");
                        return;
                    }
                }, null);

                if (socketError != SocketError.Success)
                    Console.WriteLine("Server bağlantısı koptu!");
            }
        }



    }
}
