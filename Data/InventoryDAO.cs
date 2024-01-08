using ERPSystems.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPSystems.Data
{
    internal class InventoryDAO
    {

        private string connectionString = @"Data Source=SETH\SQLEXPRESS;Initial Catalog=erpsystem;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public List<Inventory> FetchAll()
        {
            List<Inventory> returnList = new List<Inventory>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "select * from Inventory inner join Product on Product.ProdId = Inventory.ProdId inner join ProductCategory on ProductCategory.ProdcategoryId = Product.ProdcategoryId";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Inventory inventory = new Inventory();

                        inventory.ProdId = reader.GetInt32(0);
                        inventory.ProdName = reader.GetString(6);
                        inventory.ProdDescription = reader.GetString(7);
                        inventory.ProdQOH = reader.GetInt32(1);
                        inventory.ProdUnit = reader.GetString(2);
                        inventory.ProdPriceUnit = reader.GetDecimal(3);
                        inventory.ProcategoryName = reader.GetString(14);

                        returnList.Add(inventory);
                    }
                }
            }

            return returnList;
        }
    }
}