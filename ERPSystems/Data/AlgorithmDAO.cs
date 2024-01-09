using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using ERPSystems.Models;

namespace ERPSystems.Data
{
    public class AlgorithmDAO
    {
        private string connString = ERPSystems.Properties.Resources.ConnectionString;

        public double calculateMRF(InventoryModel product)
        {
            //Calculate Annual Restock Frequency
            double setNumMonths = 1; //change value up to 12 where 1 value per month
            return (double)product.totalStock / setNumMonths; 
        }

        public double getMonthlyUV(double ucost, double mrf)
        {
            return ucost * mrf; //unit cost * monthly restocking frequency
        }

        public string AssignA(int product, double ARF)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO CATEGORY_A VALUES(@pName, @arf)";
                        cmd.Parameters.AddWithValue("@pName", product);
                        cmd.Parameters.AddWithValue("@arf", ARF);

                        var row = cmd.ExecuteNonQuery();
                        if(row > 0)
                        {
                            return "ITEM ASSIGNED TO CATEGORY A";
                        }
                        else
                        {
                            return "INSERTION FAILED";
                        }
                    }
                }
            }
            catch(Exception)
            {
                return "SOMETHING WENT WRONG";
            }
        
        }


        public string AssignB(int product, double ARF)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO CATEGORY_B VALUES(@pName, @arf)";
                        cmd.Parameters.AddWithValue("@pName", product);
                        cmd.Parameters.AddWithValue("@arf", ARF);

                        var row = cmd.ExecuteNonQuery();
                        if (row > 0)
                        {
                            return "ITEM ASSIGNED TO CATEGORY B";
                        }
                        else
                        {
                            return "INSERTION FAILED";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "SOMETHING WENT WRONG";
            }

        }

        public string AssignC(int product, double ARF)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO CATEGORY_C VALUES(@pName, @arf)";
                        cmd.Parameters.AddWithValue("@pName", product);
                        cmd.Parameters.AddWithValue("@arf", ARF);

                        var row = cmd.ExecuteNonQuery();
                        if (row > 0)
                        {
                            return "ITEM ASSIGNED TO CATEGORY C";
                        }
                        else
                        {
                            return "INSERTION FAILED";
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "SOMETHING WENT WRONG";
            }
        }

        public List<PriorityItem> categoriesA()
        {
            List<PriorityItem> priorityItems = new List<PriorityItem>();
            try
            {
                using (var ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * from CATEGORY_A";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    PriorityItem item = new PriorityItem();
                                    item.prodname = Convert.ToInt32(reader["ItemName"]);
                                    item.ARFvalue = Convert.ToDouble(reader["ItemARF"]);
                                    priorityItems.Add(item);
                                }
                            }
                            return priorityItems;
                        }
                    }
                }
            }
            catch
            {
                return priorityItems;
            }

        }


    }
}
