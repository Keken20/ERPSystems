using ERPSystem.Data;
using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using ERPSystems.Data;
using ERPSystems.Models;

namespace ERPSystem.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product

        public ActionResult Index(string sortName, int? pages)
        {
            List<ViewModel.ProductViewModel> products = new List<ViewModel.ProductViewModel>();
            ProductDAO productDAO = new ProductDAO();

            if (sortName == "NONE" || sortName == null)
            {
                products = productDAO.ShowProducts();
            }
            else
            {
                products = productDAO.sortProduct(sortName);
            }

            ViewBag.CurrentProductSort = sortName;

            return View("Index", products.ToPagedList(pages ?? 1,10));
        }

        public ActionResult productCreate()
        {

            CategoryDAO categoryDAO = new CategoryDAO();
            SupplierDAO supplierDAO = new SupplierDAO();

            //Get and Set Data for the Dropdown Lists
            List<CategoryModel> category = categoryDAO.FetchAll();
            List<Supplier> supplist = supplierDAO.GetAll();

            SelectList categoryList = new SelectList(category, "categID", "CategoryName");
            SelectList supplierList = new SelectList(supplist, "SuppID", "SuppName");

            ViewBag.categories = categoryList;
            ViewBag.suppliers = supplierList;
            
            return View("productForm");
        }

        [HttpPost]
        public ActionResult ProcessCreate(ProductModel productModel, int? pages)
        {
            ProductDAO productDAO = new ProductDAO();

            if (string.IsNullOrWhiteSpace(productModel.prod_Name) || string.IsNullOrEmpty(productModel.prod_Name))
            {
                TempData["Error"] = "PRODUCT NAME IS REQUIRED";
                return RedirectToAction("productCreate");
            }

            var check = productDAO.checkEntry(productModel.prod_Name); //check existing product

            if (!check)
            { 
                int row = productDAO.CreateProduct(productModel);
                if(row > 0)
                {
                    TempData.Clear();
                    ModelState.Clear();

                    TempData["Success"] = "Product Successfully Created";
                    return RedirectToAction("ProductCreate");
                }
                else
                {

                    TempData["Error"] = "Data Insert is Invalid!!";
                    return RedirectToAction("productCreate");
                }          
            }
            else
            {
                TempData["Error"] = "Product Already Existed !!";
                return RedirectToAction("productCreate");
            }  
                    
        }


        public ActionResult Delete(int ID)
        {          
            ProductDAO productDAO = new ProductDAO();
            productDAO.itemDelete(ID);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? ID)
        {
            if (!ID.HasValue)
            {
                TempData["AlertMessage"] = "No Product Selected";
                return RedirectToAction("Index");

            }
            CategoryDAO categoryDAO = new CategoryDAO();
            SupplierDAO supplierDAO = new SupplierDAO();

            //Get and Set Data for the DropDownList
            List<CategoryModel> category = categoryDAO.FetchAll();
            List<Supplier> supplist = supplierDAO.GetAll();

            ViewBag.categories = new SelectList(category, "categID", "CategoryName");
            ViewBag.suppliers = new SelectList(supplist, "SuppID", "SuppName");

            ProductDAO productDAO = new ProductDAO();
            ProductModel products = productDAO.FetchOne((int)ID);

            return View("Edit", products);
        }

        [HttpPost]
        public ActionResult ProcessEdit(ProductModel productModel)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.UpdateProduct(productModel);
            return RedirectToAction("Index");
        }

        public ActionResult Search(string searchPhrase, int? pages)
        {
            ProductDAO productDAO = new ProductDAO();
            if (string.IsNullOrWhiteSpace(searchPhrase) || string.IsNullOrEmpty(searchPhrase))
            {
                return RedirectToAction("Index");
            }
            else
            {
                List<ViewModel.ProductViewModel> searchResults = productDAO.SearchKey(searchPhrase);
                return View("Index", searchResults.ToPagedList(pages ?? 1, 10));
            }
          
        }

        public ActionResult Reactivate(int ID, int? pages, string sortName)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.productActivation(ID);
             
            //Pass the current sort action and current page state to the index
            return RedirectToAction("Index", new { sortName, pages });
        }

        public ActionResult productForm()
        {
            return RedirectToAction("productCreate");
        }
    }
}