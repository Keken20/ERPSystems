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
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
        int recordaffected;
        List<Account> userAccounts = new List<Account>();
        [HttpGet]
        private void connnectionString()
        {
            con.ConnectionString = ERPSystems.Properties.Resources.ConnectionString;
        }
        
        public ActionResult Home()
        {
            return View();
        }
      
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyLogin(Account account)
        {
            try
            {
                connnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Account where AccUserName = @Username and AccPassword = @Password and AccIsActive = 1";
                com.Parameters.AddWithValue("@Username", account.AccUserName);
                com.Parameters.AddWithValue("@Password", account.AccPassword);
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    string type = dr["AccType"].ToString();
                    Session["userId"] = dr["AccId"].ToString();
                    Session["userName"] = dr["AccUserName"].ToString();
                    if (type == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "AdminPage");
                    }
                    else if (type == "Purchaser")
                    {
                        return RedirectToAction("PurchasingPageRequest", "PurchasingPage");
                    }
                    else if (type == "Requestor")
                    {
                        return RedirectToAction("RequestorRequisition", "RequestorPage");
                    }
                    else if (type == "Custodian")
                    {
                        return RedirectToAction("CustodianQuotation", "CustodianPage");
                    }
                    else
                    {
                        return RedirectToAction("VerifyUser", "Home");
                    }
                }
                ViewBag.Message = "Invalid Account";
                return View("Login");
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Login");
            }        
        }
     
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Account acc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    connnectionString();
                    con.Open();
                    com.Connection = con;
                    com.CommandText = "insert into Account(AccFname,AccLname,AccMname,AccUserName,AccPassword)values(@UseraccountFname,@UseraccountLname,@UseraccountMname,@UseraccountUserName,@UseraccountPassword)";
                    com.Parameters.AddWithValue("@UseraccountFname", acc.AccFname);
                    com.Parameters.AddWithValue("@UseraccountLname", acc.AccLname);
                    com.Parameters.AddWithValue("@UseraccountMname", acc.AccMname);        
                    com.Parameters.AddWithValue("@UseraccountUserName", acc.AccUserName);
                    com.Parameters.AddWithValue("@UseraccountPassword", acc.AccPassword);
                    recordaffected = com.ExecuteNonQuery();
                    if(recordaffected > 0)
                    {
                        ViewBag.Message = "Registration successful!";
                        return View();
                    }
                    ViewBag.Message = "Invalid account!";
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.Message;
                    return View();
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
                ViewBag.Message = "Invalid account!";
                return View();
            }
           
        }
       
        public ActionResult VerifyUser()
        {

            return View();
        }
    }
}