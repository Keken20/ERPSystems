using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using System.Data;

namespace ERPSystems.Controllers
{
    public class PurchasingPageController : Controller
    {
        int purid;
        int recordAffected;
        int prodid;
        int reqid;
        int quoteid;
        string Suppliername;
        int GetsuppId;
        int SupplierId;
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<Account> userAccounts = new List<Account>();
        List<RequestForm> requestForm = new List<RequestForm>();
        List<Inventory> inventory = new List<Inventory>();
        List<PurchasingForm> purchaseform = new List<PurchasingForm>();
        List<PurchaseOrderFormInfo> purchaseforminfo = new List<PurchaseOrderFormInfo>();
        List<PurchaseOrderItem> purchaseorderitem = new List<PurchaseOrderItem>();
        List<ProductToPO> producttopo = new List<ProductToPO>();
        List<QuotationForm> quotationforms = new List<QuotationForm>();
        List<QuotationItem> quotationitems = new List<QuotationItem>();
        List<SupplierInfo> supplierinfo = new List<SupplierInfo>();

        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        // GET: PurchasingPage
        public ActionResult PurchasingPageRequest(RequestModel model)
        {
            int userId = int.Parse(Session["userId"].ToString());
            string username = (string)Session["userName"];
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
                com.CommandText = "select * From RequisitionForm inner join  Account on RequisitionForm.AccId = Account.AccId where ReqIsActive = 1";
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
        [HttpPost]
        public List<Inventory> FetchProdIdInventory(int[] id)
        {
            RequestModel model = new RequestModel();
            connnectionString();
            try
            {
                Response.Write(id);
                con.Open();
                com.Connection = con;
                string parameterizedIds = string.Join(",", id.Select((_, index) => $"@prodid{index}"));
                com.CommandText = $"SELECT * FROM inventory WHERE ProdId IN ({parameterizedIds})";
                // Add parameters to the command
                for (int i = 0; i < id.Length; i++)
                {
                    com.Parameters.AddWithValue($"@prodid{i}", id[i]);
                }
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    inventory.Add(new Inventory
                    {
                        ProdQoh = int.Parse(dr["ProdQOH"].ToString())
                    });
                }
            }
            catch
            {

            }
            return (inventory);
        }
        public ActionResult ReceivedProdId(int[] id)
        {
            List<Inventory> inv = FetchProdIdInventory(id);
            RequestModel model = new RequestModel();
            {
                model.inventory = inv;
            };
            return Json(model);
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
                com.CommandText = "select * from RequisitionForm inner join RequisitionItem on RequisitionForm.ReqId = RequisitionItem.ReqId inner join Inventory on RequisitionItem.ProdId = Inventory.ProdId where ReqId = '" + reqid + "'";
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

        public ActionResult CreatePurchaseOrderForm(int id)
        {
            try
            {
                if (!CheckPoExist(id))
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "Insert into PurchaseOrderForm(ReqId)values(@requid)";
                    com.Parameters.AddWithValue("@requid", id);
                    recordAffected = com.ExecuteNonQuery();
                    con.Close();
                    if (recordAffected > 0)
                    {
                        UpdateRequestStatus(id);
                        return Json(new { success = true, message = "Purchase order form created successfully" });
                    }
                    else
                    {
                        recordAffected = 0;
                        return Json(new { success = false, message = "Failed to create purchase order form" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Request is already approved" });
                }

            }
            catch (Exception e)
            {
                Response.Write(e.Message);
                return Json(new { success = false, message = "An error occurred" });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();  // Ensure the connection is closed even in case of an exception
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
        public void UpdateRequestStatus(int id)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "update RequisitionForm set ReqStatus = 'Approved' where ReqId = @id";
                com.Parameters.AddWithValue("@id", id);
                int recordAffected = com.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Response.Write(e.Message);
            }

            finally
            {
                con.Close();
            }
        }
        public ActionResult PurchaseOrder(RequestModel model)
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
        [HttpPost]
        public List<PurchasingForm> FetchPurchaseOrder()
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT p.PurId,p.ReqId,p.PurTotalItem,p.PurStatus,(SELECT COUNT(QouteId)FROM QoutationForm q WHERE q.PurId = p.PurId) AS QuoteCount FROM PurchaseOrderForm p Where PurIsDelete = 0";
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
        public ActionResult GetPurchaseOrderInfo(int selectedPurId)
        {
            GetPurchaseOrderFormInfo(selectedPurId);
            RequestModel model = new RequestModel();
            model.purchaseorderinfo = purchaseforminfo;
            return Json(model);
        }

        public List<PurchaseOrderFormInfo> GetPurchaseOrderFormInfo(int id)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from PurchaseOrderForm inner join RequisitionForm on PurchaseOrderForm.ReqId = RequisitionForm.ReqId inner join Account on RequisitionForm.AccId = Account.AccId where PurId = @id";
                com.Parameters.AddWithValue("@id", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    purchaseforminfo.Add(new PurchaseOrderFormInfo
                    {
                        PurId = int.Parse(dr["PurId"].ToString())
                    ,
                        ReqId = int.Parse(dr["ReqId"].ToString())
                    ,
                        PurCreated = DateTime.Parse(dr["PurCreateAt"].ToString())
                    ,
                        PurStatus = dr["PurStatus"].ToString()
                    ,
                        RequestFrom = dr["AccFname"].ToString() + " " + dr["AccMname"].ToString().Substring(0, 1) + ". " + dr["AccLname"].ToString()
                    });
                }
                con.Close();
            }
            catch
            {
                throw;
            }
            return (purchaseforminfo);
        }
        [HttpPost]
        public ActionResult GetPOItem(int selectedPurId)
        {
            GetPurchaseOrderItem(selectedPurId);
            RequestModel model = new RequestModel();
            model.purchaseorderitem = purchaseorderitem;
            return Json(model);
        }
        public List<PurchaseOrderItem> GetPurchaseOrderItem(int purid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select PurId,  Product.ProdId, ProdName, ProdDescription, PurUnit, PurQuantity from PurchaseOrderItem inner join Product on PurchaseOrderItem.ProdId = Product.ProdId where PurId = @id";
                com.Parameters.AddWithValue("@id", purid);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    purchaseorderitem.Add(new PurchaseOrderItem
                    {
                        PurId = int.Parse(dr["PurId"].ToString())
                    ,
                        ProdId = int.Parse(dr["ProdId"].ToString())
                    ,
                        ProdName = string.Format(dr["ProdName"].ToString())
                    ,
                        ProdDescription = string.Format(dr["ProdDescription"].ToString())
                    ,
                        PurQuantity = int.Parse(dr["PurQuantity"].ToString())
                    ,
                        PurUnit = dr["PurUnit"].ToString()
                    });
                    Console.WriteLine(purchaseorderitem);
                }
                con.Close();
                return (purchaseorderitem);
            }
            catch(Exception ex)
            {
                throw ex;
            }          
        }
        [HttpPost]
        public ActionResult SaveProductToPO(List<ProductToPO> tableData, string reqid)
        {
            connnectionString();
            try
            {
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
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "insert into PurchaseOrderItem(PurId,ProdId,PurQuantity,PurUnit)values(@purid,@prodid,@purquantity,@purunit)";
                    foreach (var row in tableData)
                    {
                        com.Parameters.Clear();
                        Console.WriteLine($"ProdId: {row.column1}, PurQuantity: {row.column7}, PurUnit: {row.column4}");
                        // ... rest of the loop
                        prodid = int.Parse(row.column1);
                        com.Parameters.AddWithValue("@purid", purid);
                        com.Parameters.AddWithValue("@prodid", row.column1);
                        com.Parameters.AddWithValue("@purquantity", row.column7);
                        com.Parameters.AddWithValue("@purunit", row.column4);
                        recordAffected = com.ExecuteNonQuery();
                        if (recordAffected <= 0)
                        {
                            return Json(new { success = false, message = "Failed to insert data into PurchaseOrderItem" });
                        }
                    }
                    con.Close();
                    UpdateTotalPurItem(purid);
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
        private void UpdateTotalPurItem(int poid)
        {
            con.Open();
            com.Connection = con;
            com.CommandText = "UPDATE PurchaseOrderForm SET PurTotalItem = (SELECT COUNT(ProdId) from purchaseorderitem where PurId =  @poid)";
            com.Parameters.AddWithValue("@poid", purid);
            com.ExecuteNonQuery();
            con.Close();

        }
        public ActionResult PurchasingQoutationPage(int purId, DateTime purCreated)
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
                return (quotationitems);

            }
            catch
            {
                throw;
            }

        }
        public ActionResult SendRequestoAdmin(int reqid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE RequisitionForm SET RFSubmit = 1 where ReqId = @reid";
                com.Parameters.AddWithValue("@reid", reqid);
                recordAffected = com.ExecuteNonQuery();
                if (recordAffected > 0)
                {
                    return Json(new { response = true, message = "Submitted to Top Management." });
                }
                else
                {
                    return Json(new { response = false, message = "Error occurred while processing the request" });
                }

            }
            catch (Exception ex)
            {
                con.Close();
                return Json(new { response = false, message = ex.Message + "Error occurred while processing the request" });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult SendPoToAdmin(int purid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE PurchaseOrderForm SET POSubmit = 1 where PurId = @purchaseid";
                com.Parameters.AddWithValue("@purchaseid", purid);
                recordAffected = com.ExecuteNonQuery();
                if (recordAffected > 0)
                {
                    return Json(new { response = true, message = "Submitted to Top Management." });
                }
                else
                {
                    return Json(new { response = false, message = "Error occurred while processing the request" });
                }

            }
            catch (Exception ex)
            {
                con.Close();
                return Json(new { response = false, message = ex.Message + "Error occurred while processing the request" });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult ApproveQuotation(int QuoteId, int PurId)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE QoutationForm SET QouteStatus = 'Approved' where QouteId = @id";
                com.Parameters.AddWithValue("@id", QuoteId);
                recordAffected = com.ExecuteNonQuery();
                con.Close();
                if (recordAffected > 0)
                {
                    List<SupplierInfo> supplierinfo = GetSuppInfo(QuoteId);
                    foreach (var data in supplierinfo)
                    {
                        Suppliername = data.SuppName;
                    }
                    if (!CheckSupplierExist(Suppliername))
                    {
                        InsertSupplier(QuoteId);
                        SupplierId = GetSupplierID(Suppliername);
                    }
                    SupplierId = GetSupplierID(Suppliername);
                    InsertSuppIdPO(SupplierId, PurId);
                    return Json(new { response = true, message = "Purchase Order ID: " + PurId + " Approved Successfully!" });
                }
                else
                {
                    return Json(new { response = false, message = "Error occurred while processing." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging and troubleshooting
                // You might want to use a logging framework like Serilog or log4net
                Console.WriteLine(ex.Message);
                return Json(new { response = false, message = "Error occurred while processing." });
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public JsonResult InsertSupplier(int qouteid)
        {
            try
            {
                List<SupplierInfo> supplierinfo = GetSuppInfo(qouteid);
                // Process supplierinfo as needed              
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Insert into Supplier(SuppName,SuppPhone,SuppCity,SuppMunicipality,SuppBarangay,SuppZipcode) Values(@SuppName,@Phone,@City,@Municipality,@Barangay,@Zipcode)";
                foreach (var data in supplierinfo)
                {
                    com.Parameters.AddWithValue("@SuppName", data.SuppName);
                    com.Parameters.AddWithValue("@Phone", data.SuppPhone);
                    com.Parameters.AddWithValue("@City", data.SuppCity);
                    com.Parameters.AddWithValue("@Municipality", data.SuppMunicipality);
                    com.Parameters.AddWithValue("@Barangay", data.SuppBarangay);
                    com.Parameters.AddWithValue("@Zipcode", data.SuppZipcode);
                    recordAffected = com.ExecuteNonQuery();
                    if (recordAffected < 0)
                    {
                        return Json(new { response = false, message = "Error occurred while processing." });
                    }
                }
                return Json(new { response = true, message = "Supplier information retrieved successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { response = false, message = "Error occurred while processing." });
            }
        }
        public List<SupplierInfo> GetSuppInfo(int qid)
        {
            List<SupplierInfo> supplierinfo = new List<SupplierInfo>();
            try
            {
                quoteid = qid;
                connnectionString();
                con.Open();
                com.Connection = con;
                com.Parameters.Clear();
                com.CommandText = "SELECT SuppName, SuppPhone, SuppCity, SuppMunicipality, SuppBarangay, SuppZipcode FROM QoutationForm WHERE Qouteid = @qid";
                com.Parameters.AddWithValue("@qid", qid);

                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        supplierinfo.Add(new SupplierInfo
                        {
                            SuppName = dr["SuppName"].ToString(),
                            SuppPhone = dr["SuppPhone"].ToString(),
                            SuppCity = dr["SuppCity"].ToString(),
                            SuppMunicipality = dr["SuppMunicipality"].ToString(),
                            SuppBarangay = dr["SuppBarangay"].ToString(),
                            SuppZipcode = dr["SuppZipcode"].ToString()
                        });
                    }
                }
                return supplierinfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw; // Rethrow the exception for higher-level error handling
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public bool CheckSupplierExist(string suppname)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Select * from Supplier where SuppName = @Suppliername";
                com.Parameters.AddWithValue("@Suppliername", suppname);
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public int GetSupplierID(string suppname)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Select SuppId from Supplier where SuppName = @suppname";
                com.Parameters.AddWithValue("@suppname", suppname);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    GetsuppId = int.Parse(dr["SuppId"].ToString());
                }
                return (GetsuppId);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public ActionResult InsertSuppIdPO(int suppid, int poid)
        {
            try
            {

                connnectionString();
                con.Open();
                com.Parameters.Clear();
                com.Connection = con;
                com.CommandText = "Update PurchaseOrderForm Set SuppId = @suppid,PurStatus = 'Approved' ,PurUpdatedAt = CURRENT_TIMESTAMP WHERE PurId = @PoId";
                com.Parameters.AddWithValue("@suppid", suppid);
                com.Parameters.AddWithValue("@PoId", poid);
                recordAffected = com.ExecuteNonQuery();
                if (recordAffected > 0)
                {
                    return new HttpStatusCodeResult(200);
                }
                return new HttpStatusCodeResult(500);
            }
            catch(Exception ex)
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
        public ActionResult DeleteRequest(int reqId)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Update RequisitionForm Set ReqIsActive = 0 where ReqId = @RequestId";
                com.Parameters.AddWithValue("@RequestId",reqId);
                recordAffected = com.ExecuteNonQuery();
                if(recordAffected > 0)
                {
                    return Json(new { response = true, message = "Request deleted sucessfully!" });
                }
                return Json(new { response = false, message = "Error while processing!" });
            }
            catch(Exception ex)
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
                    return Json(new { response = true, message = "Pruchase Order deleted sucessfully!" });
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
        public ActionResult PurchaseSettings()
        {
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
            Account acc = new Account();
            var accid = int.Parse(Session["userId"].ToString());
            if (Session["userId"] == null)
            {
                // Redirect to another action or URL
                return RedirectToAction("Login", "Home");
            }
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
            catch 
            {
                TempData["Message"] = "Error! While updating account.";
                return View(acc);

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
        public ActionResult DisapproveRequest(int reqid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Update RequisitionForm Set ReqStatus = 'Denied' where ReqId = @reqid";
                com.Parameters.AddWithValue("@reqid", reqid);
                recordAffected = com.ExecuteNonQuery();

                if (recordAffected > 0)
                {
                    return Json(new { response = true, message = "Request Order denied sucessfully!" });
                }
                return Json(new { response = false, message = "Error while processing!" });
            }
            catch
            {
                
                return Json(new { response = false, message = "Error while processing!" });
            }
            finally
            {
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
                        return RedirectToAction("PurchaseSettings", "PurchasingPage", acc);
                    }

                    TempData["Message"] = "Error! While updating account.";
                    return RedirectToAction("PurchaseSettings", "PurchasingPage", acc);
                }
                catch
                {
                    TempData["Message"] = "Error! While updating account.";
                    return RedirectToAction("PurchaseSettings", "PurchasingPage", acc);
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
            else
            {              
                TempData["Message"] = "Error! While updating account.";
                return RedirectToAction("PurchaseSettings", "PurchasingPage", acc);
            }
          

        }
        
        public ActionResult GetQouteItemPo(int purid)
        {
            GetQouteItemsPO(purid);
            RequestModel requestModel = new RequestModel();
            requestModel.quotationitem = quotationitems;
            return Json(requestModel);
 
        }
        
        public List<QuotationItem> GetQouteItemsPO(int purid)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "Select * from PurchaseOrderForm inner join Supplier on PurchaseOrderForm.SuppId = Supplier.SuppId inner join QoutationForm on QoutationForm.PurId = PurchaseOrderForm.PurId inner join Qoutationitem on QoutationItem.QouteId = QoutationForm.QouteId  inner join Product on Product.ProdId = QoutationItem.ProdId where PurchaseOrderForm.PurId = @purid and QouteStatus = 'Approved'";
                com.Parameters.AddWithValue("@purid", purid);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    quotationitems.Add(new QuotationItem
                    {
                        ProdId = int.Parse(dr["ProdId"].ToString()),
                        QuoteUnit = dr["QouteUnit"].ToString(),
                        QuoteQouantity = int.Parse(dr["QouteQuantity"].ToString()),
                        QuotePricePerUnit = double.Parse(dr["QoutePricePerUnit"].ToString()),
                        ProdDescription = dr["ProdDescription"].ToString(),
                        ProdName = dr["ProdName"].ToString()
                    });
                }
                return (quotationitems);
            }
            catch
            {
                throw;
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
    }
}

