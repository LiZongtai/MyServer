using MyServer;
using MySql.Data.MySqlClient;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Database
{
    public class DatabaseManager
    {
        private static MySqlConnection sqlConnection;
        private static Dictionary<int, ClientPeer> idClientDic;
        public static void StartConnect()
        {
            idClientDic = new Dictionary<int, ClientPeer>();
            string conStr = "database=cardgame;data source=127.0.0.1;port=3306;user=root;password=0615";
            sqlConnection = new MySqlConnection(conStr);
            sqlConnection.Open();
        }
        public static bool isExistUserName(string userName)
        {
            MySqlCommand cmd = new MySqlCommand("select Username from userinfo where Username=@name", sqlConnection);
            cmd.Parameters.AddWithValue("name", userName);
            MySqlDataReader reader=cmd.ExecuteReader();
            bool result= reader.HasRows;
            reader.Close();
            return result;
        }
        public static void CreateUser(string userName,string pwd)
        {
            MySqlCommand cmd = new MySqlCommand("insert into userinfo set Username = @name, Password = @pwd, Online = 0, IconName = @iconName", sqlConnection);
            cmd.Parameters.AddWithValue("name", userName);
            cmd.Parameters.AddWithValue("pwd", pwd);
            Random ran = new Random();
            int index = ran.Next(0, 19);
            cmd.Parameters.AddWithValue("iconName", "headIcon_" + index.ToString());
            cmd.ExecuteNonQuery();
        }
        // if username match with password
        public static bool isMatch(string username,string pwd)
        {
            MySqlCommand cmd = new MySqlCommand("select * from userinfo where Username=@name", sqlConnection);
            cmd.Parameters.AddWithValue("name", username);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                bool result=(reader.GetString("Password") == pwd);
                reader.Close();
                return result;
            }
            reader.Close();
            return false;
        }   
        //if user is Online
        public static bool isOnline(string username)
        {
            MySqlCommand cmd = new MySqlCommand("select Online from userinfo where Username=@name", sqlConnection);
            cmd.Parameters.AddWithValue("name", username);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                bool result = reader.GetBoolean("Online");
                reader.Close();
                return result;
            }
            reader.Close();
            return false;
        }
        public static void Login(string username,ClientPeer client)
        {
            MySqlCommand cmd = new MySqlCommand("update userinfo set Online=1 where Username=@name", sqlConnection);
            cmd.Parameters.AddWithValue("name", username);
            cmd.ExecuteNonQuery();

            MySqlCommand cmd1 = new MySqlCommand("select * from userinfo where Username=@name", sqlConnection);
            cmd1.Parameters.AddWithValue("name", username);
            MySqlDataReader reader = cmd1.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                int id = reader.GetInt32("Id");
                client.Id = reader.GetInt32("Id");
                client.Username = username;
                if (idClientDic.ContainsKey(id) == false)
                {
                    idClientDic.Add(id, client);
                }        
                reader.Close();
                Console.WriteLine("user " + username + " login succeeds! ");
                return;
            }
            reader.Close();
            return;
        }
        public static void Offline(ClientPeer client)
        {
            if (idClientDic.ContainsKey(client.Id))
            {
                idClientDic.Remove(client.Id);
            }
            MySqlCommand cmd = new MySqlCommand("update userinfo set Online=0 where Id=@id", sqlConnection);
            cmd.Parameters.AddWithValue("Id", client.Id);
            cmd.ExecuteNonQuery();
            Console.WriteLine("user " + client.Username + " is offline! ");
        }
        // obtain clientPeer by user id from dictionary
        public static ClientPeer GetClientPeerByUserId(int id)
        {
            if (idClientDic.ContainsKey(id))
            {
                return idClientDic[id];
            }
            return null;
        }
        public static UserDto CreateUserDto(int userId)
        {
            MySqlCommand cmd = new MySqlCommand("select * from userinfo where Id=@id", sqlConnection);
            cmd.Parameters.AddWithValue("Id", userId);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                UserDto dto = new UserDto(userId, reader.GetString("Username"), reader.GetString("IconName"), reader.GetInt32("Coin"));
                reader.Close();
                return dto;
            }
            reader.Close();
            return null;
        }
    }
}
