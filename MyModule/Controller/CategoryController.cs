using ERPSystem.Data;
using ERPSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace ERPSystem.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index(int? pages, string sortName)
        {
            List<CategoryModel> category = new List<CategoryModel>();
            CategoryDAO categoryDAO = new CategoryDAO();
            if (sortName == "NONE" || sortName == null)
            {
                category = categoryDAO.FetchAll();
            }
            else
            {
                category = categoryDAO.CategorySort(sortName);
            }
              
            ViewBag.CurrentSort = sortName;

            return View("Index", category.ToPagedList(pages ?? 1, 10));
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ProcessCreate(CategoryModel categoryModel)
        {
            CategoryDAO categoryDAO = new CategoryDAO();
            var checkExist = categoryDAO.checkCategory(categoryModel.CategoryName);
            if (!checkExist)
            {
                int rows = categoryDAO.CreateCategory(categoryModel);
                if (rows > 0)
                {

                    TempData["Success"] = "Item Added!";
                    ModelState.Clear();
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Error"] = "Inserted Data is Invalid";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                TempData["Error"] = "Data Already Existed!!";
                return RedirectToAction("Create");
            }
          
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                TempData["AlertMessage"] = "No Category Selected";
                return RedirectToAction("Index");
            }
            CategoryDAO categoryDAO = new CategoryDAO();
            CategoryModel categ = categoryDAO.FetchOne((int)id);

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

        public ActionResult categReactivate(int ID, int? pages, string sortName)
        {
            ProductDAO productDAO = new ProductDAO();
            productDAO.productActivation(ID);

            //Pass the current sort action and current page state to the index
            return RedirectToAction("Index", new { sortName, pages });
        }
    }
}