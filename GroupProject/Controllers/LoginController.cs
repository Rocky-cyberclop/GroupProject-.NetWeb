using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork;

namespace GroupProject.Controllers
{
    public class LoginController : Controller
    {
        HaoDatabase db = new HaoDatabase();
        // TrangDatabase db = new TrangDatabase();
        /*ThanhDatabase db = new ThanhDatabase();*/
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Login")]
        public ActionResult Login(string Phone, string Password)
        {
            if(Phone == "")
            {
                ModelState.AddModelError("Phone", "Tên đăng nhập không được để trống.");
                return View("Index");
            }
            if(Password == "")
            {
                ModelState.AddModelError("ErrorPassword", "Mật khẩu không được để trống.");
                ViewBag.Phone = Phone;
                return View("Index");
            }

            var staff = db.NhanViens.SingleOrDefault(s => s.MaNV == Phone && s.MatKhau == Password);
            var user = db.KhachHangs.SingleOrDefault(s => s.DienThoai == Phone && s.MatKhau == Password);
            if (staff != null)
            {
                SessionHelper.SetSession(new StaffSession(staff.MaNV, staff.Ten, staff.Quyen));
                return RedirectToAction("Index", "Admin/Statistic");
            }
            else if (user != null)
            {
                SessionHelper.SetSession(new UserSession(user.MaKH, user.Ten));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("ErrorPassword", "Tên đăng nhập hoặc mật khẩu không chính xác.");
                ViewBag.Phone = Phone;
            }
            return View("Index");
        }

        [HttpPost, ActionName("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(KhachHang model, string Password, string checkTerms)
        {
            if (Password == "")
            {
                ModelState.AddModelError("PasswordReg", "Mật khẩu không được để trống.");
                ViewBag.PhoneReg = model.DienThoai;
                ViewBag.Name = model.Ten;
                ViewBag.Address = model.DiaChi;
                ViewBag.Birthday = model.NgaySinh;
                return View("Index");
            }
            if (Password.Length < 3 || Password.Length > 50)
            {
                ModelState.AddModelError("PasswordReg", "Mật khẩu phải từ 3 đến 50 ký tự.");
                ViewBag.PhoneSU = model.DienThoai;
                ViewBag.Name = model.Ten;
                ViewBag.Address = model.DiaChi;
                ViewBag.Birthday = model.NgaySinh;
                return View("Index");
            }
            if (checkTerms != null)
            {
                if (Password == model.MatKhau)
                {
                    int countUser = db.KhachHangs.Count() + 1;
                    if (countUser < 100)
                    {
                        model.MaKH = "KH0" + countUser.ToString();
                    }
                    else
                    {
                        model.MaKH = "KH" + countUser.ToString();
                    }
                    db.KhachHangs.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không chính xác.");
                }
            }
            else
            {
                ModelState.AddModelError("Terms", "Bạn phải đồng ý với các điều khoản.");
            }
            
            return View("Index");
        }

        public ActionResult Logout()
        {
            Session.Remove("UserSession");
            Session.Remove("StaffSession");
            return View("Index");
        }
    }
}