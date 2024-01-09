using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERPSystems.Models;
using System.Data.SqlClient;
using System.Data;
using PagedList;
using PagedList.Mvc;
using ERPSystems.Data;
using System.Text.RegularExpressions;

namespace ERPSystems.Controllers
{
    public class SupplierController : Controller
    {
        // GET: Supplier
        Regex regexPhone = new Regex(@"\d+(?:-\d+)*$");
        Regex regexZipcode = new Regex(@"^\d{4}$");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddSupplier(Supplier supplier)
        {
            return View();
        }

        public ActionResult SupplierMain(int? page, string sortId)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            ViewBag.SortId = sortId;

            if (sortId == null || sortId == "")
            {
                supplierList = supplierDAO.GetAllActive();
            }
            else
            {
                supplierList = supplierDAO.SortBy(sortId);
            }

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            //ViewBag.currentList = new SelectList(returnList, "prod_Id", "prod_Name");

            return View("SupplierMain", returnList);
        }

        public ActionResult UpdateSupplier(int? page)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.GetAllActive();

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("UpdateSupplier", returnList);
        }

        public ActionResult DeleteSupplier(int? page)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.GetAll();

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("DeleteSupplier", returnList);
        }

        [HttpPost]
        public ActionResult ProcessCreate(Supplier supplier, int? page)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            if (string.IsNullOrWhiteSpace(supplier.SuppName) || string.IsNullOrWhiteSpace(supplier.SuppContact) || string.IsNullOrWhiteSpace(supplier.SuppCity) || string.IsNullOrWhiteSpace(supplier.SuppMunicipality) || string.IsNullOrWhiteSpace(supplier.SuppBarangay) || string.IsNullOrWhiteSpace(supplier.SuppZipcode))
            {
                TempData["ShowAlert"] = "Fields must not be empty";
            }
            else
            {
                Supplier supplist = supplierDAO.getPhone(supplier); //Verify if phone is already in db
                string supcon1 = supplier.SuppContact.Replace(" ", "");
                string supcon2 = supplist.SuppContact.Replace(" ", "");

                if (supcon1 == supcon2) //Check if phone is already in db
                {
                    TempData["Msg"] = "Contact is already taken";
                }
                else
                {
                    if (regexPhone.IsMatch(supplier.SuppContact) != true)
                    {
                        TempData["ValidationPhone"] = "Invalid format";

                    }
                    else if (regexZipcode.IsMatch(supplier.SuppZipcode) != true)
                    {
                        TempData["ValidationZip"] = "Invalid format";
                    }
                    else
                    {
                        supplierDAO.add(supplier);

                        supplierList = supplierDAO.GetAllActive();

                        var returnList = supplierList.ToPagedList(page ?? 1, 5);

                        TempData["ShowAlert"] = "Added Successfully";

                        return RedirectToAction("Supplier", "AdminPage", returnList);
                    }
                }
            }

            return View("AddSupplier");
        }

        public ActionResult ProcessReactivate(int? page, int id)
        {
            SupplierDAO supplierDAO = new SupplierDAO();

            supplierDAO.reactivate(id);

            TempData["ShowAlert"] = "Supplier Reactivated!";

            List<Supplier> supplierList = new List<Supplier>();

            supplierList = supplierDAO.GetAll();

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return RedirectToAction("DeleteSupplier", "Supplier", returnList);
        }

        public ActionResult ProcessDelete(int? page, int id)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            TempData["ShowAlert"] = "Supplier Deactivated!";

            supplierDAO.delete(id);

            supplierList = supplierDAO.GetAll();

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("DeleteSupplier", returnList);
        }

        [HttpPost]
        public ActionResult ProcessUpdate(int? page, Supplier supplier)
        {
            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            if (string.IsNullOrWhiteSpace(supplier.SuppName) || string.IsNullOrWhiteSpace(supplier.SuppContact) || string.IsNullOrWhiteSpace(supplier.SuppCity) || string.IsNullOrWhiteSpace(supplier.SuppMunicipality) || string.IsNullOrWhiteSpace(supplier.SuppBarangay) || string.IsNullOrWhiteSpace(supplier.SuppZipcode))
            {
                Response.Write("<script>alert('Fields must not be empty');</script>");
            }
            else
            {
                Supplier supplist = supplierDAO.getPhone(supplier); //Verify if phone is already in db
                string supcon1 = supplier.SuppContact.Replace(" ", "");
                string supcon2 = supplist.SuppContact.Replace(" ", "");

                if ((supcon1 == supcon2 && supplier.SuppID == supplist.SuppID) || supplist.SuppID == -1) //Validates the phone and passes on two conditions: 
                {
                    if (regexPhone.IsMatch(supplier.SuppContact) != true)
                    {
                        TempData["ValidationPhone"] = "Invalid format";

                    }
                    else if (regexZipcode.IsMatch(supplier.SuppZipcode) != true)
                    {
                        TempData["ValidationZip"] = "Invalid format";
                    }
                    else
                    {
                        supplierDAO.update(supplier);

                        supplierList = supplierDAO.GetAllActive();

                        var returnList = supplierList.ToPagedList(page ?? 1, 5);

                        TempData["ShowAlert"] = "Updated Successfully";

                        return RedirectToAction("UpdateSupplier", "Supplier", returnList);
                    }
                }
                else
                {
                    TempData["Msg"] = "Contact is already taken";
                }
            }

            return View("UpdateSupplierForm");
        }

        [HttpPost]
        public ActionResult ProcessSearch(string searchId, int? page)
        {
            Supplier supplier = new Supplier();

            List<Supplier> supplierList = new List<Supplier>();

            if (searchId == "")
            {
                Response.Write("<script>alert('Fields must not be empty');</script>");

                SupplierDAO supplierDAO = new SupplierDAO();

                supplierList = supplierDAO.GetAllActive();
            }
            else
            {
                SupplierDAO supplierDAO = new SupplierDAO();

                supplierList = supplierDAO.SearchName(searchId);
            }

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

            return View("SupplierMain", returnList);
        }

        [HttpPost]
        public ActionResult ProcessSort(string sortId, int? page)
        {
            Supplier supplier = new Supplier();

            List<Supplier> supplierList = new List<Supplier>();

            SupplierDAO supplierDAO = new SupplierDAO();

            supplierList = supplierDAO.SortBy(sortId);

            var returnList = supplierList.ToPagedList(page ?? 1, 5);

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
