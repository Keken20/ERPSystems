using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystem.Models;
using System.Data.SqlClient;
using ERPSystem.Data;

namespace ERPSystem.Controllers
{
    public class RequestorPageController : Controller
    {
        // GET: RequestorPage
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

        public ActionResult RequestorDashboard(RequestModel model)
        {
            FetchRequest();
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

        public ActionResult AddRequisition()
        {
            int id = Convert.ToInt32(Session["userId"]);

            RequestorDAO rDAO = new RequestorDAO();

            rDAO.addRequestForm(id);

            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            return View("AddRequisition");
        }


        [HttpPost]
        public ActionResult AddRequisitionItem(ItemRequestModel itemlist)
        {
            RequestForm reqForm = new RequestForm();
          
            RequestorDAO rDAO = new RequestorDAO();
            
            var requestId = rDAO.getReqId();

            foreach (var item in itemlist.reqItems)
            {
               rDAO.addRequestItem(item, requestId); //insert data
            }

            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            RequestModel model = new RequestModel();

            FetchRequest();

            model.requestform = requestForm;

            return View("RequestorDashboard", model);
        }

         public ActionResult UpdateRequisition(RequestModel model)
        {
            FetchRequest();
            model.requestform = requestForm;

            ProductDAO prodDAO = new ProductDAO();

            return View("UpdateRequisition", model);
        }

        public ActionResult UpdateRequisitionForm(int id)
        {
            ViewBag.RequestId = id;
            ProductDAO prodDAO = new ProductDAO();

            List<ProductModel> prodList = prodDAO.FetchAll();

            ViewBag.returnList = new SelectList(prodList, "prod_Id", "prod_Name");

            RequestorDAO rDAO = new RequestorDAO();
         
            List<RequestItem> reqItem = rDAO.FetchItems(id);
            ItemRequestModel requests = new ItemRequestModel { reqItems = reqItem };

            //reqItem = rDAO.GetOne(id);

            return View("UpdateRequisitionForm", requests);
        }

        [HttpPost]
        public ActionResult UpdateRequisitionItem(List<RequestItem> reqItem)
        {
            RequestorDAO rDAO = new RequestorDAO();

            foreach(var item in reqItem)
            {
                rDAO.updateRequestItem(item);
            }

            Response.Write("<script>alert('Updated Successfully');</script>");

            RequestModel model = new RequestModel();

            FetchRequest();

            model.requestform = requestForm;

            return View("UpdateRequisition", model);
        }

        public ActionResult DeleteRequisitionForm(int id)
        {
            RequestorDAO rDAO = new RequestorDAO();

            Response.Write("<script>alert('Delete Successfully');</script>");

            rDAO.deleteRequest(id);

            RequestModel model = new RequestModel();

            FetchRequest();

            model.requestform = requestForm;

            return View("DeleteRequisition", model);
        }

        public ActionResult DeleteRequisition(RequestModel model)
        {
            FetchRequest();
            model.requestform = requestForm;
            return View(model);
        }


        public ActionResult RequestorRequisition()
        {
            return View();
        }
        public ActionResult RequestorInventory()
        {
            return View();
        }
    }
}