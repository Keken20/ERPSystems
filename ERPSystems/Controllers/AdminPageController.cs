using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystem.Models;
using System.Data.SqlClient;
using ERPSystem.Data;

namespace ERPSystems.Controllers
{
    public class AdminPageController : Controller
    {
        // GET: AdminPage
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<RequestItem> requestItem = new List<RequestItem>();
        List<UserAccount> userAccounts = new List<UserAccount>();
        List<RequestForm> requestForm = new List<RequestForm>();



        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystem.Properties.Resources.ConnectionString;
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
                com.CommandText = "select * From RequisitionForm inner join Account on RequisitionForm.AccId = Account.AccId";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    //RequestForm reqForm = new RequestForm();

                    //reqForm.RequestId = int.Parse(dr["ReqId"].ToString());
                    //reqForm.RequestDate = DateTime.Parse(dr["ReqCreatedAt"].ToString());
                    ////reqForm.TotalItem = int.Parse(dr["ReqTotalItem"].ToString());
                    //reqForm.RequestorName = dr["AccFname"] + " " + dr["AccLname"].ToString();
                    //reqForm.Status = dr["ReqStatus"].ToString();

                    //requestForm.Add(reqForm);

                    requestForm.Add(new RequestForm
                    {
                        RequestId = int.Parse(dr["ReqId"].ToString())
                    ,
                        RequestDate = DateTime.Parse(dr["ReqCreatedAt"].ToString())
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
                com.CommandText = "SELECT * FROM Account";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    userAccounts.Add(new UserAccount
                    {
                        UserID = int.Parse(dr["AccID"].ToString())
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
        public ActionResult Account(UserAccount acc)
        {

            int id = acc.UserID;
            string type = acc.UserType;
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "UPDATE Account set AccType = '" + type + "' where AccId = '" + id + "'";
                com.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                throw;
            }
            return View("Account");
        }

        public ActionResult Dashboard()
        {
            if(Session["userId"] != null)
            {
                var accID = Convert.ToInt32(Session["userId"]);
                //Initialize Data Access Objects 
                UserAccountDAO userAccountDAO = new UserAccountDAO();
                InventoryDAO inventoryDAO = new InventoryDAO();
                

                UserAccount user = userAccountDAO.fetchAccount(accID);
                DashboardViewModel stats = new DashboardViewModel { User = user, 
                    inventoryCount = inventoryDAO.countInventory()
                };
                

                return View(stats);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
    }
}