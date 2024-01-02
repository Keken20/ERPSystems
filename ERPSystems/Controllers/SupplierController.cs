using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using ERPSystem.Models;
using ERPSystem.Data;
using PagedList;
using PagedList.Mvc;

namespace ERPSystem.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Supplier

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSupplier(Supplier supplier)
        {
            return View();
        }

        public ActionResult SupplierMain(int? page)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.GetAll();

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("SupplierMain", returnList);
        }

        public ActionResult UpdateSupplier()
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.GetAll();

            return View("UpdateSupplier", supplierList);
        }
        
        public ActionResult DeleteSupplier()
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.GetAll();

            return View("DeleteSupplier", supplierList);
        }

        [HttpPost]
        public ActionResult ProcessCreate(Supplier supplier)
        {
            if (supplier.SuppName == "" || supplier.SuppContact == "" || supplier.SuppCity == "" || supplier.SuppMunicipality == "" || supplier.SuppBarangay == "" || supplier.SuppZipcode == "")
            {
                //ModelState.AddModelError("SupplierEmail", "Fields must not be empty");
                TempData["Msg"] = "Fields must not be empty";
            }
            else
            {
                SupplierDAO supplierDAO = new SupplierDAO();
                supplierDAO.add(supplier);

                TempData["Msg"] = "Added Successfully!";
            }

            return View("AddSupplier");
        }

        public ActionResult ProcessDelete(int id)
        {
            List<Supplier> supplierList = new List<Supplier>();

            //Supplier supplier = new Supplier();

            SupplierDAO supplierDAO = new SupplierDAO();

            TempData["Msg"] = "Deleted Successfully!";
            supplierDAO.delete(id);

            supplierList = supplierDAO.GetAll();

            return View("DeleteSupplier", supplierList);
        }

        [HttpPost]
        public ActionResult ProcessUpdate(Supplier supplier)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            if (supplier.SuppName == "" || supplier.SuppContact == "" || supplier.SuppCity == "" || supplier.SuppMunicipality == "" || supplier.SuppBarangay == "" || supplier.SuppZipcode == "")
            {
                TempData["Msg"] = "Fields must not be empty";
            }
            else
            {
                supplierDAO.update(supplier);

                TempData["Msg"] = "Updated Successfully!";
            }

            supplierList = supplierDAO.GetAll();;

            return View("UpdateSupplier", supplierList);
        }

        [HttpPost]
        public ActionResult ProcessSearch(string searchId, int? page)
        {
            Supplier supplier = new Supplier();

            List<Supplier> supplierList = new List<Supplier>();

            if (searchId == "")
            {
                TempData["Msg"] = "Fields must not be empty";
            }
            else
            {
                SupplierDAO supplierDAO = new SupplierDAO();

                supplierList = supplierDAO.SearchName(searchId);
            }

            var returnList = supplierList.ToPagedList(page ?? 1, 2);

            return View("SupplierMain", returnList);
        }

        public ActionResult UpdateSupplierForm(int id)
        {
            SupplierDAO supplierDAO = new SupplierDAO();
            Supplier supplier = new Supplier();
            supplier = supplierDAO.GetOne(id);
            return View("UpdateSupplierForm", supplier);
        }
    }
}