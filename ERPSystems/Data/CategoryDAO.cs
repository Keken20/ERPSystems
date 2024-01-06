using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPSystem.Data

{
    internal class CategoryDAO
    {
        private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\Source\Repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security = True";

        public List<CategoryModel> FetchAll()
        {
            List<CategoryModel> returnCategory = new List<CategoryModel>();

            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();

                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM ProductCategory where IsActive = 1";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                CategoryModel category = new CategoryModel();
                                category.categID = Convert.ToInt32(reader["ProdCategoryId"]);
                                category.CategoryName = reader["ProdcategoryName"].ToString();
                                category.categ_IsActive = Convert.ToInt32(reader["IsActive"]);

                                returnCategory.Add(category);
                            }
                        }
                    }
                }
            }

            return returnCategory;
        }

        public int CreateCategory(CategoryModel categoryModel)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                try
                {
                    ConnectDB.Open();

                    string sqlQuery = "INSERT INTO PRODUCTCATEGORY(PRODCATEGORYNAME) VALUES(@PCNAME)";

                    SqlCommand cmd = new SqlCommand(sqlQuery, ConnectDB);
                    cmd.Parameters.AddWithValue("@PCNAME", categoryModel.CategoryName);
                    var categID = cmd.ExecuteNonQuery();
                    return categID;
                }
                catch (Exception)
                {
                    return -1;
                }

            }
        }

        internal int DeleteCateg(int ID)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE ProductCategory set IsActive = @deactivate where ProdcategoryId = @pID";
                    
                    cmd.Parameters.AddWithValue("@pID", ID);
                    cmd.Parameters.AddWithValue("@deactivate", 0);
                    int deleteID = cmd.ExecuteNonQuery();
                    return deleteID;
                }
             
            }
        }

        public CategoryModel FetchOne(int ID)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM PRODUCTCATEGORY WHERE ProdCategoryID = @id and IsActive = 1";
                    cmd.Parameters.AddWithValue("@id", ID);

                    CategoryModel category = new CategoryModel();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                category.categID = Convert.ToInt32(reader["ProdcategoryID"]);
                                category.CategoryName = reader["ProdcategoryName"].ToString();
                                category.categ_IsActive = Convert.ToInt32(reader["IsActive"]);
                            }
                        }
                        return category;
                    } 
                }
            }
                   
        }

        public int UpdateCategory(CategoryModel categoryModel)
        {
            using (var ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE PRODUCTCATEGORY SET PRODCATEGORYNAME = @PRCNAME where PRODCATEGORYID = @ID";
                        cmd.Parameters.AddWithValue("@ID", categoryModel.categID);
                        cmd.Parameters.AddWithValue("@PRCNAME", categoryModel.CategoryName);

                        var categID = cmd.ExecuteNonQuery();
                        return categID;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }
        }

    }
}