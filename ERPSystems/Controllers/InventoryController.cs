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
using ERPSystems.Data;

namespace ERPSystem.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Inventory
        private string connString = ERPSystems.Properties.Resources.ConnectionString;
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

            return View("Inv_Index", sortResults.ToPagedList(pages ?? 1, 10));
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
            SelectList selectedProducts = new SelectList(products, "prod_Id", "prod_Name");
            ViewBag.prod = selectedProducts;

            return View("ItemForm");
        }

        [HttpPost]
        public ActionResult ProcessCreate(InventoryModel inventoryModel)
        {
            try
            {
                InventoryDAO inventoryDAO = new InventoryDAO();
                if (inventoryModel?.prod_Id == null || inventoryModel.prod_Id == 0)
                {
                    TempData["Error"] = "PRODUCT FIELD IS REQUIRED";
                    return RedirectToAction("Create");
                }

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
            return RedirectToAction("Inv_Index", items.ToList().ToPagedList(pages ?? 1, 10));
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
                TempData["AlertMessage"] = "Item Updated Successfully!";            
            }
            else
            {
                TempData["Error"] = "Inserted Data is Invalid";
                return RedirectToAction("Edit", inventoryModel);
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
                return View("Inv_Index", searchResults.ToList().ToPagedList(pages ?? 1, 10));
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

        public ActionResult ABCalgorithm()
        {
            //instantiate models to store and access values
            List<InventoryModel> products = new List<InventoryModel>();
            InventoryDAO inventoryDAO = new InventoryDAO();
            AlgorithmDAO algorithm = new AlgorithmDAO();

            products = inventoryDAO.GetAll();

            foreach(var item in products)
            {
                item.itemMRF = algorithm.calculateMRF(item); //calculate Monthy Restock Frequency
        
            }

            foreach (var item in products)
            {
                //get the Monthly Usage Quantity
                item.MonthlyFV = algorithm.getMonthlyUV(item.ProdUnitPrice, item.itemMRF);
            }

            // Sort products by ARF value using LinQ
            var sortItems = products.OrderBy(x => x.MonthlyFV).ToList();

            //get threshold
            int threshold20 = (int)(sortItems.Count * .20);
            int threshold50 = (int)(sortItems.Count * .50);

        
            //Assign Items threshold Items to Categories
            var CategoryA = sortItems.Take(threshold20).ToList();
            var CategoryB = sortItems.Skip(threshold20).Take(threshold50 - threshold20).ToList();
            var CategoryC = sortItems.Skip(threshold50).ToList();


            foreach(var item in CategoryA)
            {
                algorithm.AssignA(item.prod_Id, item.MonthlyFV);
            }
            foreach(var item in CategoryB)
            {
                algorithm.AssignB(item.prod_Id, item.MonthlyFV);
            }
            foreach(var item in CategoryC)
            {
                algorithm.AssignC(item.prod_Id, item.MonthlyFV);
            }

            return View("INV_INDEX");
        }
    }
}