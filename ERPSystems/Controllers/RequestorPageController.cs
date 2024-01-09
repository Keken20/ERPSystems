using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using System.Data;
using ERPSystem.Data;
using ERPSystem.Models;

namespace ERPSystems.Controllers
{
    public class RequestorPageController : Controller
    {
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

        public ActionResult RequestorDashboard()
        {
            int userid = Convert.ToInt32(Session["userId"]);

            FetchRequest(userid);

            return View();
        }

        //public ActionResult FetchAccountID(int id)
        //{
        //    connnectionString();
        //    try
        //    {
        //        con.Open();
        //        com.Connection = con;
        //        com.CommandText = "select * From ACCOUNT WHERE ACCID = @id";
        //        com.Parameters.AddWithValue("@id", id);
        //        dr = com.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            userAccounts.Add(new UserAccount
        //            {
        //                UserID = int.Parse(dr["AccId"].ToString())
        //            });
        //        }
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View(userAccounts);
        //}

        public ActionResult FetchRequest(int id)
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
                com.CommandText = "select * From RequisitionForm inner join Account on RequisitionForm.AccId = Account.AccId WHERE REQISACTIVE = 1 AND REQUISITIONFORM.ACCID = @id ";
                com.Parameters.AddWithValue("@id", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    requestForm.Add(new RequestForm
                    {
                        RequestId = int.Parse(dr["ReqId"].ToString())
                    ,
                        RequestDate = DateTime.Parse(dr["ReqCreatedAt"].ToString())
                    ,
                        TotalItem = dr["ReqTotalItem"] != DBNull.Value ? Convert.ToInt32(dr["ReqTotalItem"]) : 0
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

        public ActionResult FetchRequestPending(int id)
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
                com.CommandText = "select * From RequisitionForm inner join Account on RequisitionForm.AccId = Account.AccId WHERE REQISACTIVE = 1 AND REQSTATUS = 'Pending' AND REQUISITIONFORM.ACCID = @id ";
                com.Parameters.AddWithValue("@id", id);
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    requestForm.Add(new RequestForm
                    {
                        RequestId = int.Parse(dr["ReqId"].ToString())
                    ,
                        RequestDate = DateTime.Parse(dr["ReqCreatedAt"].ToString())
                    ,
                        TotalItem = dr["ReqTotalItem"] != DBNull.Value ? Convert.ToInt32(dr["ReqTotalItem"]) : 0
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

        public ActionResult RequestorRequisition(RequestModel model)
        {
            int userid = Convert.ToInt32(Session["userId"]);
            FetchRequest(userid);
            model.requestform = requestForm;
            return View(model);
        }

        public ActionResult AddRequisition(RequestModel model)
        {
            int userid = Convert.ToInt32(Session["userId"]);

            RequestorDAO rDAO = new RequestorDAO();

            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            rDAO.addRequestForm(userid);

            int reqId = rDAO.getReqId();

            ItemRequestModel itemModel = new ItemRequestModel();

            itemModel.reqItems = rDAO.GetOne(reqId);

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            return View("AddRequisition", itemModel);
        }

        [HttpPost]
        public ActionResult AddRequisitionItem(ItemRequestModel reqItem) // try cats
        {
            int userid = Convert.ToInt32(Session["userId"]);

            RequestorDAO rDAO = new RequestorDAO();

            int rID = reqItem.reqItems[0].RequestId;

            foreach (var item in reqItem.reqItems)
            {

                rDAO.addRequestItem(item, rID);
            }

            Response.Write("<script>alert('Added Successfully');</script>");

            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            RequestModel model = new RequestModel();

            model.requestform = requestForm;

            FetchRequest(userid);

            model.requestform = requestForm;

            return View("RequestorRequisition", model);
        }

        public ActionResult UpdateRequisition(RequestModel model)
        {
            int userid = Convert.ToInt32(Session["userId"]);

            FetchRequestPending(userid);

            model.requestform = requestForm;

            //ProductDAO prodDAO = new ProductDAO();

            return View("UpdateRequisition", model);
        }

        public ActionResult UpdateRequisitionForm(int id)
        {
            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            RequestorDAO rDAO = new RequestorDAO();

            ItemRequestModel itemModel = new ItemRequestModel();

            itemModel.reqItems = rDAO.GetOne(id);

            return View("UpdateRequisitionForm", itemModel);
        }

        [HttpPost]
        public ActionResult UpdateRequisitionItem(ItemRequestModel reqItem)
        {
            RequestorDAO rDAO = new RequestorDAO();

            int rID = reqItem.reqItems[0].RequestId;

            if (rDAO.verify(rID) >= 1)
            {
                foreach (var item in reqItem.reqItems)
                {
                    rDAO.updateRequestItem(item, rID);
                }
            }
            else
            {
                foreach (var item in reqItem.reqItems)
                {
                    rDAO.addRequestItem(item, rID);
                }
            }

            Response.Write("<script>alert('Updated Successfully');</script>");

            RequestModel model = new RequestModel();

            int userid = Convert.ToInt32(Session["userId"]);

            FetchRequest(userid);

            model.requestform = requestForm;

            return View("UpdateRequisition", model);
        }

        public ActionResult DeleteRequisitionForm(int id)
        {
            RequestorDAO rDAO = new RequestorDAO();

            Response.Write("<script>alert('Delete Successfully');</script>");

            rDAO.deleteRequest(id);

            RequestModel model = new RequestModel();

            int userid = Convert.ToInt32(Session["userId"]);

            FetchRequest(userid);

            model.requestform = requestForm;

            return View("DeleteRequisition", model);
        }

        public ActionResult DeleteRequisition(RequestModel model)
        {
            try
            {
                int userid = Convert.ToInt32(Session["userId"]);
                FetchRequest(userid);
                model.requestform = requestForm;
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}