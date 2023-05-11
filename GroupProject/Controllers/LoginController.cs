using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace GroupProject.Controllers
{
    public class LoginController : Controller
    {
        //HaoDatabase db = new HaoDatabase();
        // TrangDatabase db = new TrangDatabase();
        //ThanhDatabase db = new ThanhDatabase();
        NhatDatabase db = new NhatDatabase();
        //NganDatabase db = new NganDatabase();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Login")]
        public ActionResult Login(string Email, string Password)
        {
            if(Email == "")
            {
                ModelState.AddModelError("EmailLogin", "Tên đăng nhập không được để trống.");
                return View("Index");
            }
            if(Password == "")
            {
                ModelState.AddModelError("PasswordLogin", "Mật khẩu không được để trống.");
                ViewBag.EmailLogin = Email;
                return View("Index");
            }

            var user = db.KhachHangs.SingleOrDefault(s => s.DienThoai == Email && s.MatKhau == Password);
            if(user == null)
            {
                user = db.KhachHangs.SingleOrDefault(s => s.Email == Email && s.MatKhau == Password);
            }

            var staff = db.NhanViens.SingleOrDefault(s => s.MaNV == Email && s.MatKhau == Password);
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
                ModelState.AddModelError("PasswordLogin", "Tên đăng nhập hoặc mật khẩu không chính xác.");
                ViewBag.EmailLogin = Email;
            }
            return View("Index");
        }

        static KhachHang userRegister;     
        [HttpPost, ActionName("Register")]
        [AllowAnonymous]
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

            //kt email
            var checkEmail = db.KhachHangs.SingleOrDefault(s => s.Email == model.Email);
            if (checkEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được đăng ký.");
                return View("Index");
            }

            ViewBag.Email = model.Email;
            //kt so dien thoai
            var checkPhone = db.KhachHangs.SingleOrDefault(s => s.DienThoai == model.DienThoai);
            if (checkPhone != null)
            {
                ModelState.AddModelError("DienThoai", "Số điện thoại đã được đăng ký");
                return View("Index");
            }

            ViewBag.Phone = model.DienThoai;
            //kt ngay sinh
            if (model.NgaySinh > DateTime.Today)
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh phải nhỏ hơn hôm nay");
                return View("Index");
            }

            //kt mat khau
            if (Password.Length < 3 || Password.Length > 50)
            {
                ModelState.AddModelError("Password", "Mật khẩu phải từ 3 đến 50 ký tự");
                return View("Index");
            }
            
            if (checkTerms != null)
            {
                if (Password == model.MatKhau)
                {
                    if (ModelState.IsValid)
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
                        //db.KhachHangs.Add(model);
                        //db.SaveChanges();

                        userRegister = model;
                        var massage = MailUtils.SendMail("minhnhat0123401@gmail.com", model.Email, "zidqamoidsxdnsza", false);
                        if (massage)
                        {
                            ModelState.AddModelError("CodeError", "Đã gửi mã xác minh đến địa chỉ email của bạn.");
                            return View();
                        }
                        else
                        {
                            ModelState.AddModelError("msFail", "Có lỗi xảy ra. Vui lòng liên hệ hotline để được hỗ trợ!");
                            return View("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng ký không thành công, vui lòng kiểm tra lại");
                        return View("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("MatKhau", "Mật khẩu xác nhận không chính xác");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError("Terms", "Bạn phải đồng ý với các điều khoản");
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult ConfirmRegister(int? Code)
        {
            if (Code == null)
            {
                ModelState.AddModelError("CodeError", "Vui lòng nhập mã xác nhận");
                return View("Register");
            }
            if (Code != MailUtils.getCode())
            {
                ModelState.AddModelError("CodeError", "Mã xác nhận không chính xác");
                return View("Register");
            }
            else
            {
                db.KhachHangs.Add(userRegister);
                db.SaveChanges();
                TempData["LoginMessage"] = "Đăng ký thành công!";
                return View("Index");
            }
            
        }

        public ActionResult Logout()
        {
            Session.Remove("UserSession");
            Session.Remove("StaffSession");
            TempData["LoginMessage"] = "Đăng xuất thành công";
            return RedirectToAction("Index");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(string Email)
        {
            var user = db.KhachHangs.SingleOrDefault(s => s.Email == Email);
            
            if(user != null)
            {   
                Session["Guest"] = user.MaKH;
                var massage = MailUtils.SendMail("minhnhat0123401@gmail.com", user.Email, "zidqamoidsxdnsza", true);
                if (massage)
                {
                    ModelState.AddModelError("CodeError", "Đã gửi mã xác minh đến địa chỉ email của bạn.");
                    return View();
                }
                else
                {
                    ModelState.AddModelError("msFail", "Có lỗi xảy ra. Vui lòng liên hệ hotline để được hỗ trợ!");
                    return View("ForgetPassword");
                }
            }
            else
            {
                ModelState.AddModelError("msFail", "Email không chính xác, vui lòng thử lại");
                return View("ForgetPassword");
            }
        }

        [HttpPost]
        public ActionResult RenewPassword(int? Code)
        {
            if(Code == null)
            {
                ModelState.AddModelError("CodeError", "Vui lòng nhập mã xác nhận");
                return View("ConfirmEmail");
            }
            if (Code != MailUtils.getCode())
            {
                ModelState.AddModelError("CodeError", "Mã xác nhận không chính xác");
                return View("ConfirmEmail");
            }
            else
            {
                MailUtils.setCode();
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(string Password, string MatKhau)
        {
            if (Password.Length < 3 || Password.Length > 50)
            {
                ModelState.AddModelError("PasswordError", "Mật khẩu từ 3 đến 50 ký tự");
                return View("RenewPassword");
            }
            if (MatKhau == "")
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không được để trống");
                return View("RenewPassword");
            }

            var user = db.KhachHangs.Find(Session["Guest"]);
            if (Password == MatKhau)
            {
                if(TryUpdateModel(user, "", new string[] { "MatKhau" }))
                {
                    try
                    {
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Error Save Data");
                        return View("RenewPassword");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error Save Data");
                    return View("RenewPassword");
                }
            }
            else
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không chính xác");
                return View("RenewPassword");
            }
        }
    }
}