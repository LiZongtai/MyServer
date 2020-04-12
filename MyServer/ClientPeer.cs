using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace MyServer
{
    public class ClientPeer
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Socket clientSocket { get; set; }
        private NetMsg msg;
        public ClientPeer()
        {
            msg = new NetMsg();
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;
            ReceiveArgs.SetBuffer(new byte[2048], 0, 2048);
        }
        #region receive
        // 
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        // store the received msg to buffer
        private List<byte> cache = new List<byte>();
        // if is processing the receive data
        private bool isProcessingReceive = false;
        public delegate void ReceiveCompleted(ClientPeer client, NetMsg msg);
        public ReceiveCompleted receiveCompleted;
        public void ProcessReceive(byte[] packet)
        {
            cache.AddRange(packet);
            if (isProcessingReceive == false)
            {
                ProcessData();
            }
        }
        private void ProcessData()
        {
            isProcessingReceive = true;
            // get a completed packet from cache
            byte[] packet = EncodeTool.DecodePacket(ref cache);
            if (packet == null)
            {
                isProcessingReceive = false;
                return;
            }
            NetMsg msg = EncodeTool.DecodeMsg(packet);
            if (receiveCompleted != null)
            {
                receiveCompleted(this, msg);
            }
            ProcessData();
        }
        #endregion
        #region send msg
        public void SendMsg(int opCode, int subCode, object value)
        {
            msg.Change(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            SendMsg(packet);
        }
        public void SendMsg(byte[] packet)
        {
            try
            {
                clientSocket.Send(packet);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region disconnect
        // client disconnected
        public void Disconnect()
        {
            cache.Clear();
            isProcessingReceive = false;
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            clientSocket = null;
        }
        #endregion

    }
}