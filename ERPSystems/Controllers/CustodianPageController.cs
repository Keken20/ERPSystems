using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using ERPSystems.Data;
using System.Data;

namespace ERPSystems.Controllers
{
    public class CustodianPageController : Controller
    {
        // GET: CustodianPage
        int purid;
        int recordAffected;
        int prodid;
        int quoteid;
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<Account> userAccounts = new List<Account>();
        List<RequestForm> requestForm = new List<RequestForm>();
        List<QuoteForm> quoteForms = new List<QuoteForm>();
        List<QuoteItem> quoteItems = new List<QuoteItem>();
        List<QuoteFormItem> quoteformitems = new List<QuoteFormItem>();
        List<QuotePrice> quotePrices = new List<QuotePrice>();

        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        public ActionResult CustodianDashboard()
        {
            return View();
        }
        public ActionResult CustodianPurchaseOrder(RequestModel model)
        {
            FetchRequest();
            List<RequestForm> requestForms = new List<RequestForm>();
            model.requestform = requestForm;
            return View(model);
        }
        public ActionResult FetchRequest()
        {
            connnectionString();
            if (userAccounts.Count > 0)
            {
                userAccounts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "select * From RequisitionForm inner join  Account on RequisitionForm.AccId = Account.AccId";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    requestForm.Add(new RequestForm
                    {
                        RequestId = int.Parse(dr["ReqId"].ToString())
                    ,
                        RequestDate = DateTime.Parse(dr["ReqCreatedAt"].ToString())
                    ,
                        TotalItem = int.Parse(dr["ReqTotalItem"].ToString())
                    ,
                        RequestorName = dr["AccFname"] + " " + dr["AccLname"].ToString()
                    ,
                        Status = dr["ReqStatus"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(requestForm);
        }
        public ActionResult ReceivedRequestItem(int id)
        {
            List<RequestItem> reqitem = FetchRequestItem(id);
            RequestModel requestmodel = new RequestModel();
            {
                requestmodel.requestItems = reqitem;
            };
            return Json(requestmodel);
        }
        [HttpPost]
        private List<RequestItem> FetchRequestItem(int id)
        {
            try
            {
                int reqid = id;
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from RequisitionItem inner join Product on Product.ProdId = RequisitionItem.ProdId where ReqId = '" + reqid + "'";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    requestItem.Add(new RequestItem
                    {
                        ProdId = int.Parse(dr["ProdId"].ToString())
                    ,
                        ProdName = dr["ProdName"].ToString()
                    ,
                        Description = dr["ProdDescription"].ToString()
                    ,
                        Unit = dr["ReqUnit"].ToString()
                    ,
                        Quantity = int.Parse(dr["ReqQuantity"].ToString())
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (requestItem);
        }
        public ActionResult CustodianQuotation(QuoteModel model)
        {
            model.quoteForms = FetchQuotationData();
            return View(model);
        }
        [HttpPost]
        public List<QuoteForm> FetchQuotationData()
        {
            List<QuoteForm> quoteForms = new List<QuoteForm>();

            try
            {
                connnectionString();

                using (SqlConnection connection = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("select * from PurchaseOrderForm", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quoteForms.Add(new QuoteForm
                                {
                                    PurID = int.Parse(reader["PurId"].ToString()),
                                    PurDate = DateTime.Parse(reader["PurCreateAt"].ToString()),
                                    Status = reader["PurStatus"].ToString(),
                                    //IsDelete = int.Parse(reader["PurIsDelete"].ToString()),
                                });
                            }
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return quoteForms;
        }
        public ActionResult ReceivedQuoteItem(int id)
        {
            List<QuoteItem> quoteItems = FetchQuoteItem(id);
            QuoteModel quoteModel = new QuoteModel();
            quoteModel.quoteItems = quoteItems;
            return Json(quoteModel);
        }

        private List<QuoteItem> FetchQuoteItem(int id)
        {
            List<QuoteItem> quoteItems = new List<QuoteItem>();

            try
            {
                int purid = id;

                connnectionString();

                using (SqlConnection connection = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT PurId, Product.ProdId, ProdName, ProdDescription, ProdQuantityReceived, PurUnit FROM PurchaseOrderItem INNER JOIN Product ON PurchaseOrderItem.ProdId = Product.ProdId WHERE PurId = @purid", connection))
                    {
                        command.Parameters.AddWithValue("@purid", purid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quoteItems.Add(new QuoteItem
                                {
                                    PurId = int.Parse(reader["PurId"].ToString()),
                                    ProdId = int.Parse(reader["ProdId"].ToString()),
                                    ProdName = reader["ProdName"].ToString(),
                                    Description = reader["ProdDescription"].ToString(),
                                    Quantity = int.Parse(reader["ProdQuantityReceived"].ToString()),
                                    Unit = reader["PurUnit"].ToString(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately.
                Console.WriteLine(ex.Message);
                // Rethrow the exception if needed.
                throw ex;
            }

            return quoteItems;
        }

        ////public ActionResult CreateQuotationForm(int id)
        ////{
        ////    try
        ////    {
        ////        if (!QuotationCheck(id))
        ////        {
        ////            connnectionString();
        ////            con.Open();
        ////            com.Connection = con;
        ////            com.CommandText = "Insert into QoutationForm(PurId)values(@purid)";
        ////            com.Parameters.AddWithValue("@purid", id);
        ////            recordAffected = com.ExecuteNonQuery();
        ////            con.Close();
        ////            if (recordAffected > 0)
        ////            {
        ////                return Json(new { success = true, message = "Quotation form created successfully" });
        ////            }
        ////            else
        ////            {
        ////                recordAffected = 0;
        ////                return Json(new { success = false, message = "Failed to create Quotation form" });
        ////            }
        ////        }
        ////        else
        ////        {
        ////            return Json(new { success = false, message = "Quotation is already approved" });
        ////        }

        ////    }
        ////    catch (Exception e)
        ////    {
        ////        Response.Write(e.Message);
        ////        return Json(new { success = false, message = "An error occurred" });
        ////    }
        ////    finally
        ////    {
        ////        if (con.State == ConnectionState.Open)
        ////        {
        ////            con.Close();
        ////        }
        ////    }
        ////}
        ////public bool QuotationCheck(int id)
        ////{
        ////    connnectionString();
        ////    con.Open();
        ////    com.Connection = con;
        ////    com.CommandText = "Select * from QoutationForm where PurId = @purid";
        ////    com.Parameters.AddWithValue("@purid", id);
        ////    dr = com.ExecuteReader();
        ////    while (dr.Read())
        ////    {
        ////        purid = int.Parse(dr["PurId"].ToString());
        ////    }
        ////    con.Close();
        ////    if (id == purid)
        ////    {
        ////        return true;
        ////    }
        ////    else
        ////    {
        ////        return false;
        ////    }
        ////}
        [HttpPost]
        public ActionResult SaveQuotationForm(List<QuotePrice> tableData, string purId, string suppname, string suppzipcode, string suppbarangay, string suppcity, string suppmunicipality, string suppphone)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;

                // Check if the quotation form exists
                com.CommandText = "SELECT PurId FROM PurchaseOrderForm WHERE PurId = @id";
                com.Parameters.AddWithValue("@id", int.Parse(purId));
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    purid = int.Parse(dr["PurId"].ToString());
                }

                dr.Close();

                com.Parameters.Clear();
                com.CommandText = "INSERT INTO QoutationForm (PurId, SuppName, SuppZipCode, SuppBarangay, SuppCity, SuppMunicipality, SuppPhone) VALUES (@id, @suppname, @suppzipcode, @suppbarangay, @suppcity, @suppmunicipality, @suppphone)";
                com.Parameters.AddWithValue("@id", purid);
                com.Parameters.AddWithValue("@suppname", suppname);
                com.Parameters.AddWithValue("@suppzipcode", suppzipcode);
                com.Parameters.AddWithValue("@suppbarangay", suppbarangay);
                com.Parameters.AddWithValue("@suppcity", suppcity);
                com.Parameters.AddWithValue("@suppmunicipality", suppmunicipality);
                com.Parameters.AddWithValue("@suppphone", suppphone);

                recordAffected = com.ExecuteNonQuery();

                if (recordAffected <= 0)
                {
                    return Json(new { success = false, message = "Failed to create Quotation form" });
                }

                // Retrieve the newly created QouteId
                com.Parameters.Clear();
                com.CommandText = "SELECT QouteId FROM QoutationForm WHERE SuppName = @suppname";
                com.Parameters.AddWithValue("@suppname", suppname);
                dr = com.ExecuteReader();

                while (dr.Read())
                {
                    quoteid = int.Parse(dr["QouteId"].ToString());
                }

                dr.Close();

                // Save QuotationItem data
                com.Parameters.Clear();
                com.CommandText = "INSERT INTO QoutationItem (QouteId, ProdId, QouteQuantity, QouteUnit, QoutePricePerUnit) VALUES (@quoteid, @prodid, @quotequantity, @quoteunit, @quoteunitprice)";

                foreach (var row in tableData)
                {
                    com.Parameters.Clear();
                    prodid = int.Parse(row.ProdId);
                    com.Parameters.AddWithValue("@quoteid", quoteid);
                    com.Parameters.AddWithValue("@prodid", row.ProdId);
                    com.Parameters.AddWithValue("@quotequantity", row.QuoteQuantity);
                    com.Parameters.AddWithValue("@quoteunit", row.QuoteUnit);
                    com.Parameters.AddWithValue("@quoteunitprice", row.UnitPrice);
                    recordAffected = com.ExecuteNonQuery();

                    if (recordAffected <= 0)
                    {
                        return Json(new { success = false, message = "Failed to insert data into QuotationItem" });
                    }
                }

                con.Close();

                return Json(new { success = true, message = "Data saved successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }
        }

        public ActionResult CustodianInventory()
        {
            List<Inventory> inventory = new List<Inventory>();

            InventoryDAO inventoryDAO = new InventoryDAO();

            inventory = inventoryDAO.FetchAll();

            return View(inventory);
        }
        public ActionResult FetchInventory()
        {
            return View();
        }
        public ActionResult Account()
        {
            FetchAccountData();
            return View(userAccounts);
        }
        private void FetchAccountData()
        {
            connnectionString();
            if (userAccounts.Count > 0)
            {
                userAccounts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM Account";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    userAccounts.Add(new Account
                    {
                        UserID = int.Parse(dr["AccId"].ToString())
                    ,
                        UserFullName = dr["AccLname"] + ", " + dr["AccFname"] + " " + dr["AccUserName"].ToString()
                    ,
                        UserName = dr["AccUserName"].ToString()
                    ,
                        UserPassword = dr["AccPassword"].ToString()
                    ,
                        UserType = dr["AccType"].ToString()
                    });
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult Logout()
        {
            return RedirectToAction("Login", "Home");
        }
    }
}