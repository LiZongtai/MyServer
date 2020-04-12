using MyServer;
using Protocol.Code;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Database;

namespace GameServer.Logic
{
    public class AccountHandler : IHandler
    {
        public void Disconnect(ClientPeer client)
        {
            
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.Register_CREQ:
                    //Console.WriteLine(AccountCode.Register_CREQ);
                    Register(client, value as AccountDto);
                    break;
                default:
                    Console.WriteLine("no Account Code");
                    break;
            }
        }
        //client register process
        private void Register(ClientPeer client, AccountDto dto)
        {
            // username has been registered
            //Console.WriteLine(dto.username.ToString());
            if (DatabaseManager.isExistUserName(dto.username))
            {
                Console.WriteLine("userinfo exited");
                client.SendMsg(OpCode.Account, AccountCode.Register_SRES, -1);
                return;
            }
            // create a new userinfo
            DatabaseManager.CreateUser(dto.username, dto.password);
            Console.WriteLine("create a new userinfo, username is: "+ dto.username.ToString());
            client.SendMsg(OpCode.Account, AccountCode.Register_SRES, 0);
        }
    }
}
