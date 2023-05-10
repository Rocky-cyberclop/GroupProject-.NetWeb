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
using System.Web.Script.Serialization;

namespace GroupProject.Controllers
{
    public class CartController : Controller
    {
        HaoDatabase db = new HaoDatabase();
        //ThanhDatabase db = new ThanhDatabase(); //Dùng đúng Entity trên máy

        // GET: Cart

        public ActionResult Index()
        {

            UserSession userss = SessionHelper.GetUserSession();

            if (Session["UserSession"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string MaKH = userss.getUserName();
            var listCart = from ts in db.GioHangs where ts.MaKH == MaKH select ts;
            return View(listCart);
        }
        /********************************************************/

        //gio hang rong
        public ActionResult CartNull()
        {
            UserSession userss = SessionHelper.GetUserSession();
            string MaKH = userss.getUserName();
            if (MaKH == null)
            {
                return View("Login");
            }
            var listCart = from ts in db.GioHangs where ts.MaKH == MaKH select ts;
            return View(listCart);

        }

        /********************************************************/

        //xóa
        [HttpPost, ActionName("Delete")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            UserSession userss = SessionHelper.GetUserSession();
            string MaKH = userss.getUserName();
            GioHang gh = db.GioHangs.Where(ps => ps.MaSP == id).FirstOrDefault();
            // var path = Path.Combine(Server.MapPath("~/Content/Image"), book.CoverPage);

            db.GioHangs.Remove(gh);
            db.SaveChanges();
            var listCart = from ts in db.GioHangs where ts.MaKH == MaKH select ts;
            return RedirectToAction("Index");
        }

        //xóa tất cả
        [HttpPost, ActionName("DeleteAll")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAll()
        {
            UserSession userss = SessionHelper.GetUserSession();
            string MaKH = userss.getUserName();
            var gh = db.GioHangs.Where(ps => ps.MaKH == MaKH).ToList();
            foreach (var i in gh)
                db.GioHangs.Remove(i);

            db.SaveChanges();

            return RedirectToAction("CartNull");
        }

        /********************************************************/

        //update so luong

        [HttpGet, ActionName("PlusQuantity")]
        public JsonResult PlusQuantity(/*string user,*/ string masp)
        {
            UserSession userss = SessionHelper.GetUserSession();
            string user = userss.getUserName();
            var cart = db.GioHangs.Where(cs => cs.MaKH == user && cs.MaSP == masp).SingleOrDefault();
            var slSP = db.SanPhams.Find(masp);
            if (cart.SoLuong < slSP.SoLuong)
                cart.SoLuong++;

            db.SaveChanges();

            return Json(new { quantity = cart.SoLuong, cart = masp }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet, ActionName("MinusQuantity")]
        public JsonResult MinusQuantity(string masp)
        {
            UserSession userss = SessionHelper.GetUserSession();
            string user = userss.getUserName();
            var cart = db.GioHangs.Where(s => s.MaKH == user && s.MaSP == masp).SingleOrDefault();
            if (cart.SoLuong > 1)
                cart.SoLuong--;

            db.SaveChanges();
            return Json(new { quantity = cart.SoLuong, cart = masp }, JsonRequestBehavior.AllowGet);
        }



        /// xuat hoa don 
        public ActionResult Bill()
        {
            UserSession userss = SessionHelper.GetUserSession();

            if (Session["UserSession"] == null)
            {
                return View("CartNull");
            }
            string MaKH = userss.getUserName();
            var listCart = from ts in db.GioHangs where ts.MaKH == MaKH select ts;
            foreach (var item in listCart)
            {
                ViewBag.TenKH = item.KhachHang.Ten;
            }

            return View(listCart);
        }


        // thanh toan
        [HttpPost, ActionName("Checkout")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(HoaDon model)
        {
            UserSession userss = SessionHelper.GetUserSession();
            string user = userss.getUserName();

            var listBill = from ts in db.HoaDons select ts;
            model.MaHD = "HD" + (listBill.Count() + 1).ToString();
            model.MaKH = user;
            model.NgayDat = DateTime.Today;
            model.TrangThai = "PENDING";
            model.MaNV = null;
            var listCart = from ts in db.GioHangs where ts.MaKH == user select ts;
            long tongtien = 0;
            long thanhtien;
            foreach (var item in listCart)
            {
                thanhtien = (long)(item.SoLuong * item.GiaBan);

                if (item.SoLuong > item.SanPham.KhuyenMais.SingleOrDefault().SoLuong - 1)
                {
                    thanhtien = thanhtien - (long)(thanhtien * item.SanPham.KhuyenMais.SingleOrDefault().TiLe);
                }
                tongtien = (long)(tongtien + thanhtien);

            }

            foreach (var item in listCart)
            {
                ChiTietHoaDon ct = new ChiTietHoaDon();

                ct.MaHD = model.MaHD;
                ct.MaSP = item.MaSP;
                SanPham sp = db.SanPhams.Where(ms => ms.MaSP == item.MaSP).FirstOrDefault();
                var quantity = sp.SoLuong - item.SoLuong;
                sp.SoLuong = quantity;
                ct.SoLuong = item.SoLuong;
                ct.GiaBan = item.GiaBan;
                if (item.SoLuong >= item.SanPham.KhuyenMais.First().SoLuong)
                {
                    ct.KhuyenMai = (item.SanPham.KhuyenMais.First().TiLe).ToString();
                }
                db.ChiTietHoaDons.Add(ct);

                model.TongTien = tongtien + 30000;
                db.HoaDons.Add(model);

            }


            //xoa trong csdl
            var gh = db.GioHangs.Where(ps => ps.MaKH == user).ToList();
            foreach (var i in gh)
                db.GioHangs.Remove(i);



            db.SaveChanges();
            return View("Result");

        }

        // dat hang thanh cong
        public ActionResult Result()
        {
            return View();
        }

        //add
        [HttpPost, ActionName("Add")]
        public JsonResult Add(string masp, int soluong)
        {
            UserSession userss = SessionHelper.GetUserSession();
            string user = userss.getUserName();
            SanPham sp = db.SanPhams.Where(ps => ps.MaSP == masp).FirstOrDefault();
            var cart = db.GioHangs.Where(cs => cs.MaKH == user && cs.MaSP == masp).SingleOrDefault();
            if (cart != null)
                cart.SoLuong = cart.SoLuong + soluong;

            if (cart == null)
            {
                GioHang gh = new GioHang();
                gh.MaKH = user;
                gh.MaSP = masp;
                gh.SoLuong = soluong;
                gh.GiaBan = sp.Gia;
                db.GioHangs.Add(gh);
            }

            db.SaveChanges();

            return Json(new { quantity = cart.SoLuong, cart = masp }, JsonRequestBehavior.AllowGet);

        }
    }
}
