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
        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        // GET: PurchasingPage
        public ActionResult PurchasingDashboard()
        {
            return View();
        }
        public ActionResult PurchasingPageRequest(RequestModel model)
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
        [HttpPost]
        public List<Inventory> FetchProdIdInventory(int[] id)
        {
            RequestModel model = new RequestModel();
            connnectionString();
            try
            {
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
                while(dr.Read())
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
            catch
            {
                throw;
            }
            return (requestItem);
        }
        public ActionResult PurchasingPageCanvass()
        {
            return View();
        }
        public ActionResult PurchasingPageQoutation()
        {
            return View();
        }

      
        public ActionResult CreatePurchaseOrderForm(int id)
        {
            try
            {
                if(!CheckPoExist(id))
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
            catch(Exception e)
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
            while(dr.Read())
            {
                reqid = int.Parse(dr["ReqId"].ToString());
            }
            con.Close();
            if(id == reqid)
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
            catch(Exception e)
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
                com.CommandText = "select * from PurchaseOrderForm";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    purchaseform.Add(new PurchasingForm
                    {
                        PurId = int.Parse(dr["PurId"].ToString())
                    ,
                        ReqId =int.Parse(dr["ReqId"].ToString())
                    ,
                        PurTotalItem = int.Parse(dr["PurTotalItem"].ToString())
                    ,
                        PurStatus = dr["PurStatus"].ToString()                 
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
                        PurCreated =DateTime.Parse(dr["PurCreateAt"].ToString())
                    ,
                        PurStatus = dr["PurStatus"].ToString()
                    ,
                        RequestFrom = dr["AccFname"].ToString() + " " + dr["AccMname"].ToString().Substring(0,1) +". "+ dr["AccLname"].ToString()
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
                        ProdDescription = dr["ProdDescription"].ToString()
                    ,
                        PurQuantity = int.Parse(dr["PurQuantity"].ToString())
                    ,
                        PurUnit = dr["PurUnit"].ToString()
                    }); 
                }
                con.Close();
            }
            catch
            {

            }
            return (purchaseorderitem);
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
                        Console.WriteLine($"ProdId: {row.column1}, PurQuantity: {row.column4}, PurUnit: {row.column7}");
                        // ... rest of the loop
                        prodid = int.Parse(row.column1);
                        com.Parameters.AddWithValue("@purid", purid);
                        com.Parameters.AddWithValue("@prodid", row.column1);
                        com.Parameters.AddWithValue("@purquantity", row.column4);
                        com.Parameters.AddWithValue("@purunit", row.column7);
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
    }
}