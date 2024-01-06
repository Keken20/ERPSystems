using ERPSystem.Data;
using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace ERPSystem.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        public ActionResult Index(string sortName, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> sortResults = new List<ViewModel>();

            string MsgeResult = TempData["ResultMsge"] as string;
            
            if (sortName == "NONE" || sortName == null)
                sortResults = inventoryDAO.FetchAll();
            else
                sortResults = inventoryDAO.ItemSort(sortName);

            ViewBag.CurrentSort = sortName;  //Store the current selected data from sorted droplist

            return View("Index", sortResults.ToPagedList(pages ?? 1, 2));
 
        }

        public ActionResult NonAdminIndex(string sortName, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> sortResult = new List<ViewModel>();

            string MsgeResult = TempData["ResultMsge"] as string;

            if (sortName == "NONE" || sortName == null)
                sortResult = inventoryDAO.FetchAll();
            else
                sortResult = inventoryDAO.ItemSort(sortName);

            ViewBag.CurrentSort = sortName;  


            return View("NonAdminIndex", sortResult.ToPagedList(pages ?? 1, 2));
        }

        public ActionResult Create()
        {
            ProductDAO productDAO = new ProductDAO();
            List<ProductModel> products = productDAO.FetchAll();
            ViewBag.prod = new SelectList(products, "prod_Id", "prod_Name");

            return View("ItemForm");
        }

        [HttpPost]
        public ActionResult ProcessCreate(InventoryModel inventoryModel)
        {
            try
            {
                InventoryDAO inventoryDAO = new InventoryDAO();
                int rows = inventoryDAO.CreateItem(inventoryModel);
                if (rows > 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Inserted Data is Invalid";
                    return View("ItemForm");
                }
            }
            catch(Exception)
            {
                TempData["Error"] = "Data Already Existed!";
                return View("ItemForm");
            }     
        }

        public ActionResult Delete(int ID, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            TempData["ResultMsge"] = inventoryDAO.itemDelete(ID); //get result

            List<ViewModel> items = inventoryDAO.FetchAll();
            return RedirectToAction("Index", items.ToList().ToPagedList(pages ?? 1, 15));
        }

        public ActionResult Edit(int ID)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            InventoryModel item = inventoryDAO.FetchOne(ID);
            return View("Edit", item);
        }

        [HttpPost]
        public ActionResult ProcessEdit(InventoryModel inventoryModel, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            int rows = inventoryDAO.UpdateItem(inventoryModel);
            List<ViewModel> items = inventoryDAO.FetchAll();
            if (rows > 0)
            {
                TempData["Success"] = "Item Updated!";            
            }
            else
            {
                TempData["Error"] = "Inserted Data is Invalid";
                return View("Edit", inventoryModel);
            }
            return RedirectToAction("Index", items.ToList().ToPagedList(pages ?? 1, 15));
        }

        public ActionResult Search(string searchPhrase, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            if(searchPhrase =="" || string.IsNullOrEmpty(searchPhrase))
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<ViewModel> searchResults = inventoryDAO.SearchID(searchPhrase);
                return View("Index", searchResults.ToList().ToPagedList(pages ?? 1, 15));
            }      
        }

        public ActionResult Reactivate(int ID, int? pages, string sortName)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            inventoryDAO.itemReactivation(ID);

            //Pass the current sort action and current page state to the index
            return RedirectToAction("Index", new { sortName, pages });
        }


    }
}