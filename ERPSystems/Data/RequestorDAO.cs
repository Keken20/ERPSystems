using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPSystem.Controllers
{
    internal class RequestorDAO
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zak\source\repos\Keken20\ERPSystem\ERPSystem\App_Data\FullDB.mdf;Integrated Security=True";

        private int reqId;

        private int totItem;

        internal int addRequestForm(int id)
        {
            List<RequestForm> rfForm = new List<RequestForm>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO REQUISITIONFORM (AccId) VALUES(@userId)";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@userId", id);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        internal int addRequestItem(RequestItem reqItem, int rID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                getReqId();

                string sqlQuery = "INSERT INTO REQUISITIONITEM VALUES (@reqId, @prodId, @reqQuant, @reqUnit)";

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@reqId", rID);
                command.Parameters.AddWithValue("@prodId", reqItem.ProdId);
                command.Parameters.AddWithValue("@reqQuant", reqItem.Quantity);
                command.Parameters.AddWithValue("@reqUnit", reqItem.Unit);

                totItem += reqItem.Quantity;

                connection.Open();
                int newID = command.ExecuteNonQuery();

                reqUpdateTotalItem(reqId, totItem);

                return newID;
            }
        }

        internal int deleteRequest(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE REQUISITIONFORM SET REQISACTIVE = 0 WHERE REQID = @Id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int deletedID = command.ExecuteNonQuery();

                return deletedID;
            }
        }

        public int reqUpdateTotalItem(int id, int item)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE REQUISITIONFORM SET REQTOTALITEM = @item WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@item", item);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        public int getReqId()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "select top 1 (ReqId) from RequisitionForm order by reqId desc";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                int requestId = 0;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        requestId = reader.GetInt32(0);
                    }
                }
                return requestId;
            }
        }

        public int updateRequestItem(RequestItem item)
        {
            RequestForm requestForm = new RequestForm();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE REQUISITIONITEM SET PRODID = @prodId, REQQUANTITY = @reqQuant, REQUNIT = @reqUnit WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);


                command.Parameters.AddWithValue("@prodId", item.ProdId);
                command.Parameters.AddWithValue("@reqQuant", item.Quantity);
                command.Parameters.AddWithValue("@reqUnit", item.Unit);
                command.Parameters.AddWithValue("@id", item.RequestId);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        public int reqUpdateDate(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE REQUISITIONFORM SET REQUPDATED = @DATE WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);


                command.Parameters.AddWithValue("DATE", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        public RequestItem GetOne(int id)
        {
            //List<Supplier> supplierList = new List<Supplier>();

            RequestItem reqItem = new RequestItem();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM REQUISITIONITEM INNER JOIN PRODUCT ON RequisitionItem.ProdId = Product.ProdId WHERE REQID = @Id ";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        reqItem.RequestId = id;
                        reqItem.ProdId = reader.GetInt32(1);
                        reqItem.Quantity = reader.GetInt32(2);
                        reqItem.ProdName = reader.GetString(5);
                        reqItem.Unit = reader.GetString(3);
                    }
                }
                else
                {
                    reqItem.RequestId = id;
                }
            }
            return reqItem;
        }

        public List<RequestItem> FetchItems(int id)
        {
            //List<Supplier> supplierList = new List<Supplier>();

            List<RequestItem> returnItems = new List<RequestItem>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM REQUISITIONITEM INNER JOIN PRODUCT ON RequisitionItem.ProdId = Product.ProdId WHERE REQID = @Id ";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
        
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RequestItem reqItem = new RequestItem();
                        reqItem.RequestId = Convert.ToInt32(reader["ReqId"]);
                        reqItem.ProdId = Convert.ToInt32(reader["ProdId"]);
                        reqItem.Quantity = Convert.ToInt32(reader["ReqQuantity"]);
                        reqItem.ProdName = reader["ProdName"].ToString();
                        reqItem.Unit = reader["ReqUnit"].ToString();
                        returnItems.Add(reqItem);
                    }
                }
            }
            return returnItems;
        }


        //public List<RequestForm> getNewForm()
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string sqlQuery = "select top 1 (ReqId) from RequisitionForm order by reqId desc";

        //        List<RequestForm> rqForm = new List<RequestForm>();

        //        SqlCommand command = new SqlCommand(sqlQuery, connection);

        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                RequestForm rform = new RequestForm();
        //                rform.RequestId = reader.GetInt32(0);

        //                rqForm.Add(rform);
        //            }
        //        }
        //        return rqForm;
        //    }
        //}
    }
}