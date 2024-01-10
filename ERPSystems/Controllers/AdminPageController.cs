using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using System.Data;
using PagedList;
using PagedList.Mvc;
using ERPSystems.Data;


namespace ERPSystems.Controllers
{
    public class AdminPageController : Controller
    {
        int purid;
        int recordAffected;
        int prodid;
        int reqid;
        // GET: AdminPage
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<Account> userAccounts = new List<Account>();
        List<RequestForm> requestForm = new List<RequestForm>();
        List<PurchasingForm> purchaseform = new List<PurchasingForm>();
        List<QuotationForm> quotationforms = new List<QuotationForm>();
        List<QuotationItem> quotationitems = new List<QuotationItem>();
        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        public ActionResult AdminDashboard()
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult Inventory()
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult PurchaseRequest(RequestModel model)
        {
             if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
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
                com.CommandText = "select * From RequisitionForm inner join  Account on RequisitionForm.AccId = Account.AccId where RFSubmit = 1 and ReqIsActive = 1";
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
                com.CommandText = "select * from RequisitionItem inner join Product on Product.ProdId = RequisitionItem.ProdId where ReqId =@reqid";
                com.Parameters.AddWithValue("@reqid", id);
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
            catch
            {
                throw;
            }
            return (requestItem);
        }


        public ActionResult Supplier(int? page, string sortId)
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            else
            {
                List<Supplier> supplierList = new List<Supplier>();

                SupplierDAO supplierDAO = new SupplierDAO();

                ViewBag.SortId = sortId;

                if (sortId == null || sortId == "")
                {
                    supplierList = supplierDAO.GetAllActive();
                }
                else
                {
                    supplierList = supplierDAO.SortBy(sortId);
                }

                var returnList = supplierList.ToPagedList(page ?? 1, 5);


                return View("Supplier", returnList);
            }
        }

        [HttpPost]
        public ActionResult Search(string searchId, int? page)
        {
            Supplier supplier = new Supplier();

            List<Supplier> supplierList = new List<Supplier>();

            if (searchId == "")
            {
                Response.Write("<script>alert('Fields must not be empty');</script>");

                SupplierDAO supplierDAO = new SupplierDAO();

                supplierList = supplierDAO.GetAllActive();
            }
            else
            {
                SupplierDAO supplierDAO = new SupplierDAO();

                supplierList = supplierDAO.SearchName(searchId);
            }

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("Supplier", returnList);
        }

