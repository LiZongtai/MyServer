using MySql.Data.MySqlClient;
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
        public static void StartConnect()
        {
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
    }
}
