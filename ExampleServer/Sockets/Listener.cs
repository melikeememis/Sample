using ExampleDataTransferObjects;
using System;
using System.Net;
using System.Net.Sockets;

namespace ExampleServer.Sockets
{
    public class Listener
    {
    
        Socket _Socket;
        
        int _Port;
        int _MaxConnectionQueue;
        public Listener(int port, int maxConnectionQueue)
        {
            _Port = port;
            _MaxConnectionQueue = maxConnectionQueue;
            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, _Port);
            _Socket.Bind(ipEndPoint);
            _Socket.Listen(_MaxConnectionQueue);
            _Socket.BeginAccept(OnBeginAccept, _Socket);
        }

        void OnBeginAccept(IAsyncResult asyncResult)
        {
            Socket socket = _Socket.EndAccept(asyncResult);

            Client client = new Client(socket);
            client._OnExampleDTOReceived += new Sockets.OnExampleDTOReceived(OnExampleDTOReceived);
            client.Start();
            _Socket.BeginAccept(OnBeginAccept, null);
        }

        void OnExampleDTOReceived(ExampleDTO exampleDTO)
        {
            Console.WriteLine(string.Format("Status: {0}", exampleDTO.Status));
            Console.WriteLine(string.Format("Message: {0}", exampleDTO.Message));
        }

    }
}