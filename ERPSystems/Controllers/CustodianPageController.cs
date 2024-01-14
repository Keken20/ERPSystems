using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using System.Data;
using ERPSystem.Models;
using ERPSystem.Data;

namespace ERPSystems.Controllers
{
    public class CustodianPageController : Controller
    {
        // GET: CustodianPage
        int recordAffected;
        int prodid;
        int quoteid;
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<Account> userAccounts = new List<Account>();
        List<RequestForm> requestForm = new List<RequestForm>();
        List<Inventory> inventory = new List<Inventory>();
        List<QuoteForm> quoteForms = new List<QuoteForm>();
        List<QuoteItem> quoteItems = new List<QuoteItem>();
        List<QuoteFormItem> quoteformitems = new List<QuoteFormItem>();
        List<QuotePrice> tableData = new List<QuotePrice>();

        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        public ActionResult CustodianQuotation(QuoteModel model)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
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
                    using (SqlCommand command = new SqlCommand("select * from PurchaseOrderForm where PurIsDelete = 0", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quoteForms.Add(new QuoteForm
                                {
                                    PurID = int.Parse(reader["PurId"].ToString()),
                                    PurDate = DateTime.Parse(reader["PurCreatedAt"].ToString()),
                                    TotalItem = int.Parse(reader["PurTotalItem"].ToString()),
                                    Status = reader["PurStatus"].ToString(),
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

                    using (SqlCommand command = new SqlCommand("SELECT PurId, Product.ProdId, ProdName, ProdDescription, PurQuantity, PurUnit FROM PurchaseOrderItem INNER JOIN Product ON PurchaseOrderItem.ProdId = Product.ProdId WHERE PurId = @purid", connection))
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
                                    Quantity = int.Parse(reader["PurQuantity"].ToString()),
                                    Unit = reader["PurUnit"].ToString(),
                                });
                            }
                        }
                        Console.WriteLine($"Number of items retrieved: {quoteItems.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return quoteItems;
        }
        [HttpPost]
        public ActionResult CreateQuotationForm(int purid, QuoteFormItem supplierInfo)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;

                // Check if QoutationForm with the same supplier name already exists
                int existingQuoteId;
                using (SqlCommand checkFormExistenceCommand = new SqlCommand())
                {
                    checkFormExistenceCommand.Connection = con;
                    checkFormExistenceCommand.CommandText = "SELECT QouteId FROM QoutationForm WHERE SuppName = @suppname";
                    checkFormExistenceCommand.Parameters.AddWithValue("@suppname", supplierInfo.SupplierName);

                    existingQuoteId = (int?)checkFormExistenceCommand.ExecuteScalar() ?? 0;

                    if (existingQuoteId == 0)
                    {
                        // If the QoutationForm with the same supplier name does not exist, create a new one
                        com.Parameters.Clear();
                        com.CommandText = "INSERT INTO QoutationForm (PurId, SuppName, SuppZipCode, SuppBarangay, SuppCity, SuppMunicipality, SuppPhone, QouteSubtotal, QouteDiscount, QouteTotal) " +
                                          "VALUES (@id, @suppname, @suppzipcode, @suppbarangay, @suppcity, @suppmunicipality, @suppphone, @quotesub, @quotediscount, @quotetotal); SELECT SCOPE_IDENTITY();";

                        com.Parameters.AddWithValue("@id", purid);
                        com.Parameters.AddWithValue("@suppname", supplierInfo.SupplierName);
                        com.Parameters.AddWithValue("@suppzipcode", supplierInfo.SupplierZipcode);
                        com.Parameters.AddWithValue("@suppbarangay", supplierInfo.SupplierBarangay);
                        com.Parameters.AddWithValue("@suppcity", supplierInfo.SupplierCity);
                        com.Parameters.AddWithValue("@suppmunicipality", supplierInfo.SupplierMunicipality);
                        com.Parameters.AddWithValue("@suppphone", supplierInfo.SupplierPhone);
                        com.Parameters.AddWithValue("@quotesub", supplierInfo.Subtotal);
                        com.Parameters.AddWithValue("@quotediscount", supplierInfo.Discount);
                        com.Parameters.AddWithValue("@quotetotal", supplierInfo.TotalPrice);

                        existingQuoteId = Convert.ToInt32(com.ExecuteScalar());
                    }
                    else
                    {
                        // If a QoutationForm already exists for the supplier, return an error
                        return Json(new { success = false, message = "Quotation Form already exists for this supplier" });
                    }
                }

