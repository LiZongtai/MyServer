using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace MyServer
{
    public class ServerPeer
    {
        private Socket serverSocket;
        private Semaphore semaphore;
        private ClientPeerPool clientPeerPool;
        private IApplication application;
        public void SetApplication(IApplication application)
        {
            this.application = application;
        }
        // 开启服务器
        public void StartServer(string ip, int port, int maxClient)
        {
            try
            {
                clientPeerPool = new ClientPeerPool(maxClient);
                semaphore = new Semaphore(maxClient, maxClient);
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // fill the client peer pool
                for (int i = 0; i < maxClient; i++)
                {
                    ClientPeer temp = new ClientPeer();
                    temp.receiveCompleted = ReceiveProcessCompleted;
                    temp.ReceiveArgs.Completed += ReceiveArgs_Completed;
                    clientPeerPool.Enqueue(temp);
                }
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                serverSocket.Listen(maxClient);
                Console.WriteLine("server launched successfully!");
                StartAccept(null);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        #region proccess connection request
        //accept connection from client
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += E_Completed;
            }
            bool result = serverSocket.AcceptAsync(e);
            if (result == false)
            {
                ProcessAccept(e);
            }
        }
        // Asynchronous connection proccess completed
        private void E_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }
        // proccess connection request
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            semaphore.WaitOne();
            ClientPeer client = clientPeerPool.Dequeue();
            client.clientSocket = e.AcceptSocket;
            Console.WriteLine(client.clientSocket.RemoteEndPoint + " client connected successfully! ");
            // receive message
            StartReceive(client);
            e.AcceptSocket = null;
            StartAccept(e);
        }
        
        #endregion
        #region receive data
        private void StartReceive(ClientPeer client)
        {
            try
            {
                bool result = client.clientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    ProcessReceive(client.ReceiveArgs);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        private void ReceiveArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessReceive(e);
        }
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            // if receive successes
            if (client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, packet, 0, client.ReceiveArgs.BytesTransferred);
                client.ProcessReceive(packet);
                StartReceive(client);
            }
            // disconnected
            else
            {
                // no byte is transmitting
                if (client.ReceiveArgs.BytesTransferred == 0)
                {
                    // client disconnected actively
                    if (client.ReceiveArgs.SocketError == SocketError.Success)
                    {
                        Disconnect(client, " client disconnected actively ");
                    }
                    // disconnected passively
                    else
                    {
                        Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                    }
                }
            }
        }
        // recall after a message process completed
        private void ReceiveProcessCompleted(ClientPeer client, NetMsg msg)
        {
            application.Receive(client, msg);
        }
        #endregion
        #region disconnect
        private void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                if (client == null)
                {
                    throw new Exception(" client is null, can't disconnect ");
                }
                Console.Write(client.clientSocket.RemoteEndPoint + " client disconnected, the reason is " + reason);
                application.Disconnect(client);
                client.Disconnect();
                clientPeerPool.Enqueue(client);
                semaphore.Release();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
