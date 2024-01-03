using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPSystem.Data
{
    internal class SupplierDAO
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\Source\Repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security = True";

        public List<Supplier> GetAll()
        {
            List<Supplier> supplierList = new List<Supplier>();

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM Supplier WHERE SUPPISACTIVE = 1";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        Supplier supplier = new Supplier();
                        supplier.SuppID = reader.GetInt32(0);
                        supplier.SuppName = reader.GetString(1);
                        //supplier.SuppContact = reader.GetString(2);
                        //supplier.SuppCity = reader.GetString(3);
                        //supplier.SuppMunicipality = reader.GetString(4);
                        //supplier.SuppBarangay = reader.GetString(5);
                        //supplier.SuppZipcode = reader.GetString(6);
                        //supplier.SuppCreatedAt = reader.GetDateTime(7);
                        try
                        {
                            supplier.SuppUpdatedAt = reader.GetDateTime(8);
                        }
                        catch
                        {
                            supplier.SuppUpdatedAt = DateTime.MinValue;
                        }
                        supplier.SuppIsActive = reader.GetInt32(9);

                        supplierList.Add(supplier);
                    }
                }
            }

            return supplierList;
        }

        public Supplier GetOne(int id)
        {
            //List<Supplier> supplierList = new List<Supplier>();

            Supplier supplier = new Supplier();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Supplier where SUPPID = @Id AND SUPPISACTIVE = 1";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        supplier.SuppID = reader.GetInt32(0);
                        supplier.SuppName = reader.GetString(1);
                        supplier.SuppContact = reader.GetString(2);
                        supplier.SuppCity = reader.GetString(3);
                        supplier.SuppMunicipality = reader.GetString(4);
                        supplier.SuppBarangay = reader.GetString(5);
                        supplier.SuppZipcode = reader.GetString(6);
                        supplier.SuppCreatedAt = reader.GetDateTime(7);
                        try
                        {
                            supplier.SuppUpdatedAt = reader.GetDateTime(8);
                        }
                        catch
                        {
                            supplier.SuppUpdatedAt = DateTime.MinValue;
                        }
                        supplier.SuppIsActive = reader.GetInt32(9);
                    }
                }
            }

            return supplier;
        }

        internal List<Supplier> SearchName(string searchId)
        {
            List<Supplier> supplierList = new List<Supplier>();

            Supplier supplier = new Supplier();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM dbo.Supplier where SUPPNAME LIKE '%' +@searchPhrase+ '%' AND SUPPISACTIVE = 1";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@searchPhrase", searchId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        supplier.SuppID = reader.GetInt32(0);
                        supplier.SuppName = reader.GetString(1);
                        supplier.SuppContact = reader.GetString(2);
                        supplier.SuppCity = reader.GetString(3);
                        supplier.SuppMunicipality = reader.GetString(4);
                        supplier.SuppBarangay = reader.GetString(5);
                        supplier.SuppZipcode = reader.GetString(6);
                        supplier.SuppCreatedAt = reader.GetDateTime(7);
                        try
                        {
                            supplier.SuppUpdatedAt = reader.GetDateTime(8);
                        }
                        catch
                        {
                            supplier.SuppUpdatedAt = DateTime.MinValue;
                        }
                        supplier.SuppIsActive = reader.GetInt32(9);

                        supplierList.Add(supplier);
                    }
                }
            }

            return supplierList;
        }

        public int add(Supplier supplier)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO dbo.Supplier (SUPPNAME, SUPPPHONE, SUPPCITY, SUPPMUNICIPALITY, SUPPBARANGAY, SUPPZIPCODE, SUPPCREATEDAT,SUPPISACTIVE) VALUES(@Name, @Contact, @City, @Municipality, @Barangay, @Zipcode, @CreatedAt, @IsActive)";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Name", supplier.SuppName);
                command.Parameters.AddWithValue("@Contact", supplier.SuppContact);
                command.Parameters.AddWithValue("@City", supplier.SuppCity);
                command.Parameters.AddWithValue("@Municipality", supplier.SuppMunicipality);
                command.Parameters.AddWithValue("@Barangay", supplier.SuppBarangay);
                command.Parameters.AddWithValue("@Zipcode", supplier.SuppZipcode);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                //command.Parameters.AddWithValue("@UpdatedAt", supplier.SuppUpdatedAt);
                command.Parameters.AddWithValue("@IsActive", supplier.SuppIsActive);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        internal int delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE dbo.Supplier SET SUPPISACTIVE = 0 WHERE SUPPID = @Id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", id);
                
                connection.Open();
                int deletedID = command.ExecuteNonQuery();

                return deletedID;
            }
        }

        public int update(Supplier supplier)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE dbo.Supplier SET SUPPNAME = @Name, SUPPPHONE = @Contact, SUPPCITY = @City, SUPPMUNICIPALITY = @Municipality, SUPPBARANGAY = @Barangay, SUPPZIPCODE = @Zipcode, SUPPUPDATEDAT = @UpdatedAt WHERE SUPPID = @Id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", supplier.SuppID);
                command.Parameters.AddWithValue("@Name", supplier.SuppName);
                command.Parameters.AddWithValue("@Contact", supplier.SuppContact);
                command.Parameters.AddWithValue("@City", supplier.SuppCity);
                command.Parameters.AddWithValue("@Municipality", supplier.SuppMunicipality);
                command.Parameters.AddWithValue("@Barangay", supplier.SuppBarangay);
                command.Parameters.AddWithValue("@Zipcode", supplier.SuppZipcode);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }
    }
}