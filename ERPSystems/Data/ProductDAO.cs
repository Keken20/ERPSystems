using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace ERPSystem.Data
{
    internal class ProductDAO
    {
        private string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\source\repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security=True";
        
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
                    cmd.CommandText = "SELECT ProdId, ProdName, ProdDescription, ProductCategory.ProdcategoryName, " +
                    "SuppName, ProdIsActive, ProdCreatedAt FROM Product INNER JOIN  ProductCategory ON Product.ProdcategoryId = ProductCategory.ProdcategoryId " +
                    "INNER JOIN Supplier ON Product.SupplierId = Supplier.SuppId WHERE ProdIsActive = 1 ORDER BY PRODID ASC";

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
                    cmd.CommandText = "SELECT * FROM PRODUCT WHERE prodId = @id WHERE PRODISACTIVE = 1";
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
                                product.inDate = Convert.ToDateTime(reader["ProdCreatedAt"]);
                                product.upDate = Convert.ToDateTime(reader["ProdUpdatedAt"]);
                                product.prodCategoryID = Convert.ToInt32(reader["ProdCategoryId"]);
                                product.supplierID = Convert.ToInt32(reader["SupplierID"]);
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
                        cmd.CommandText = "INSERT INTO PRODUCT(ProdName, ProdDescription, ProdCreatedAt,ProdIsActive, ProdcategoryId, SupplierId) " +
                                "values(@pName, @Description, @indate, @active, @pcatId, @supId)";
                        cmd.Parameters.AddWithValue("@pID", productModel.prod_Id);
                        cmd.Parameters.AddWithValue("@pName", productModel.prod_Name);
                        cmd.Parameters.AddWithValue("@Description", productModel.prod_Description);
                        cmd.Parameters.AddWithValue("@indate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@update", DateTime.Now);
                        cmd.Parameters.AddWithValue("@pcatId", productModel.prodCategoryID);
                        cmd.Parameters.AddWithValue("@supId", productModel.supplierID);
                        cmd.Parameters.AddWithValue("@active", 1);

                        var productID = cmd.ExecuteNonQuery();
                        return productID;
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
               
               
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

                        int productID = cmd.ExecuteNonQuery();
                        return productID;
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
    }


}