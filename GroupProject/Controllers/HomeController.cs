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
        NhatDatabase db = new NhatDatabase();
        public ActionResult Index()
        {
            //var sp = db.SanPhams.First();
            //ViewBag.TenSP = sp.Ten;
            //ViewBag.GiaSP = sp.Gia;
            //ViewBag.HA = db.HinhAnhs.Where(e => e.MaSP == sp.MaSP).First().Hinh;
            var sp = db.SanPhams;
            ViewBag.SP = sp;

            return View();
        }

        //Test csdl ở bằng hàm bên dưới
        [HttpGet]
        public ActionResult GetStaff()
        {
            NhatDatabase db = new NhatDatabase(); //Dùng đúng Entity trên máy
            var listStaff = from ts in db.NhanViens select ts; //Truy vấn tất cả dữ liệu trong bảng
            return View(listStaff); //Hiển thị
        }
    }
}