        public ActionResult Account()
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
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
                        AccId = int.Parse(dr["AccID"].ToString())
                    ,
                        UserFullName = dr["AccFname"] + ", " + dr["AccMname"] + ", " + dr["AccLname"].ToString()
                    ,
                        AccUserName = dr["AccUserName"].ToString()
                    ,
                        AccPassword = dr["AccPassword"].ToString()
                    ,
                        AccType = dr["AccType"].ToString()
                    ,
                        AccStatus = int.Parse(dr["AccIsActive"].ToString())
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
        public ActionResult Account(Account userAccounts, string submitbutton)
        {
            int id = userAccounts.AccId;
            string type = userAccounts.AccType;
            if (submitbutton == "updateBtn")
            {
                try
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE Account set AccType = @type,AccCreatedAt = Current_timestamp where AccId = @accid";
                    com.Parameters.AddWithValue("@type", type);
                    com.Parameters.AddWithValue("@accid", id);
                    recordAffected = com.ExecuteNonQuery();
                    if (recordAffected > 0)
                    {
                        TempData["Update"] = "Updated sucessfully!";
                    }
                    con.Close();
                    return RedirectToAction("Account");
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else if (submitbutton == "deactBtn")
            {
                try
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE Account set AccIsActive = 0 where AccId = @accountid";
                    com.Parameters.AddWithValue("@accountid", id);
                    recordAffected = com.ExecuteNonQuery();
                    if (recordAffected > 0)
                    {
                        TempData["Status"] = "Deactivated sucessfully!";
                    }
                    con.Close();
                    return RedirectToAction("Account");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE Account set AccIsActive = 1 where AccId = @userid";
                    com.Parameters.AddWithValue("@userid", id);
                    recordAffected = com.ExecuteNonQuery();
                    if (recordAffected > 0)
                    {
                        TempData["Status"] = "Activated sucessfully!";
                    }
                    con.Close();
                    return RedirectToAction("Account");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public ActionResult RequestList(RequestModel model)
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            FetchRequest();
            List<RequestForm> requestForms = new List<RequestForm>();
            model.requestform = requestForm;
            return View(model);
        }
        public ActionResult QuotationList(RequestModel model)
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            FetchPurchaseOrder();
            model.purchaseorder = purchaseform;
            return View(model);
        }
        public ActionResult UpdateStatus(int reqid, string decision)
        {
            try
            {
                connnectionString();
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE RequisitionForm SET ReqStatus = @status where ReqId = @id ";
                if (decision == "Approved")
                {
                    com.Parameters.AddWithValue("@id", reqid);
                    com.Parameters.AddWithValue("@status", decision);
                    int resulaffected = com.ExecuteNonQuery();
                    con.Close();
                    if (resulaffected == 0)
                    {
                        return Json(new { response = false, message = "Can't find request" });
                    }
                    else
                    {
                        if (!CheckPoExist(reqid))
                        {
                            connnectionString();
                            con.Open();
                            com.Connection = con;
                            com.CommandText = "Insert into PurchaseOrderForm(ReqId)values(@requid)";
                            com.Parameters.AddWithValue("@requid", reqid);
                            recordAffected = com.ExecuteNonQuery();
                            con.Close();
                            if (recordAffected > 0)
                            {
                                return Json(new { success = true, message = "Requisition Request Approved! and Purchase order form created successfully" });
                            }
                            else
                            {
                                return Json(new { success = false, message = "Failed to create purchase order form" });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Request is already approved" });
                        }
                    }
                }
                else
                {
                    com.Parameters.AddWithValue("@id", reqid);
                    com.Parameters.AddWithValue("@status", decision);
                    int resulaffected = com.ExecuteNonQuery();
                    if (resulaffected == 0)
                    {
                        return Json(new { response = false, message = "Can't find request" });
                    }

                    return Json(new { response = true, message = "Requisition Request Disapproved!" });
                }
            }
            catch
            {
                con.Close();
                return Json(new { response = false, message = "Can't process action!" });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public bool CheckPoExist(int id)
        {
            connnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "Select * from PurchaseOrderForm where ReqId = @reqid";
            com.Parameters.AddWithValue("@reqid", id);
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                reqid = int.Parse(dr["ReqId"].ToString());
            }
            con.Close();
            if (id == reqid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult SaveProductToPO(List<ProductToPO> tableData, string reqid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from PurchaseOrderForm where ReqId = @id";
                com.Parameters.AddWithValue("@id", int.Parse(reqid));
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    purid = int.Parse(dr["PurId"].ToString());
                }
                con.Close();
                if (purid > 0)
                {
                    foreach (var row in tableData)
                    {
                        com.Parameters.Clear();
                        prodid = int.Parse(row.column1);

                        if (!CheckPoItemExist(purid, prodid))
                        {
                            try
                            {
                                connnectionString();
                                con.Open();
                                com.Connection = con;
                                com.CommandText = "insert into PurchaseOrderItem(PurId,ProdId,PurQuantity,PurUnit)values(@purid,@prdid,@purquantity,@purunit)";
                                com.Parameters.AddWithValue("@purid", purid);
                                com.Parameters.AddWithValue("@prdid", row.column1);
                                com.Parameters.AddWithValue("@purquantity", row.column7);
                                com.Parameters.AddWithValue("@purunit", row.column4);
                                recordAffected = com.ExecuteNonQuery();
                                if (recordAffected <= 0)
                                {
                                    con.Close();
                                    return Json(new { success = false, message = "Failed to insert data into PurchaseOrderItem" });
                                }
                                con.Close();
                                UpdateTotalPurItem(purid);
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        else
                        {
                            con.Close();
                            return Json(new { success = false, message = "Selected product is already saved!" });
                        }
                    }
                    con.Close();
                    return Json(new { success = true, message = "Data saved successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid PurId" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Error occurred while processing the request" });
            }
        }
        public bool CheckPoItemExist(int purid, int prodid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Select * from PurchaseOrderItem where PurId = @poId and Prodid = @prodId";
                com.Parameters.AddWithValue("@poId", purid);
                com.Parameters.AddWithValue("@prodId", prodid);
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    con.Close();
                    return true;
                }
                else
                {
                    con.Close();
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }
        private void UpdateTotalPurItem(int purid)
        {
            connnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "UPDATE PurchaseOrderForm SET PurTotalItem = (SELECT COUNT(ProdId) from purchaseorderitem where PurId =  @purchaseid)";
            com.Parameters.AddWithValue("@purchaseid", purid);
            recordAffected = com.ExecuteNonQuery();
            con.Close();
        }
        [HttpPost]
        public List<PurchasingForm> FetchPurchaseOrder()
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT p.PurId,p.ReqId,p.PurTotalItem,p.PurStatus,(SELECT COUNT(QouteId)FROM QoutationForm q WHERE q.PurId = p.PurId) AS QuoteCount FROM PurchaseOrderForm p Where POSubmit = 1 and PurIsDelete = 0";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    purchaseform.Add(new PurchasingForm
                    {
                        PurId = int.Parse(dr["PurId"].ToString())
                    ,
                        ReqId = int.Parse(dr["ReqId"].ToString())
                    ,
                        PurTotalItem = int.Parse(dr["PurTotalItem"].ToString())
                    ,
                        PurStatus = dr["PurStatus"].ToString()
                    ,
                        QuoteCount = int.Parse(dr["QuoteCount"].ToString())
                    });
                }
                con.Close();
            }
            catch
            {
                throw;
            }
            return (purchaseform);
        }
        public ActionResult GetQuoteItem(int Qouteid)
        {
            RequestModel model = new RequestModel();
            AssignQoutationItem(Qouteid);
            model.quotationitem = quotationitems;
            return Json(model);
        }
        [HttpPost]
        public List<QuotationItem> AssignQoutationItem(int qid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from QoutationItem inner join Product on QoutationItem.ProdId = Product.ProdId where QouteId = @qouid";
                com.Parameters.AddWithValue("@qouid", qid);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    quotationitems.Add(new QuotationItem
                    {
                        QuoteId = int.Parse(dr["QouteId"].ToString()),
                        ProdId = int.Parse(dr["ProdId"].ToString()),
                        QuoteUnit = dr["QouteUnit"].ToString(),
                        QuoteQouantity = int.Parse(dr["QouteQuantity"].ToString()),
                        QuotePricePerUnit = double.Parse(dr["QoutePricePerUnit"].ToString()),
                        ProdDescription = dr["ProdDescription"].ToString(),
                        ProdName = dr["ProdName"].ToString()
                    });
                }
                con.Close();
            }
            catch
            {
                throw;
            }
            return (quotationitems);
        }
        public ActionResult AdminQuotationList(int purId, DateTime purCreated)
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            ViewBag.PurId = purId;
            ViewBag.PurCreated = purCreated.ToString("d");
            GetQuoteInfo(purId);
            RequestModel model = new RequestModel();
            model.quotationform = quotationforms;
            return View(model);
        }
        public List<QuotationForm> GetQuoteInfo(int qid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM QoutationForm WHERE PurId = @qouid";
                com.Parameters.AddWithValue("@qouid", qid);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    quotationforms.Add(new QuotationForm
                    {
                        QuoteId = int.Parse(dr["QouteId"].ToString()),
                        PurId = int.Parse(dr["PurId"].ToString()),
                        QuoteCreated = DateTime.Parse(dr["QouteCreatedAt"].ToString()),
                        QuoteSubTotal = double.Parse(dr["QouteSubtotal"].ToString()),
                        QuoteDiscount = double.Parse(dr["QouteDiscount"].ToString()),
                        QuoteTotal = double.Parse(dr["QouteTotal"].ToString()),
                        QuoteStatus = dr["QouteStatus"].ToString()
                    });
                }
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
            return (quotationforms);
        }
        public ActionResult Settings()
        {
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
        public ActionResult Logout()
        {
            Session.Abandon();
            // Redirect to the login page
            return RedirectToAction("Login", "Home");
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
                        return RedirectToAction("Settings", "AdminPage");
                    }

                    TempData["Message"] = "Error! While updating account.";
                    return RedirectToAction("Settings", "AdminPage", acc);
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
            return RedirectToAction("Settings", "AdminPage", acc);
        }
    }
}