using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
namespace MyServer
{
    public interface IApplication
    {
        // disconnect
        void Disconnect(ClientPeer client);
        // receive data
        void Receive(ClientPeer client, NetMsg msg);
    }
}