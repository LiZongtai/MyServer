using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Protocol.Code
{
    public class OpCode
    {
        public const int Account = 0;
        public const int Match = 1;
        public const int Chat = 2;
        public const int Fight = 3;
    }
}