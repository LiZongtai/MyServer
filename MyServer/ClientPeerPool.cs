using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace MyServer
{
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;
        public ClientPeerPool(int maxCount)
        {
            clientPeerQueue = new Queue<ClientPeer>(maxCount);
        }
        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }
        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }

    }
}
