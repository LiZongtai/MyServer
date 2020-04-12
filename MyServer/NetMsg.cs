using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;

namespace MyServer
{
    public class NetMsg
    {
        // operation code
        public int opCode { get; set; }
        public int subCode { get; set; }
        // parament transmitted
        public object value { get; set; }
        public NetMsg()
        {

        }
        public NetMsg(int opCode, int subCode, object value)
        {
            this.opCode = opCode;
            this.subCode = subCode;
            this.value = value;
        }
        public void Change(int opCode, int subCode, object value)
        {
            this.opCode = opCode;
            this.subCode = subCode;
            this.value = value;
        }
    }
}