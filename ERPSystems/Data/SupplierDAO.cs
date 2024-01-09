using ERPSystems.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPSystems.Data
{
    internal class SupplierDAO
    {
        private string connectionString = ERPSystems.Properties.Resources.ConnectionString;

        public List<Supplier> GetAllActive()
        {
            List<Supplier> supplierList = new List<Supplier>();

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string sqlQuery = "SELECT * FROM dbo.Supplier WHERE SUPPISACTIVE = 1 order by SuppId desc";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Supplier supplier = new Supplier();
                            supplier.SuppID = reader.GetInt32(0);
                            supplier.SuppName = reader.GetString(1);
                            supplier.SuppContact = reader.GetString(2);
                            supplier.SuppCity = reader.GetString(3);
                            supplier.SuppMunicipality = reader.GetString(4);
                            supplier.SuppBarangay = reader.GetString(5);
                            supplier.SuppZipcode = reader.GetString(6);
                            supplier.SuppCreatedAt = reader.GetDateTime(7);
                            supplier.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplier.SuppIsActive = reader.GetInt32(9);

                            supplierList.Add(supplier);
                        }
                    }
                }
                catch(Exception excep)
                {
                    throw excep;
                }
            }

            return supplierList;
        }

        public List<Supplier> GetAll()
        {
            try
            {
                List<Supplier> supplierList = new List<Supplier>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM dbo.Supplier order by SuppId desc";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Supplier supplier = new Supplier();
                            supplier.SuppID = reader.GetInt32(0);
                            supplier.SuppName = reader.GetString(1);
                            supplier.SuppContact = reader.GetString(2);
                            supplier.SuppCity = reader.GetString(3);
                            supplier.SuppMunicipality = reader.GetString(4);
                            supplier.SuppBarangay = reader.GetString(5);
                            supplier.SuppZipcode = reader.GetString(6);
                            supplier.SuppCreatedAt = reader.GetDateTime(7);
                            supplier.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplier.SuppIsActive = reader.GetInt32(9);

                            supplierList.Add(supplier);
                        }
                    }
                }

                return supplierList;
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }

        public Supplier GetOne(int id)
        {
            try
            {
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
                            supplier.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplier.SuppIsActive = reader.GetInt32(9);
                        }
                    }
                }

                return supplier;
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }

        internal List<Supplier> SortBy(string sortId)
        {
            try
            {
                List<Supplier> supplierList = new List<Supplier>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = $"SELECT * FROM dbo.Supplier where SUPPISACTIVE = 1 order by {sortId}";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Supplier supplier = new Supplier();

                            supplier.SuppID = reader.GetInt32(0);
                            supplier.SuppName = reader.GetString(1);
                            supplier.SuppContact = reader.GetString(2);
                            supplier.SuppCity = reader.GetString(3);
                            supplier.SuppMunicipality = reader.GetString(4);
                            supplier.SuppBarangay = reader.GetString(5);
                            supplier.SuppZipcode = reader.GetString(6);
                            supplier.SuppCreatedAt = reader.GetDateTime(7);
                            supplier.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplier.SuppIsActive = reader.GetInt32(9);

                            supplierList.Add(supplier);
                        }
                    }
                }

                return supplierList;
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }

        internal List<Supplier> SearchName(string searchId)
        {
            try
            {
                List<Supplier> supplierList = new List<Supplier>();

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
                            Supplier supplier = new Supplier();

                            supplier.SuppID = reader.GetInt32(0);
                            supplier.SuppName = reader.GetString(1);
                            supplier.SuppContact = reader.GetString(2);
                            supplier.SuppCity = reader.GetString(3);
                            supplier.SuppMunicipality = reader.GetString(4);
                            supplier.SuppBarangay = reader.GetString(5);
                            supplier.SuppZipcode = reader.GetString(6);
                            supplier.SuppCreatedAt = reader.GetDateTime(7);
                            supplier.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplier.SuppIsActive = reader.GetInt32(9);

                            supplierList.Add(supplier);
                        }
                    }
                }

                return supplierList;
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }

        public int add(Supplier supplier)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "INSERT INTO dbo.Supplier (SUPPNAME, SUPPPHONE, SUPPCITY, SUPPMUNICIPALITY, SUPPBARANGAY, SUPPZIPCODE, SUPPUPDATEDAT) VALUES(@Name, @Contact, @City, @Municipality, @Barangay, @Zipcode, @UpdatedAt)";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    command.Parameters.AddWithValue("@Name", supplier.SuppName);
                    command.Parameters.AddWithValue("@Contact", supplier.SuppContact);
                    command.Parameters.AddWithValue("@City", supplier.SuppCity);
                    command.Parameters.AddWithValue("@Municipality", supplier.SuppMunicipality);
                    command.Parameters.AddWithValue("@Barangay", supplier.SuppBarangay);
                    command.Parameters.AddWithValue("@Zipcode", supplier.SuppZipcode);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                    connection.Open();
                    int newID = command.ExecuteNonQuery();

                    return newID;
                }
            }
            catch (Exception excep)
            {
                throw excep;
            }
        }

        internal int delete(int id)
        {
            try
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
            catch(Exception excep)
            {
                throw excep;
            }
        }

        internal int reactivate(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "UPDATE Supplier SET SUPPISACTIVE = 1 WHERE SUPPID = @Id";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    int updatedID = command.ExecuteNonQuery();

                    return updatedID;
                }
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }

        public int update(Supplier supplier)
        {
            try
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
            catch(Exception excep)
            {
                throw excep;
            }
        }

        public Supplier getPhone(Supplier supplier)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "SELECT * FROM SUPPLIER WHERE SUPPPHONE = @Contact";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);

                    command.Parameters.AddWithValue("@Contact", supplier.SuppContact);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    Supplier supplist = new Supplier();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            supplist.SuppID = reader.GetInt32(0);
                            supplist.SuppName = reader.GetString(1);
                            supplist.SuppContact = reader.GetString(2);
                            supplist.SuppCity = reader.GetString(3);
                            supplist.SuppMunicipality = reader.GetString(4);
                            supplist.SuppBarangay = reader.GetString(5);
                            supplist.SuppZipcode = reader.GetString(6);
                            supplist.SuppCreatedAt = reader.GetDateTime(7);
                            supplist.SuppUpdatedAt = reader["SuppUpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["SuppUpdatedAt"]) : DateTime.Now;
                            supplist.SuppIsActive = reader.GetInt32(9);
                        }
                    }
                    return supplist;
                }
            }
            catch(Exception excep)
            {
                throw excep;
            }
        }
    }
}