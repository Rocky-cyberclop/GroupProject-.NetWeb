using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            //Dinh dang yyyy-mm-dd
            if (model.NgaySinh != null)
            {
                DateTime bday = (DateTime)model.NgaySinh;
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
                ViewBag.Birthday = birthday;
            }
            ViewBag.Name = model.Ten;
            ViewBag.Address = model.DiaChi;

            //kt so dien thoai
            var checkPhone = db.KhachHangs.SingleOrDefault(s => s.DienThoai == model.DienThoai);
            string pattern = "[0-9]{10,10}";
            if (model.DienThoai == null)
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại không được để trống.");
                return View("Index");
            }
            if (checkPhone != null)
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại đã được đăng ký.");
                return View("Index");
            }
            if (!Regex.IsMatch(model.DienThoai, pattern))
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại không đúng định dạng.");
                return View("Index");
            }

            ViewBag.Phone = model.DienThoai;
            //kt ngay sinh
            if (model.NgaySinh > DateTime.Today)
            {
                ModelState.AddModelError("Birthday", "Ngày sinh phải nhỏ hơn hôm nay.");
                return View("Index");
            }

            //kt mat khau
            if (Password.Length < 3 || Password.Length > 50)
            {
                ModelState.AddModelError("PasswordReg", "Mật khẩu từ 3 đến 50 ký tự.");
                return View("Index");
            }
            if (model.MatKhau == null)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không được để trống.");
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
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không chính xác.");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError("Terms", "Bạn phải đồng ý với các điều khoản.");
                return View("Index");
            }
        }


        public ActionResult Logout()
        {
            Session.Remove("UserSession");
            Session.Remove("StaffSession");
            return View("Index");
        }
    }
}