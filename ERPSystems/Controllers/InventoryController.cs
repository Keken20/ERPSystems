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
        public ActionResult Index(int? pages)
        {
            List<ViewModel> items = new List<ViewModel>();
            InventoryDAO inventDAO = new InventoryDAO();
            items = inventDAO.FetchAll();
            return View("Index", items.ToList().ToPagedList(pages ?? 1, 3));
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
            InventoryDAO inventoryDAO = new InventoryDAO();
            int rows = inventoryDAO.CreateItem(inventoryModel);
            if(rows > 0)
            {
                TempData["Success"] = "Item Added!";
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Inserted Data is Invalid";
                return View("ItemForm");
            }
           
        }

        public ActionResult Delete(int ID, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            inventoryDAO.itemDelete(ID);

            List<ViewModel> items = inventoryDAO.FetchAll();
            return RedirectToAction("Index", items.ToList().ToPagedList(pages ?? 1, 3));
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
            return RedirectToAction("Index", items.ToList().ToPagedList(pages ?? 1, 3));
        }

        public ActionResult Search(string searchPhrase, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> searchResults = inventoryDAO.SearchID(searchPhrase);
            return View("Index", searchResults.ToList().ToPagedList(pages ?? 1,3));
        }


        public ActionResult SortBy(string sortName, string sortType, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> sortResults = inventoryDAO.ItemSort(sortName, sortType);
            return View("Index", sortResults);
        }
    }
}