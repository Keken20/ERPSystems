using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystem.Models;
using System.Data.SqlClient;

namespace ERPSystem.Controllers
{
    public class DefaultController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        List<UserAccount> userAccounts = new List<UserAccount>();
        [HttpGet]
        private void connnectionString()
        { 
            con.ConnectionString = ERPSystem.Properties.Resources.ConnectionString;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AdminDashboard()
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
            if(userAccounts.Count > 0)
            {
                userAccounts.Clear();
            }
            try
            {
                con.Open();
                com.Connection = con;
                com.CommandText = "SELECT * FROM ACCOUNT";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    userAccounts.Add(new UserAccount
                    {
                        UserID = int.Parse(dr["UserID"].ToString())
                    ,
                        UserName = dr["UserName"].ToString()
                    ,
                        UserPassword = dr["UserPassword"].ToString()
                    });
                }
                con.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult Inventory()
        {
            return View();
        }
        public ActionResult PurchaseRequest()
        {
            return View();
        }
        public ActionResult Supplier()
        {
            return View();
        }
        public ActionResult ExampleLogin()
        {
            return View();
        }

    }
}