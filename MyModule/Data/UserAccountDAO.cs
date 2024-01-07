using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ERPSystem.Data
{
    public class UserAccountDAO
    {
        private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\source\repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security=True";
        public UserAccount fetchAccount(int ID)
        {
            UserAccount userDetail = new UserAccount();
            using (SqlConnection connectDB = new SqlConnection(connString))
            {
                connectDB.Open();

                using (var cmd = connectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ACCOUNT WHERE ACCID = @id and AccIsActive = 1";
                    cmd.Parameters.AddWithValue("@id", ID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                userDetail.UserID = Convert.ToInt32(reader["AccId"]);
                                userDetail.UserName = reader["AccUserName"].ToString();
                                userDetail.UserPassword = reader["AccPassword"].ToString();
                                userDetail.UserFname = reader["AccFname"].ToString();
                                userDetail.UserLname = reader["AccLname"].ToString();
                                userDetail.UserType = reader["AccType"].ToString();
                                userDetail.UserMname = reader["AccMname"].ToString();
                            }
                        }
                    }
                }
            }
            return userDetail; 
        }
        //
    }
}