using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;

namespace ERPSystems.Controllers
{
    public class HomeController : Controller
    {
        SqlCommand com = new SqlCommand();
        SqlConnection con = new SqlConnection();
        SqlDataReader dr;
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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyLogin(Account account)
        {
            connnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Useraccount where UseraccountUserName = '" + account.UserName + "' and UseraccountPassword = '" + account.UserPassword + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                string type = dr["UseraccountType"].ToString();
                if (type == "Admin")
                {
                    return RedirectToAction("AdminDashboard","AdminPage");
                }
                else
                {
                    return RedirectToAction("VerifyUser","Home");
                }                
            }
            else
            {
                ViewBag.Message = "Invalid Account";
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
                    com.CommandText = "insert into Useraccount(UseraccountFname,UseraccountLname,UseraccountMname,UseraccountPhone,UseraccountEmail,UseraccountUserName,UseraccountPassword)values(@UseraccountFname,@UseraccountLname,@UseraccountMname,@UseraccountPhone,@UseraccountEmail,@UseraccountUserName,@UseraccountPassword)";
                    com.Parameters.AddWithValue("@UseraccountFname", acc.UserFname);
                    com.Parameters.AddWithValue("@UseraccountLname", acc.UserLname);
                    com.Parameters.AddWithValue("@UseraccountMname", acc.UserMname);
                    com.Parameters.AddWithValue("@UseraccountPhone", acc.UserPhone);
                    com.Parameters.AddWithValue("@UseraccountEmail", acc.UserEmail);
                    com.Parameters.AddWithValue("@UseraccountUserName", acc.UserName);
                    com.Parameters.AddWithValue("@UseraccountPassword", acc.UserPassword);
                    com.ExecuteNonQuery();
                    ViewBag.Message = "Registration successful!";
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.Message = e.Message;
                    return View();
                }
            }
            return View();
        }
       
        public ActionResult VerifyUser()
        {

            return View();
        }
    }
}