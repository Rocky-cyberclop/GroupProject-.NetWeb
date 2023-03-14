using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork;

namespace GroupProject.Areas.Admin.Controllers
{
    public class ManagementController : Controller
    {
        HaoDatabase db = new HaoDatabase();
        // GET: Admin/Managament
        public ActionResult Index()
        {
            return View();
        }

        //Employees management
        [HttpGet, ActionName("Staffs")]
        public ActionResult StaffsList()
        {
            var listStaff = from ts in db.NhanViens select ts;
            return View(listStaff);
        }

        [HttpGet, ActionName("AddStaff")]
        public ActionResult AddStaff() {
            var listStaff = from ts in db.NhanViens select ts;
            ViewBag.count = listStaff.Count()+1;

            return View();
        }

        [HttpPost, ActionName("AddStaff")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddStaff(NhanVien model)
        {
            var listStaff = from ts in db.NhanViens select ts;
            model.MaNV = "NV0" + (listStaff.Count() + 1).ToString();
            model.MatKhau = "NV0" + (listStaff.Count() + 1).ToString();

            db.NhanViens.Add(model);
            db.SaveChanges();

            return RedirectToAction("Staffs");
        }

        [HttpGet, ActionName("EditStaff")]
        public ActionResult EditStaff(string id)
        {
            var staff = from ss in db.NhanViens where ss.MaNV==id select ss;
            return View(staff.FirstOrDefault());
        }

        [HttpPost, ActionName("EditStaff")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditStaffSave(string id)
        {
            var staff = db.NhanViens.Find(id);
            TryUpdateModel(staff, "", new string[] { "Ten", "Email", "DiaChi", "DienThoai", "Quyen" });
            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Staffs");
        }

        //Products management
        [HttpGet, ActionName("Products")]
        public ActionResult ProductsList()
        {
            var listProduct = from ps in db.SanPhams select ps;
            return View(listProduct);
        }

        [HttpGet, ActionName("CreateProduct")]
        public ActionResult CreateProduct()
        {
            return View();
        }
    }
}