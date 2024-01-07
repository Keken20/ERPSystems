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
        public ActionResult Inv_Index(string sortName, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            List<ViewModel> sortResults = new List<ViewModel>();

            string MsgeResult = TempData["ResultMsge"] as string;

            if (sortName == "NONE" || sortName == null)
            {
                sortResults = inventoryDAO.FetchAll();
            }
            else
            {
                sortResults = inventoryDAO.ItemSort(sortName);
            }

            ViewBag.CurrentSort = sortName;  //Store the current selected data from sorted droplist

            return View("Inv_Index", sortResults.ToPagedList(pages ?? 1, 2));
        }


        public ActionResult Create()
        {
            ProductDAO productDAO = new ProductDAO();
            InventoryDAO inventoryDAO = new InventoryDAO();
            
            List<ProductModel> getproducts = productDAO.FetchAll();
            List<ProductModel> products = new List<ProductModel>(); //stores no existing products

            foreach (var item in getproducts)
            {

                if(!inventoryDAO.checkExisting(item.prod_Id))
                {
                    ProductModel prod = productDAO.FetchOne(item.prod_Id);
                    products.Add(prod);
                }

            }
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
                    return RedirectToAction("Inv_Index");
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
            return RedirectToAction("Inv_Index", items.ToList().ToPagedList(pages ?? 1, 15));
        }

        public ActionResult Edit(int? ID)
        {
            if (!ID.HasValue)
            {
                TempData["AlertMessage"] = "No Item Selected";
                return RedirectToAction("Inv_Index");
            }

            ProductDAO productDAO = new ProductDAO();
            InventoryDAO inventoryDAO = new InventoryDAO();

            List<ProductModel> products = productDAO.FetchAll();

            SelectList productSelectList = new SelectList(products, "prod_Id", "prod_Name", ID);
            ViewBag.prod = productSelectList;

            InventoryModel item = inventoryDAO.FetchOne((int)ID);
            return View("Edit", item);
        }

        [HttpPost]
        public ActionResult ProcessEdit(InventoryModel inventoryModel, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            int rows = inventoryDAO.UpdateItem(inventoryModel);
            if (rows > 0)
            {
                TempData["Success"] = "Item Updated Successfully!";            
            }
            else
            {
                TempData["Error"] = "Inserted Data is Invalid";
                return View("Edit", inventoryModel);
            }
            return RedirectToAction("Inv_Index");
        }

        public ActionResult Search(string searchPhrase, int? pages)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            if(searchPhrase ==" " || string.IsNullOrEmpty(searchPhrase))
            {
                return RedirectToAction("Inv_Index");
            }
            else
            {
                List<ViewModel> searchResults = inventoryDAO.SearchID(searchPhrase);
                return View("Inv_Index", searchResults.ToList().ToPagedList(pages ?? 1, 15));
            }      
        }

        public ActionResult Reactivate(int ID, int? pages, string sortName)
        {
            InventoryDAO inventoryDAO = new InventoryDAO();
            inventoryDAO.itemReactivation(ID);

            //Pass the current sort action and current page state to the Inv_Index
            return RedirectToAction("Inv_Index", new { sortName, pages });
        }

        public ActionResult ItemForm()
        {
            return RedirectToAction("Create");
        }


    }
}