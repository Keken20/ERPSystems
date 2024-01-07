using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ERPSystem.Data
{
    public class DashviewDAO
    {
        private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\source\repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security=True";

        public int countAccounts()
        {
            int getCount = 0;
            using (SqlConnection connectDB = new SqlConnection(connString))
            {
                connectDB.Open();

                using (var cmd = connectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) FROM ACCOUNT WHERE AccIsActive = 1";

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }

        public int countInventory()
        {
            int getCount = 0;
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) AS ICOUNT FROM INVENTORY WHERE INVT_ISACTIVE = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }

        public int countSupplier()
        {
            int getCount = 0;
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) AS ICOUNT FROM SUPPLIER WHERE INVT_ISACTIVE = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }

        public int countRFSubmit()
        {
            int getCount = 0;
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) AS ICOUNT FROM REQUISITIONFORM WHERE  ReqIsActive = 1 AND RFSubmit = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }

        public int totalReqForm()
        {
            int getCount = 0;
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) AS ICOUNT FROM REQUISITIONFORM WHERE ReqIsActive = 1 ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }

        public int countPO()
        {
            int getCount = 0;
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT COUNT(*) AS ICOUNT FROM PurchaseOrderForm WHERE ReqIsActive = 1 ";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.Read())
                            {
                                getCount = Convert.ToInt32(reader["ICOUNT"]);
                            }
                            else
                            {
                                getCount = 0;
                            }
                        }
                        return getCount;
                    }
                }
            }
        }
    }
}