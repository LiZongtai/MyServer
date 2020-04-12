using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto
{
    //data tranmission model of Account
    [Serializable]
    public class AccountDto
    {
        public string username;
        public string password;
        public AccountDto(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        public void Change(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
