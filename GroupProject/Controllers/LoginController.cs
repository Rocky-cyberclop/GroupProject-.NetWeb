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
        ThanhDatabase db = new ThanhDatabase();
        //NhatDatabase db = new NhatDatabase();
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
                ModelState.AddModelError("Email", "Tên đăng nhập không được để trống.");
                return View("Index");
            }
            if(Password == "")
            {
                ModelState.AddModelError("ErrorPassword", "Mật khẩu không được để trống.");
                ViewBag.Phone = Email;
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
                ModelState.AddModelError("ErrorPassword", "Tên đăng nhập hoặc mật khẩu không chính xác.");
                ViewBag.Name = Email;
            }
            return View("Index");
        }

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
            string pattern = "[A-Za-z0-9._-]*@+[A-Za-z0-9]+(\\.[A-Za-z0-9])";
            
            if(model.Email == null)
            {
                ModelState.AddModelError("EmailReg", "Email không được để trống");
                return View("Index");
            }
            if (!Regex.IsMatch(model.Email, pattern))
            {
                ModelState.AddModelError("EmailReg", "Email không hợp lệ");
                return View("Index");
            }
            if (checkEmail != null)
            {
                ModelState.AddModelError("EmailReg", "Email đã được đăng ký.");
                return View("Index");
            }

            ViewBag.Email = model.Email;
            //kt so dien thoai
            var checkPhone = db.KhachHangs.SingleOrDefault(s => s.DienThoai == model.DienThoai);
            pattern = "[0-9]{10,10}";
            if (model.DienThoai == null)
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại không được để trống");
                return View("Index");
            }
            if (!Regex.IsMatch(model.DienThoai, pattern))
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại không đúng định dạng");
                return View("Index");
            }
            if (checkPhone != null)
            {
                ModelState.AddModelError("PhoneReg", "Số điện thoại đã được đăng ký");
                return View("Index");
            }

            ViewBag.Phone = model.DienThoai;
            //kt ngay sinh
            if (model.NgaySinh > DateTime.Today)
            {
                ModelState.AddModelError("Birthday", "Ngày sinh phải nhỏ hơn hôm nay");
                return View("Index");
            }

            //kt mat khau
            if (Password.Length < 3 || Password.Length > 50)
            {
                ModelState.AddModelError("PasswordReg", "Mật khẩu từ 3 đến 50 ký tự");
                return View("Index");
            }
            if (model.MatKhau == null)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không được để trống");
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
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không chính xác");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError("Terms", "Bạn phải đồng ý với các điều khoản");
                return View("Index");
            }
        }


        public ActionResult Logout()
        {
            Session.Remove("UserSession");
            Session.Remove("StaffSession");
            return View("Index");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(string Email)
        {
            var user = db.KhachHangs.SingleOrDefault(s => s.Email == Email);
            string subject = "Xác nhận mật khẩu";
            string body = "Chào bạn,\nBạn vừa yêu cầu mật khẩu mới.\nMã xác nhận của bạn là: " + MailUtils.getCode();
            if(user != null)
            {
                string email = user.Email;
                var massage = MailUtils.SendMail("minhnhat0123401@gmail.com", email, subject, body, "zidqamoidsxdnsza");
                if (massage)
                {
                    Session["Guest"] = user.MaKH;
                    ModelState.AddModelError("msSuccess", "Đã gửi mã xác minh đến địa chỉ email của bạn");
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
                ModelState.AddModelError("msFail", "Email không chính xác");
                return View("ForgetPassword");
            }
        }

        [HttpPost]
        public ActionResult RenewPassword(int Code)
        {
            if (Code != MailUtils.getCode())
            {
                ModelState.AddModelError("CodeError", "Mã xác nhận không chính xác");
                ModelState.AddModelError("Resend", "Gửi lại mã");
                return View("ConfirmEmail");
            }
            else
            {
                MailUtils.reCode();
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