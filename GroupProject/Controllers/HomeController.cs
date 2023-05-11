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
        NhatDatabase db = new NhatDatabase();
        //NganDatabase db = new NganDatabase();

        public ActionResult Index()
        {
            //khoi tao session cart
            UserSession userss = SessionHelper.GetUserSession();
            if (Session["UserSession"] != null)
            {
                string user = userss.getUserName();
                List<GioHang> listCartItem;
                listCartItem = db.GioHangs.Where(s => s.MaKH == user).ToList();
                Session["ShoppingCart"] = listCartItem;
            }
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