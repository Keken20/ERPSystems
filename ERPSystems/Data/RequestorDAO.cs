using ERPSystems.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ERPSystems.Controllers
{
    internal class RequestorDAO
    {
        private string connectionString = ERPSystems.Properties.Resources.ConnectionString;

        internal int addRequestForm(int id) //Adds new requestform 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO dbo.REQUISITIONFORM (AccId) VALUES(@userId)";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@userId", id);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                return newID;
            }
        }

        internal int addRequestItem(RequestItem reqItem, int rID) //Adds new requestitem
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                RequestForm requestForm = new RequestForm();

                string sqlQuery = "INSERT INTO REQUISITIONITEM VALUES (@reqId, @prodId, @reqQuant, @reqUnit)";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@reqId", rID);
                command.Parameters.AddWithValue("@prodId", reqItem.ProdId);
                command.Parameters.AddWithValue("@reqQuant", reqItem.Quantity);
                command.Parameters.AddWithValue("@reqUnit", reqItem.Unit);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                int totitem = getReqTotItem(rID);

                reqUpdateTotalItem(rID, totitem);

                return newID;
            }
        }

        internal int deleteRequest(int id) //Delete requestform
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

        public int reqUpdateTotalItem(int id, int item) //Update Total Item from requestItem to requestForm
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

        public int getReqId() //Get requisition ID of the newly created requisition form
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "select top 1 (ReqId) from RequisitionForm order by reqId desc";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                int id = 0;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32(0);
                    }
                }

                return id;
            }
        }
        public int updateRequestItem(RequestItem item, int id) // Update request item
        {
            RequestForm requestForm = new RequestForm();

            int TotalItem = requestForm.TotalItem;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE REQUISITIONITEM SET PRODID = @prodId, REQQUANTITY = @reqQuant, REQUNIT = @reqUnit WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@prodId", item.ProdId);
                command.Parameters.AddWithValue("@reqQuant", item.Quantity);
                command.Parameters.AddWithValue("@reqUnit", item.Unit);
                command.Parameters.AddWithValue("@id", id);

                int totitem = getReqTotItem(id);

                connection.Open();
                int newID = command.ExecuteNonQuery();

                //if(newID >= 1)
                //{
                reqUpdateTotalItem(id, totitem);
                reqUpdateDate(id);
                //}
                //else
                //{
                //    addRequestItem(item, id);
                //    //reqUpdateTotalItem(id, totitem);
                //}

                return newID;
            }
        }

        public int verify(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT * FROM REQUISITIONITEM INNER JOIN PRODUCT ON RequisitionItem.ProdId = Product.ProdId WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@id", id);

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

        //public int verify(RequestItem requestItem, int id)

        public int getReqTotItem(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT COUNT(*) FROM REQUISITIONITEM WHERE REQID = @id";

                SqlCommand command = new SqlCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@id", id);

                connection.Open();

                int count = 0;

                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
                return count;
            }
        }

        public List<RequestItem> GetOne(int id)
        {
            List<RequestItem> reqList = new List<RequestItem>();

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
                        reqItem.RequestId = id;
                        reqItem.ProdId = reader.GetInt32(1);
                        reqItem.Quantity = reader.GetInt32(2);
                        reqItem.ProdName = reader.GetString(5);
                        reqItem.Unit = reader.GetString(3);
                        reqList.Add(reqItem);
                    }
                }
                else //Get only id if requestform has empty reqitems
                {
                    RequestItem reqItem = new RequestItem();
                    reqItem.RequestId = id;
                    reqList.Add(reqItem);
                }
            }
            return reqList;
        }
    }
}