using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ERPSystem.Data

{
    internal class CategoryDAO
    {
        private string connString = ERPSystems.Properties.Resources.ConnectionString;

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
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public List<CategoryModel> CategorySort(string sortedName)
        {
            try
            {
                List<CategoryModel> sortedItem = new List<CategoryModel>();

                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;

                        if (sortedName == "IsActive")
                        {
                            cmd.CommandText = "SELECT * FROM productCategory where isActive = 0 ORDER BY prodcategoryId DESC";
                        }
                        else
                        {
                            cmd.CommandText = "SELECT * FROM productCategory where isActive = 1 ORDER BY prodcategoryId DESC";
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    CategoryModel categories = new CategoryModel();
                                    categories.categID = Convert.ToInt32(reader["ProdcategoryId"]);
                                    categories.CategoryName = reader["ProdcategoryName"].ToString();
                                    categories.categ_IsActive = Convert.ToInt32(reader["IsActive"]);
                                    sortedItem.Add(categories);
                                }
                            }
                        }
                        return sortedItem;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int categActivation(int ID)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE PRODUCTCATEGORY SET ISACTIVE = @state WHERE ProdcategoryId = @id";
                        cmd.Parameters.AddWithValue("@state", 1);
                        cmd.Parameters.AddWithValue("@id", ID);

                        var row = cmd.ExecuteNonQuery();
                        return row;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public bool checkCategory(string Name)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "select * from productCategory where prodcategoryName = @categName";
                        cmd.Parameters.AddWithValue("@categName", Name);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}