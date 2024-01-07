using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace ERPSystem.Data
{
    internal class ProductDAO
    {
        private string connString = @"Data Source=ZAKU-R44S51\SQLEXPRESS;Initial Catalog=erp;Integrated Security=True";

        public List<ProductModel> FetchAll()
        {
            List<ProductModel> productList = new List<ProductModel>();
            using (SqlConnection connectDB = new SqlConnection(connString))
            {
                connectDB.Open();
                using (var cmd = connectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select * from Product where ProdIsActive = 1";
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ProductModel products = new ProductModel();
                                products.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                products.prod_Name = reader["prodName"].ToString();
                                products.prod_Description = reader["prodDescription"].ToString();
                                products.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                products.upDate = reader["ProdUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["ProdUpdatedAt"]) : DateTime.UtcNow;
                                products.prodCategoryID = reader["ProdCategoryId"] != DBNull.Value ? Convert.ToInt32(reader["ProdCategoryId"]) : 0;
                                products.supplierID = reader["SupplierID"] != DBNull.Value ? Convert.ToInt32(reader["ProdCategoryId"]) : 0;
                                products.isActive = Convert.ToInt32(reader["ProdIsActive"]);
                                productList.Add(products);
                            }
                        }
                        return productList;
                    }
                }
            }    
        }

        public List<ViewModel.ProductViewModel> ShowProducts()
        {
            List<ViewModel.ProductViewModel> productList = new List<ViewModel.ProductViewModel>();
            using (SqlConnection connectDB = new SqlConnection(connString))
            {
                connectDB.Open();

                using (var cmd = connectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM Product LEFT JOIN  ProductCategory ON Product.ProdcategoryId = ProductCategory.ProdcategoryId " +
                    "LEFT JOIN Supplier ON Product.SupplierId = Supplier.SuppId WHERE ProdIsActive = 1 ORDER BY PRODID DESC";

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel.ProductViewModel products = new ViewModel.ProductViewModel();
                                products.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                products.prod_Name = reader["prodName"].ToString();
                                products.prod_Description = reader["prodDescription"].ToString();
                                products.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                products.CategoryName = reader["ProdcategoryName"].ToString();
                                products.SuppName = reader["SuppName"].ToString();
                                products.isActive = Convert.ToInt32(reader["ProdIsActive"]);
                                productList.Add(products);
                            }
                        }
                        return productList;
                    }
                }
            }
        }


        public ProductModel FetchOne(int ID)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM PRODUCT WHERE prodId = @id AND PRODISACTIVE = 1";
                    cmd.Parameters.AddWithValue("@id", ID);

                    using (var reader = cmd.ExecuteReader())
                    {
                        ProductModel product = new ProductModel();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                product.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                product.prod_Name = reader["prodName"].ToString();
                                product.prod_Description = reader["prodDescription"].ToString();
                                if (reader["ProdCategoryId"] != DBNull.Value)
                                {
                                    product.prodCategoryID = Convert.ToInt32(reader["ProdCategoryId"]);
                                    product.supplierID = Convert.ToInt32(reader["SupplierID"]);
                                }
                                product.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                product.upDate = Convert.ToDateTime(reader["ProdUpdatedAt"]);               
                                product.isActive = Convert.ToInt32(reader["ProdIsActive"]);
                            }
                        }
                        return product;
                    }
                }
            }
        }

        public int CreateProduct(ProductModel productModel)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {

                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        if (productModel.prodCategoryID == null || productModel.supplierID == null)
                        {
                            cmd.CommandText = "INSERT INTO PRODUCT(ProdName, ProdDescription) values(@pName, @Description)";

                        }
                        else if(productModel.prodCategoryID != null && productModel.supplierID == null)
                        {
                            cmd.CommandText = "INSERT INTO PRODUCT(ProdName, ProdDescription, ProdcategoryId) values(@pName, @Description, @pcatId)";
                            cmd.Parameters.AddWithValue("@pcatId", productModel.prodCategoryID);

                        }
                        else if(productModel.prodCategoryID == null && productModel.supplierID != null)
                        {
                            cmd.Parameters.AddWithValue("@supid", productModel.supplierID);
                        }
                        else
                        {
                            cmd.CommandText = "INSERT INTO PRODUCT(ProdName, ProdDescription, ProdcategoryId, SupplierId) values(@pName, @Description, @pcatId, @supid)";
                            cmd.Parameters.AddWithValue("@pcatId", productModel.prodCategoryID);
                            cmd.Parameters.AddWithValue("@supid", productModel.supplierID);
                        }
                        cmd.Parameters.AddWithValue("@pName", productModel.prod_Name);
                        cmd.Parameters.AddWithValue("@Description", productModel.prod_Description);

                        var productID = cmd.ExecuteNonQuery();
                        if (productID > 0)
                        {
                            return productID;
                        }
                        else
                            return -1;
                        
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception Message: {ex.Message}");
                        return -1;
                    }
                }
               
               
            }
        }

        public bool checkEntry(string Prodname)
        {
            try
            {
                using (SqlConnection ConnectDB = new SqlConnection(connString))
                {
                    ConnectDB.Open();
                    using (var cmd = ConnectDB.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM PRODUCT WHERE ProdName = @pName";
                        cmd.Parameters.AddWithValue("@pName", Prodname);

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

        public int UpdateProduct(ProductModel productModel)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE PRODUCT SET prodName = @pName, prodDescription = @Description, " +
                                "ProdUpdatedAt = @update, ProdCategoryId = @pcatId, SupplierId = @supId  WHERE prodId = @pID";

                        cmd.Parameters.AddWithValue("@pID", productModel.prod_Id);
                        cmd.Parameters.AddWithValue("@pName", productModel.prod_Name);
                        cmd.Parameters.AddWithValue("@Description", productModel.prod_Description);
                        cmd.Parameters.AddWithValue("@update", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@pcatId", productModel.prodCategoryID);
                        cmd.Parameters.AddWithValue("@supId", productModel.supplierID);

                        int row = cmd.ExecuteNonQuery();
                        if (row > 0)
                        {
                            return row;
                        }
                        else 
                        { 
                            return 0; 
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }

            }
        }

        internal int itemDelete(int ID)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE PRODUCT set ProdIsActive = @deactivate where prodid = @ID";

                        cmd.Parameters.AddWithValue("@ID", ID);
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

        internal int productActivation(int ID)
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE PRODUCT set ProdIsActive = @activate where prodid = @ID";

                        cmd.Parameters.AddWithValue("@ID", ID);
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

        internal List<ViewModel.ProductViewModel> SearchKey(string searchPhrase)
        {
            List<ViewModel.ProductViewModel> returnProducts = new List<ViewModel.ProductViewModel>();

            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT ProdId, ProdName, ProdDescription, ProdcategoryName, " +
                    "SuppName, ProdIsActive, ProdCreatedAt FROM Product INNER JOIN  ProductCategory ON Product.ProdcategoryId = ProductCategory.ProdcategoryId " +
                    "INNER JOIN Supplier ON Product.SupplierId = Supplier.SuppId WHERE (PRODID LIKE '%'+@KEY+'%' OR " +
                    "PRODNAME LIKE '%'+@KEY+'%' OR PRODDESCRIPTION LIKE '%'+@KEY+'%' OR  ProdcategoryName LIKE '%'+@KEY+'%' " +
                    "OR SUPPNAME LIKE '%'+@KEY+'%' OR PRODISACTIVE LIKE @KEY OR PRODCREATEDAT LIKE @KEY) AND PRODISACTIVE = 1 ORDER BY PRODID ASC";

                    cmd.Parameters.AddWithValue("@KEY", searchPhrase);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel.ProductViewModel products = new ViewModel.ProductViewModel();
                                products.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                products.prod_Name = reader["prodName"].ToString();
                                products.prod_Description = reader["prodDescription"].ToString();
                                products.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                //products.upDate = Convert.ToDateTime(reader["ProdUpdatedAt"]);
                                products.CategoryName = reader["ProdcategoryName"].ToString();
                                products.SuppName = reader["SuppName"].ToString();
                                products.isActive = Convert.ToInt32(reader["ProdIsActive"]);
                                returnProducts.Add(products);
                            }
                        }
                    }
                    return returnProducts;
                }
            }             
        }

        public List<ViewModel.ProductViewModel> sortProduct(string sortName)
        {
            List<ViewModel.ProductViewModel> productList = new List<ViewModel.ProductViewModel>();
            using (var ConnectDb = new SqlConnection(connString))
            {
                ConnectDb.Open();
                using (var cmd = ConnectDb.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    if(sortName != "ProdIsActive")
                    {
                        cmd.CommandText = "SELECT * FROM Product INNER JOIN  ProductCategory ON Product.ProdcategoryId = ProductCategory.ProdcategoryId " +
                        "INNER JOIN Supplier ON Product.SupplierId = Supplier.SuppId WHERE ProdIsActive = 1 ORDER BY " +
                        $"{sortName} DESC";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT * FROM Product INNER JOIN  ProductCategory ON Product.ProdcategoryId = ProductCategory.ProdcategoryId " +
                        "INNER JOIN Supplier ON Product.SupplierId = Supplier.SuppId WHERE ProdIsActive = 0 ORDER BY " +
                        $"{sortName} DESC";
                    }
               
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                ViewModel.ProductViewModel products = new ViewModel.ProductViewModel();
                                products.prod_Id = Convert.ToInt32(reader["ProdId"]);
                                products.prod_Name = reader["prodName"].ToString();
                                products.prod_Description = reader["prodDescription"].ToString();
                                products.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                products.CategoryName = reader["ProdcategoryName"].ToString();
                                products.SuppName = reader["SuppName"].ToString();
                                products.isActive = Convert.ToInt32(reader["ProdIsActive"]);
                                productList.Add(products);
                            }
                        }

                    }
                    return productList;
                }
            }
        }

        public int CountProduucts()
        {
            using (SqlConnection ConnectDB = new SqlConnection(connString))
            {
                ConnectDB.Open();
                using (var cmd = ConnectDB.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT COUNT(*) FROM PRODUCT WHERE PRODISACTIVE = 1";

                        int countNum = cmd.ExecuteNonQuery();
                        return countNum;
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