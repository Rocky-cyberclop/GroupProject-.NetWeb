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
        public ActionResult Index()
        {
            return View();
        }

        //Test csdl ở bằng hàm bên dưới
        [HttpGet]
        public ActionResult GetStaff()
        {
            HaoDatabase db = new HaoDatabase(); //Dùng đúng Entity trên máy
            var listStaff = from ts in db.NhanViens select ts; //Truy vấn tất cả dữ liệu trong bảng
            return View(listStaff); //Hiển thị
        }
    }
}