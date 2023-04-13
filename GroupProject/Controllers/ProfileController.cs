using DAL.FrameWork;
using GroupProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace GroupProject.Controllers
{
    public class ProfileController : Controller
    {
        //HaoDatabase db = new HaoDatabase();
        /*TrangDatabase db = new TrangDatabase();*/
        NhatDatabase db = new NhatDatabase();

        // GET: Profile
        public readonly bool et = true;
        public ActionResult Index()
        {
            if (Session["UserSession"] != null)
            {
                string MaKH = SessionHelper.GetUserSession().getUserName();
                var user = db.KhachHangs.SingleOrDefault(s => s.MaKH == MaKH);
                ViewBag.Ten = user.Ten;

                if(user.NgaySinh != null)
                {
                    DateTime bday = (DateTime)user.NgaySinh;
                    string y = bday.Year.ToString();
                    int month = bday.Month;
                    string m = month + "";
                    if (month < 10)
                    {
                        m = "0" + month;
                    }
                    int day = bday.Day;
                    string d = day + "";
                    if (day < 10)
                    {
                        d = "0" + day;
                    }
                    string birthday = y + "-" + m + "-" + d;

                    ViewBag.NgaySinh = birthday;
                }
                
                ViewBag.Diachi = user.DiaChi;
                ViewBag.DienThoai = user.DienThoai;
                ViewBag.Email = user.Email;
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        [HttpGet, ActionName("EditProfile")]
        public ActionResult EditProfile(string id)
        {
            if (Session["UserSession"] != null)
            {
                id = SessionHelper.GetUserSession().getUserName();
                // var user = db.KhachHangs.SingleOrDefault(s => s.MaKH == id);
                var KH = from ss in db.KhachHangs where ss.MaKH == id select ss;
                return View(KH.FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost, ActionName("EditProfile")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfileSave(string MaKH)
        {
            var KHud = db.KhachHangs.Find(MaKH);
            if (TryUpdateModel(KHud, "", new string[] { "Ten", "NgaySinh", "DiaChi", "DienThoai", "Email" }))
            {
                try
                {
                    db.Entry(KHud).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChangePass()
        {
            if (Session["UserSession"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost, ActionName("ChangePass")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(ChangePass model)
        {
            string id = SessionHelper.GetUserSession().getUserName();
            var KH = db.KhachHangs.SingleOrDefault(s => s.MaKH == id);
            if (KH != null)
            {
                if (KH.MatKhau == model.CurrentPassword)
                {
                    if (model.NewPassword == model.ConfirmPassword)
                    {
                        KH.MatKhau = model.NewPassword;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("ConfirmPass", "The new password and confirm password don't match!");
                        return View("ChangePass");
                    }
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Incorrect current password");
                    return View("ChangePass");
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult getListHD()
        {
            if (Session["UserSession"] != null)
            {
                string MaKH = SessionHelper.GetUserSession().getUserName();
                var HD = from ts in db.HoaDons where ts.MaKH == MaKH select ts;
                return View(HD);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Details(string id)
        {
            if (Session["UserSession"] != null)
            {
                string MaKH = SessionHelper.GetUserSession().getUserName();
                var model = from s in db.ChiTietHoaDons where (s.MaHD == id && s.HoaDon.MaKH == MaKH) select s;

                bool flag = false;
                foreach (var item in model)
                {
                    if (id == item.MaHD)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return RedirectToAction("Error","Home");
                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
    }
}