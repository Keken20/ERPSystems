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
        private string connString = ERPSystems.Properties.Resources.ConnectionString;
        public ActionResult Index(int? pages, string sortName)
        {
            try
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
            catch(Exception)
            {
                return ErrorHandling();
            }
     
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ProcessCreate(CategoryModel categoryModel)
        {
            try
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
            catch (Exception)
            {
                return ErrorHandling();
            }
          
          
        }

        public ActionResult Edit(int? id)
        {
            try
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
            catch (Exception)
            {

                return ErrorHandling();
            }
       
        }

        [HttpPost]
        public ActionResult ProcessEdit(CategoryModel categoryModel)
        {
            try
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
            catch(Exception)
            {

                return ErrorHandling();
            }
           


        }

        public ActionResult Delete(int ID, int? pages)
        {
            try
            {
                CategoryDAO categoryDAO = new CategoryDAO();
                categoryDAO.DeleteCateg(ID);

                List<CategoryModel> catlist = categoryDAO.FetchAll();
                return View("Index", catlist.ToPagedList(pages ?? 1, 10));
            }
            catch (Exception)
            {

                return ErrorHandling();
              
            }
        }

        public ActionResult categReactivate(int ID, int? pages, string sortName)
        {
            try
            {
                CategoryDAO categoryDAO = new CategoryDAO();
                int rows = categoryDAO.categActivation(ID);
                if (rows > 0)
                {
                    TempData["AlertMessage"] = "Data Re-Activated";
                }
                else
                {
                    TempData["AlertMessage"] = "Re-Activation Failed";
                }
                return RedirectToAction("Index", new { sortName, pages });

            }
            catch
            {
                return ErrorHandling();
            }
 
        }

        private ActionResult ErrorHandling()
        {
            ViewBag.ErrorMessage = "An error occurred. Please try again later.";
            return RedirectToAction("Index");
        }
    }
}