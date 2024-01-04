using ERPSystem.Data;
using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERPSystem.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            List<CategoryModel> category = new List<CategoryModel>();
            CategoryDAO categoryDAO = new CategoryDAO();
            category = categoryDAO.FetchAll();
            return View("Index", category);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ProcessCreate(CategoryModel categoryModel)
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            int rows = categoryDAO.CreateCategory(categoryModel);
            if (rows > 0)
            {
                TempData["Success"] = "Item Added!";
                ModelState.Clear();
                ModelState.AddModelError("prod_Id", "Product already exists.");
                return RedirectToAction("Create");
            }
            else
            {
                TempData["Error"] = "Inserted Data is Invalid";
                return View("Create");
            }
        }

        public ActionResult Edit(int id)
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            CategoryModel categ = categoryDAO.FetchOne(id);

            return View("Edit", categ);
        }

        [HttpPost]
        public ActionResult ProcessEdit(CategoryModel categoryModel)
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            int rows = categoryDAO.UpdateCategory(categoryModel);
            List<CategoryModel> catlist = categoryDAO.FetchAll();
            if (rows > 0)
            {
                TempData["Success"] = "Item Updated!";
                return RedirectToAction("Index", catlist);
            }
            else
            {
                TempData["Error"] = "Update Data Failed";
                return View("Edit", categoryModel);
            }


        }

        public ActionResult Delete(int ID)
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            categoryDAO.DeleteCateg(ID);

            List<CategoryModel> catlist = categoryDAO.FetchAll();
            return RedirectToAction("Index", catlist);
        }

    }
}