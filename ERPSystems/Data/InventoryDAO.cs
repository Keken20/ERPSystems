using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPSystem.Data
{
    internal class InventoryDAO
    {
        private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\Source\Repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security = True";

        public List<ViewModel> FetchAll()
        {
            List<ViewModel> returnItems = new List<ViewModel>();
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT Invt_InDateAt, Inventory.ProdId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, INVENTORY.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.ProdId = product.ProdId WHERE Inventory.Invt_IsActive = 1";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel inventoryItem = new ViewModel();
                                inventoryItem.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                //inventoryItem.prodCode = reader["ProdCode"].ToString();
                                inventoryItem.prod_Name = reader["ProdName"].ToString();
                                inventoryItem.prod_Description = reader["ProdDescription"].ToString();
                                inventoryItem.inv_QOH = Convert.ToInt32(reader["invt_QOH"]);
                                inventoryItem.ProdUnit = reader["ProdUnit"].ToString();
                                inventoryItem.ProdPriceUnit = reader["ProdPriceUnit"] != DBNull.Value ? Convert.ToDouble(reader["ProdPriceUnit"]) : 0; ;
                                inventoryItem.ProdTotalPrice = reader["ProdTotalPrice"] != DBNull.Value ? Convert.ToDouble(reader["ProdTotalPrice"]) : 0; ;
                                inventoryItem.inv_InDate = Convert.ToDateTime(reader["Invt_InDateAt"]);
                                inventoryItem.isActive = Convert.ToInt32(reader["Invt_IsActive"]);
                                returnItems.Add(inventoryItem);
                            }
                        }
                        return returnItems;
                    }
                }
            }
        }

        public int CreateItem(InventoryModel inventoryModel)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                try
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO Inventory(ProdId, Invt_QOH, ProdUnit, " +
                        "ProdPriceUnit,ProdTotalPrice) values(@pID, @QOH, @unitMeasure, @uPrice, @totPrice)";
                        cmd.Parameters.AddWithValue("@pID", inventoryModel.prod_Id);
                        cmd.Parameters.AddWithValue("@QOH", inventoryModel.inv_QOH);
                        cmd.Parameters.AddWithValue("@unitMEasure", inventoryModel.ProdUnit);
                        cmd.Parameters.AddWithValue("@uPrice", inventoryModel.ProdUnitPrice);
                        double total = (double)(inventoryModel.ProdUnitPrice * inventoryModel.inv_QOH);
                        cmd.Parameters.AddWithValue("@totPrice", total);
                        cmd.Parameters.AddWithValue("@active", 1);
                        var itemID = cmd.ExecuteNonQuery();
                        return itemID;
                    }
                }
                catch (Exception)
                {
                    return -1;
                }

            }
        }

        public int UpdateItem(InventoryModel inventoryModel)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE Inventory SET invt_QOH = @QOH,  ProdUnit = @unitMeasure, " +
                                    "ProdPriceUnit = @uPrice, ProdTotalPrice = @totPrice, Invt_UpDateAt = @update WHERE ProdId = @pID";

                        cmd.Parameters.AddWithValue("@pID", inventoryModel.prod_Id);
                        cmd.Parameters.AddWithValue("@QOH", inventoryModel.inv_QOH);
                        cmd.Parameters.AddWithValue("@unitMEasure", inventoryModel.ProdUnit);
                        cmd.Parameters.AddWithValue("@uPrice", inventoryModel.ProdUnitPrice);
                        double total = (double)(inventoryModel.ProdUnitPrice * inventoryModel.inv_QOH);
                        cmd.Parameters.AddWithValue("@totPrice", total);
                        cmd.Parameters.AddWithValue("@update", inventoryModel.invt_UpDate);
                        cmd.Parameters.AddWithValue("@active", 1);

                        var itemID = cmd.ExecuteNonQuery();
                        return itemID;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }


            }
        }

        internal int itemDelete(int ID)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE Inventory set Invt_IsActive = @deactivate where ProdId = @pID";

                        cmd.Parameters.AddWithValue("@pID", ID);
                        cmd.Parameters.AddWithValue("@deactivate", 0);

                        int deleteID = cmd.ExecuteNonQuery();

                        return deleteID;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
        }

        public InventoryModel FetchOne(int ID)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM INVENTORY WHERE ProdId = @id";

                    cmd.Parameters.AddWithValue("@id", ID);

                    InventoryModel inventoryItem = new InventoryModel();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                inventoryItem.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                inventoryItem.inv_QOH = Convert.ToInt32(reader["invt_QOH"]);
                                inventoryItem.ProdUnit = reader["ProdUnit"].ToString();
                                inventoryItem.ProdUnitPrice = Convert.ToDouble(reader["ProdPriceUnit"]);
                                inventoryItem.totalPrice = Convert.ToDouble(reader["ProdTotalPrice"]);
                                inventoryItem.invt_InDate = Convert.ToDateTime(reader["Invt_InDateAt"]);
                                inventoryItem.isActive = Convert.ToInt32(reader["Invt_IsActive"]);
                            }
                        }
                        return inventoryItem;
                    }
                }
            }
        }

        internal List<ViewModel> SearchID(string searchPhrase)
        {
            List<ViewModel> returnItems = new List<ViewModel>();

            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT Invt_InDateAt, Inventory.ProdId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Invt_IsActive " +
                        "from Inventory INNER JOIN product on Inventory.ProdId = product.ProdId WHERE (product.prodName LIKE '%'+@searchkey+'%' OR product.prodId LIKE '%'+@searchkey+'%' or product.prodDescription LIKE '%'+@searchkey+'%' OR " +
                        "invt_QOH LIKE '%'+@searchkey+'%' or ProdUnit LIKE '%'+@searchkey+'%' OR ProdPriceUnit LIKE '%'+@searchkey+'%' OR ProdTotalPrice LIKE '%'+@searchkey+'%') AND Invt_IsActive = 1 ORDER BY INVENTORY.PRODID ASC";

                    cmd.Parameters.AddWithValue("@searchkey", searchPhrase);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel inventoryItem = new ViewModel();
                                inventoryItem.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                inventoryItem.prod_Name = reader["ProdName"].ToString();
                                inventoryItem.prod_Description = reader["ProdDescription"].ToString();
                                inventoryItem.inv_QOH = Convert.ToInt32(reader["invt_QOH"]);
                                inventoryItem.ProdUnit = reader["ProdUnit"].ToString();
                                inventoryItem.ProdPriceUnit = Convert.ToDouble(reader["ProdPriceUnit"]);
                                inventoryItem.ProdTotalPrice = Convert.ToDouble(reader["ProdTotalPrice"]);
                                inventoryItem.inv_InDate = Convert.ToDateTime(reader["Invt_InDateAt"]);
                                inventoryItem.isActive = Convert.ToInt32(reader["Invt_IsActive"]);
                                returnItems.Add(inventoryItem);
                            }
                        }
                    }
                    return returnItems;
                }
            }
        }

        internal int itemReactivation(int ID)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE Inventory set Invt_IsActive = @activate where ProdId = @pID";

                        cmd.Parameters.AddWithValue("@pID", ID);
                        cmd.Parameters.AddWithValue("@activate", 1);

                        int deleteID = cmd.ExecuteNonQuery();

                        return deleteID;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
        }

        public List<ViewModel> ItemSort(string sortedName)
        {

            List<ViewModel> sortedItem = new List<ViewModel>();

            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = FetchSqlQuery(sortedName);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel inventoryItem = new ViewModel();
                                inventoryItem.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                inventoryItem.prod_Name = reader["ProdName"].ToString();
                                inventoryItem.prod_Description = reader["ProdDescription"].ToString();
                                inventoryItem.inv_QOH = Convert.ToInt32(reader["invt_QOH"]);
                                inventoryItem.ProdUnit = reader["ProdUnit"].ToString();
                                inventoryItem.ProdPriceUnit = Convert.ToDouble(reader["ProdPriceUnit"]);
                                inventoryItem.ProdTotalPrice = Convert.ToDouble(reader["ProdTotalPrice"]);
                                inventoryItem.inv_InDate = Convert.ToDateTime(reader["Invt_InDateAt"]);
                                inventoryItem.isActive = Convert.ToInt32(reader["Invt_IsActive"]);
                                sortedItem.Add(inventoryItem);
                            }
                        }
                    }
                    return sortedItem;
                }
            }
        }

        //used in Item sort
        private string FetchSqlQuery(string sortName)
        {
            switch (sortName)
            {
                case "product.PRODID":
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    $"ORDER BY product.PRODID ASC";

                case "product.prodName":
                    return  "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    "ORDER BY product.prodName ASC";

                case "invt_QOH":
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    "ORDER BY invt_QOH ASC";

                case "INVT_INDATEAT":
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    "ORDER BY INVT_INDATEAT ASC";

                case "PRODPRICEUNIT":
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    "ORDER BY PRODPRICEUNIT ASC";

                case "Invt_IsActive":
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 0 " +
                    "ORDER BY PRODPRICEUNIT ASC";

                default:
                    return "SELECT invt_InDateAt, Inventory.prodId, product.prodName, product.prodDescription, invt_QOH, ProdUnit, ProdPriceUnit, ProdTotalPrice, Inventory.Invt_IsActive " +
                    "from Inventory INNER JOIN product on Inventory.prodid = product.prodid WHERE Inventory.Invt_isActive = 1 " +
                    "ORDER BY PRODID ASC";
            }
        }
    }
}