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
            DatabaseManager.Offline(client);
        }

        public void Receive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.Register_CREQ:
                    //Console.WriteLine(AccountCode.Register_CREQ);
                    Register(client, value as AccountDto);
                    break;
                case AccountCode.Login_CREQ:
                    Login(client, value as AccountDto);
                    break;
                case AccountCode.GetUserInfo_CREQ:
                    GetUserInfo(client);
                    break;
                default:
                    Console.WriteLine("no Account Code");
                    break;
            }
        }
        //client obtains userinfo request
        private void GetUserInfo(ClientPeer client)
        {
            SingleExecute.Instance.Execute(() =>
            {
                UserDto dto = DatabaseManager.CreateUserDto(client.Id);
                client.SendMsg(OpCode.Account, AccountCode.GetUserInfo_SRES, dto);
            });
        }
        // client login request
        private void Login(ClientPeer client,AccountDto dto)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (DatabaseManager.isExistUserName(dto.username) == false)
                {
                    // userinfo inexistent
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -1);
                    return;
                }
                if (DatabaseManager.isMatch(dto.username, dto.password) == false)
                {
                    // mistake password;
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -2);
                    return;
                }
                if (DatabaseManager.isOnline(dto.username) == true)
                {
                    // user is already Online
                    client.SendMsg(OpCode.Account, AccountCode.Login_SRES, -3);
                    return;
                }
                //login successfully
                DatabaseManager.Login(dto.username, client);
                client.SendMsg(OpCode.Account, AccountCode.Login_SRES, 0);
            });
        }
        //client register process
        private void Register(ClientPeer client, AccountDto dto)
        {
            // single thread execute
            // prevent from multiple threads accessing simultaneously
            SingleExecute.Instance.Execute(() =>
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
                Console.WriteLine("create a new userinfo, username is: " + dto.username.ToString());
                client.SendMsg(OpCode.Account, AccountCode.Register_SRES, 0);
            });
           
        }
    }
}
