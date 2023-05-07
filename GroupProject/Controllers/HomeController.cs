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
        //HaoDatabase db = new HaoDatabase();
        //ThanhDatabase db = new ThanhDatabase();
        //TrangDatabase db = new TrangDatabase();
        //NhatDatabase db = new NhatDatabase();
        NganDatabase db = new NganDatabase();

        public ActionResult Index()
        {
            //lay ngau nhien 12 san pham
            var listProduct = db.SanPhams.OrderBy(p => Guid.NewGuid()).Take(12).ToList();
            return View(listProduct);
        }       

        //Test csdl ở bằng hàm bên dưới
        [HttpGet]
        public ActionResult GetStaff()
        {
            var listStaff = from ts in db.NhanViens select ts; //Truy vấn tất cả dữ liệu trong bảng
            return View(listStaff); //Hiển thị
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}