                // Check if QoutationItem with the same quoteid already exists
                using (SqlCommand checkExistenceCommand = new SqlCommand())
                {
                    checkExistenceCommand.Connection = con;
                    checkExistenceCommand.CommandText = "SELECT COUNT(*) FROM QoutationItem WHERE QouteId = @quoteid";
                    checkExistenceCommand.Parameters.AddWithValue("@quoteid", existingQuoteId);

                    int existingItemCount = (int)checkExistenceCommand.ExecuteScalar();

                    if (existingItemCount > 0)
                    {
                        // QoutationItem with the same quoteid already exists
                        return Json(new { success = false, message = "Quotation Item already saved" });
                    }
                }

                // Proceed with the creation of QoutationItem or any other necessary actions

                return Json(new { success = true, message = "Quotation form created successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Json(new { success = false, message = "An error occurred: " + e.Message });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        [HttpPost]
        public ActionResult SaveQuotationForm(List<QuotePrice> tableData, string suppname)
        {
            try
            {
                connnectionString();

                using (SqlConnection con = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand com = new SqlCommand())
                    {
                        com.Connection = con;
                        com.CommandText = "SELECT * FROM QoutationForm WHERE SuppName = @suppname";
                        com.Parameters.AddWithValue("@suppname", suppname);

                        using (SqlDataReader dr = com.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                quoteid = int.Parse(dr["QouteId"].ToString());
                            }
                            dr.Close();
                        }
                    }

                    // Check if QoutationItem with the same quoteid already exists
                    using (SqlCommand checkExistenceCommand = new SqlCommand())
                    {
                        checkExistenceCommand.Connection = con;
                        checkExistenceCommand.CommandText = "SELECT COUNT(*) FROM QoutationItem WHERE QouteId = @quoteid";
                        checkExistenceCommand.Parameters.AddWithValue("@quoteid", quoteid);

                        int existingItemCount = (int)checkExistenceCommand.ExecuteScalar();

                        if (existingItemCount > 0)
                        {
                            // QoutationItem with the same quoteid already exists
                            return Json(new { success = false, message = "Quotation Item already saved" });
                        }
                    }

                    // Insert QoutationItem
                    using (SqlCommand insertItemCommand = new SqlCommand())
                    {
                        insertItemCommand.Connection = con;
                        insertItemCommand.CommandText = "INSERT INTO QoutationItem (QouteId, ProdId, QouteQuantity, QouteUnit, QoutePricePerUnit) VALUES (@quoteid, @prodid, @quotequantity, @quoteunit, @quoteunitprice)";

                        foreach (var row in tableData)
                        {
                            Console.WriteLine($"ProdId: {row.ProdId}, QouteQuantity: {row.QuoteQuantity}, QouteUnit: {row.QuoteUnit}, QoutePricePerUnit: {row.UnitPrice}");

                            prodid = int.Parse(row.ProdId);

                            insertItemCommand.Parameters.Clear();

                            insertItemCommand.Parameters.AddWithValue("@quoteid", quoteid);
                            insertItemCommand.Parameters.AddWithValue("@prodid", prodid);
                            insertItemCommand.Parameters.AddWithValue("@quotequantity", row.QuoteQuantity);
                            insertItemCommand.Parameters.AddWithValue("@quoteunit", row.QuoteUnit);
                            insertItemCommand.Parameters.AddWithValue("@quoteunitprice", row.UnitPrice);

                            int recordAffected = insertItemCommand.ExecuteNonQuery();

                            if (recordAffected <= 0)
                            {
                                return Json(new { success = false, message = "Failed to insert data into QuotationItem" });
                            }
                        }
                    }

                    return Json(new { success = true, message = "Data saved successfully" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "An error occurred while processing the request" });
            }
        }
        public ActionResult CustodianPurchaseOrder(QuoteModel model)
        {
            if (Session["userId"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            model.quoteForms = FetchPO();
            return View(model);
        }
        [HttpPost]
        public List<QuoteForm> FetchPO()
        {
            List<QuoteForm> quoteForms = new List<QuoteForm>();

            try
            {
                connnectionString();

                using (SqlConnection connection = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("select * from PurchaseOrderForm inner join QoutationForm on QoutationForm.PurId = PurchaseOrderForm.PurId where QouteStatus = 'Approved' and PurIsDelete = 0", connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quoteForms.Add(new QuoteForm
                                {
                                    QuoteID = int.Parse(reader["QouteId"].ToString()),
                                    PurID = int.Parse(reader["PurId"].ToString()),
                                    PurDate = DateTime.Parse(reader["PurCreatedAt"].ToString()),
                                    TotalItem = int.Parse(reader["PurTotalItem"].ToString()),
                                    Status = reader["PurStatus"].ToString(),
                                    SupplierName = reader["SuppName"].ToString(),
                                    SupplierZipcode = reader["SuppZipcode"].ToString(),
                                    SupplierBarangay = reader["SuppBarangay"].ToString(),
                                    SupplierCity = reader["SuppCity"].ToString(),
                                    SupplierMunicipality = reader["SuppMunicipality"].ToString(),
                                    SupplierPhone = reader["SuppPhone"].ToString(),
                                    Discount = decimal.Parse(reader["QouteDiscount"].ToString()),
                                    Subtotal = decimal.Parse(reader["QouteSubtotal"].ToString()),
                                    Total = decimal.Parse(reader["QouteTotal"].ToString())
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
        public ActionResult ReceivedPOItem(int id)
        {
            List<QuoteItem> quoteItems = FetchPOItem(id);
            QuoteModel quoteModel = new QuoteModel();
            quoteModel.quoteItems = quoteItems;
            return Json(quoteModel);
        }

        private List<QuoteItem> FetchPOItem(int id)
        {
            List<QuoteItem> quoteItems = new List<QuoteItem>();

            try
            {
                int quoteid = id;

                connnectionString();

                using (SqlConnection connection = new SqlConnection(Properties.Resources.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SELECT QouteId, Product.ProdId, ProdName, ProdDescription, QouteQuantity, QouteUnit, QoutePricePerUnit FROM QoutationItem INNER JOIN Product ON QoutationItem.ProdId = Product.ProdId WHERE QouteId = @quoteid", connection))
                    {
                        command.Parameters.AddWithValue("@quoteid", quoteid);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quoteItems.Add(new QuoteItem
                                {
                                    QuoteId = int.Parse(reader["QouteId"].ToString()),
                                    ProdId = int.Parse(reader["ProdId"].ToString()),
                                    ProdName = reader["ProdName"].ToString(),
                                    Description = reader["ProdDescription"].ToString(),
                                    Unit = reader["QouteUnit"].ToString(),
                                    UnitPrice = decimal.Parse(reader["QoutePricePerUnit"].ToString()),
                                    Quantity = int.Parse(reader["QouteQuantity"].ToString()),
                                });
                            }
                        }
                        Console.WriteLine($"Number of items retrieved: {quoteItems.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return quoteItems;
        }
        [HttpPost]
        public ActionResult UpdateQOH(List<UpdateInventory> updateInventory, int purid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;

                // Check the current purchase order status
                com.CommandText = "SELECT PurStatus FROM PurchaseOrderForm WHERE PurId = @purId";
                com.Parameters.AddWithValue("@purId", purid);
                string currentStatus = com.ExecuteScalar()?.ToString();

                if (currentStatus == "Items Received")
                {
                    return Json(new { success = false, message = "Cannot update QOH. Status is already 'Items Received'." });
                }
                else if (currentStatus == "Return To Supplier")
                {
                    return Json(new { success = false, message = "Cannot update QOH. Items already returned to supplier." });
                }

                // Clear parameters after the check
                com.Parameters.Clear();

                foreach (var row in updateInventory)
                {
                    // Get the current QOH for the product
                    com.CommandText = "SELECT INVT_QOH FROM Inventory WHERE ProdId = @prodid";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("@prodid", row.ProdId);

                    int currentQOH = (int)com.ExecuteScalar();

                    // Calculate the new QOH by adding the current QOH to the new quantity
                    int newQOH = currentQOH + row.Quantity;

                    // Update the QOH in the database
                    com.CommandText = "UPDATE Inventory SET INVT_QOH = @newqoh WHERE ProdId = @prodid";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("@prodid", row.ProdId);
                    com.Parameters.AddWithValue("@newqoh", newQOH);

                    int recordAffected = com.ExecuteNonQuery();

                    // Update the Purchase Status in the PurchaseOrderForm table
                    com.CommandText = "UPDATE PurchaseOrderForm SET PurStatus = 'Items Received' WHERE PurId = @purId";
                    com.Parameters.Clear();
                    com.Parameters.AddWithValue("@purId", purid);
                    int statusUpdated = com.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "QOH and Purchase Status updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
            finally
            {
                con.Close();
            }
        }
        [HttpPost]
        public ActionResult ReturnToSupplier(int purid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;

                // Check if the specified purid exists in the PurchaseOrderForm
                com.CommandText = "SELECT COUNT(*) FROM PurchaseOrderForm WHERE PurId = @purId";
                com.Parameters.AddWithValue("@purId", purid);
                int puridCount = Convert.ToInt32(com.ExecuteScalar());

                if (puridCount == 0)
                {
                    return Json(new { response = false, message = "Purchase Order not found." });
                }

                // Check if the purchase order is eligible for return
                com.Parameters.Clear(); // Clear previous parameters
                com.CommandText = "UPDATE PurchaseOrderForm SET PurStatus = 'Return To Supplier' WHERE PurId = @purId";
                com.Parameters.AddWithValue("@purId", purid);
                recordAffected = com.ExecuteNonQuery();

                if (recordAffected > 0)
                {
                    return Json(new { response = true, message = "Purchase Order returned successfully!" });
                }
                else
                {
                    return Json(new { response = false, message = "No records were updated. Purchase Order not found or already returned." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception, don't just throw it
                // You can use a logging framework like log4net, Serilog, or write to a log file.
                // For simplicity, printing to console is used here.
                Console.WriteLine("Error in ReturnToSupplier action: " + ex.Message);
                return Json(new { response = false, message = "Error while processing the request. Please check the server logs." });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult DeletePurchaseOrder(int purId)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Update PurchaseOrderForm Set PurIsDelete = 1 where PurId = @purchaseId";
                com.Parameters.AddWithValue("@purchaseId", purId);
                recordAffected = com.ExecuteNonQuery();

                if (recordAffected > 0)
                {
                    return Json(new { response = true, message = "Purchase Order deleted sucessfully!" });
                }
                return Json(new { response = false, message = "Error while processing!" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult CustodianInventory()
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> sortResults = new List<ViewModel>();

            string MsgeResult = TempData["ResultMsge"] as string;

            sortResults = inventoryDAO.FetchAll();
           
            return View(sortResults);
        }
        public ActionResult CustodianSettings()
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            Account acc = new Account();
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            var accid = int.Parse(Session["userId"].ToString());
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM Account where AccId =  @accountid";
                com.Parameters.AddWithValue("@accountid", accid);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    acc.AccId = int.Parse(dr["AccId"].ToString());

                    acc.UserFullName = dr["AccFname"] + ", " + dr["AccMname"] + "," + dr["AccLname"].ToString();

                    acc.AccLname = dr["AccLname"].ToString();

                    acc.AccFname = dr["AccFname"].ToString();

                    acc.AccMname = dr["AccMname"].ToString();

                    acc.AccUserName = dr["AccUserName"].ToString();

                    acc.AccPassword = dr["AccPassword"].ToString();

                    acc.AccType = dr["AccType"].ToString();

                    acc.AccStatus = int.Parse(dr["AccIsActive"].ToString());
                }
                con.Close();
                return View(acc);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Ensure the connection is closed, even if an exception occurs
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult UpdateAccount(Account acc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "Update Account Set AccFname = @fname, AccMname = @mname, AccLname = @lname where AccId = @id";
                    com.Parameters.AddWithValue("@id", acc.AccId);
                    com.Parameters.AddWithValue("@fname", acc.AccFname);
                    com.Parameters.AddWithValue("@mname", acc.AccMname);
                    com.Parameters.AddWithValue("@lname", acc.AccLname);
                    recordAffected = com.ExecuteNonQuery();

                    if (recordAffected > 0)
                    {
                        TempData["Message"] = "Account Successfully Updated!";
                        return RedirectToAction("CustodianSettings", "CustodianPage");
                    }

                    TempData["Message"] = "Error! While updating account.";
                    return RedirectToAction("CustodianSettings", "CustodianPage", acc);
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it in a more meaningful way.
                    throw ex;
                }
                finally
                {
                    // Ensure the connection is closed, even if an exception occurs
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
            TempData["Message"] = "Error! While updating account.";
            return RedirectToAction("CustodianSettings", "CustodianPage", acc);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            // Redirect to the login page
            return RedirectToAction("Login", "Home");
        }
    }
}