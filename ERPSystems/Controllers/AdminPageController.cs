using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;

namespace ERPSystems.Controllers
{
    public class AdminPageController : Controller
    {
        // GET: AdminPage
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<Account> userAccounts = new List<Account>();
        List<RequestForm> requestForm = new List<RequestForm>();

      

        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        public ActionResult SamplePage()
        {
            return View();
        }
        public ActionResult AdminDashboard()
        {
            return View();
        }
        public ActionResult Inventory()
        {
            return View();
        }
        public ActionResult PurchaseRequest(RequestModel model)
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
                        TotalItem =int.Parse(dr["ReqTotalItem"].ToString())
                    ,
                        RequestorName = dr["AccFname"]+" "+ dr["AccLname"].ToString()
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
            catch
            {
                throw;
            }
           return (requestItem);
        }      
        public ActionResult Supplier()
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
                com.CommandText = "SELECT * FROM Useraccount";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    userAccounts.Add(new Account
                    {
                        UserID = int.Parse(dr["UseraccountID"].ToString())
                    ,
                        UserFullName = dr["UseraccountLNAME"] + ", " + dr["UseraccountFNAME"] + " " + dr["UseraccountUSERNAME"].ToString()
                    ,
                        UserName = dr["UseraccountUSERNAME"].ToString()
                    ,
                        UserPassword = dr["UseraccountPASSWORD"].ToString()
                    ,
                        UserType = dr["UseraccountType"].ToString()
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
        public ActionResult Account(Account acc)
        {      

            int id = acc.UserID;
            string type = acc.UserType;
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE Useraccount set UseraccountType = '" + type + "' where UseraccountId = '" + id + "'";
                com.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw;
            }
            return View("Account");
        }
    }
}