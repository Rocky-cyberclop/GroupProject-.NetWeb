using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork; //Thêm vào mới có thể tham chiếu đến DAL

namespace GroupProject.Controllers
{
    public class HomeController : Controller
    {
        HaoDatabase db = new HaoDatabase();
        //ThanhDatabase db = new ThanhDatabase();
        //TrangDatabase db = new TrangDatabase();
        public ActionResult Index()
        {
            //var sp = db.SanPhams.First();
            //ViewBag.TenSP = sp.Ten;
            //ViewBag.GiaSP = sp.Gia;
            //ViewBag.HA = db.HinhAnhs.Where(e => e.MaSP == sp.MaSP).First().Hinh;
            var listProduct = db.SanPhams.OrderBy(p => Guid.NewGuid()).Take(12).ToList();
            return View(listProduct);
        }

        [HttpGet, ActionName("ShowAll")]
        public ActionResult ShowAll(string TypeID = "")
        {

            /*db.Configuration.ProxyCreationEnabled = false;*/
            var listProduct = new List<SanPham>();
            if (TypeID == "")
            {
                listProduct = db.SanPhams.ToList();
            }
            else
            {
                listProduct = db.SanPhams.Where(p => p.MaSP.Substring(0, 1) == TypeID).ToList();
            }
            ViewBag.Type = TypeID;
            ViewBag.pageCount = Math.Ceiling(1.0 * listProduct.Count / 6);
            return PartialView(listProduct);
        }


        //Test csdl ở bằng hàm bên dưới
        [HttpGet]
        public ActionResult GetStaff()
        {
            NganDatabase db = new NganDatabase(); //Dùng đúng Entity trên máy
            var listStaff = from ts in db.NhanViens select ts; //Truy vấn tất cả dữ liệu trong bảng
            return View(listStaff); //Hiển thị
        }
    }
}