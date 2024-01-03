using ERPSystem.Data;
using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace ERPSystem.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(int? pages)
        {
            List<ViewModel.ProductViewModel> products = new List<ViewModel.ProductViewModel>();
            ProductDAO productDAO = new ProductDAO();
            products = productDAO.ShowProducts();

            return View("Index", products.ToPagedList(pages ?? 1,5));
        }
        public ActionResult productCreate()
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            SupplierDAO supplierDAO = new SupplierDAO();
            List<CategoryModel> category = categoryDAO.FetchAll();
            List<Supplier> supplist = supplierDAO.GetAll();
            ViewBag.categories = new SelectList(category, "categID", "CategoryName");
            ViewBag.suppliers = new SelectList(supplist, "SuppID", "SuppName");
            
            return View("Create");
        }

        [HttpPost]
        public ActionResult ProcessCreate(ProductModel productModel, int? pages)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.CreateProduct(productModel);

            List<ViewModel.ProductViewModel> products = new List<ViewModel.ProductViewModel>();
            products = productDAO.ShowProducts();
            return RedirectToAction("Index");
        }


        public ActionResult Delete(int ID)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.itemDelete(ID);
            List<ViewModel.ProductViewModel> products = productDAO.ShowProducts();

            return View("Index", products);
        }

        public ActionResult Edit(int ID)
        {
            ProductDAO productDAO = new ProductDAO();
            ProductModel products = productDAO.FetchOne(ID);
            return View("Edit", products);
        }

        [HttpPost]
        public ActionResult ProcessEdit(ProductModel productModel)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.UpdateProduct(productModel);

            List<ProductModel> products = new List<ProductModel>();
            products = productDAO.FetchAll();
            return RedirectToAction("Index", products);
        }

        public ActionResult Search(string searchPhrase, int? pages)
        {
            ProductDAO productDAO = new ProductDAO();
            List<ViewModel.ProductViewModel> searchResults = productDAO.SearchKey(searchPhrase);
            return View("Index", searchResults.ToPagedList(pages ?? 1,5));
        }

    }
}