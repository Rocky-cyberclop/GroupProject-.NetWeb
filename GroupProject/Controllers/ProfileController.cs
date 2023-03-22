using DAL.FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GroupProject.Controllers
{
    public class ProfileController : Controller
    {
        NhatDatabase db = new NhatDatabase();
        // GET: Profile
        public ActionResult Index()
        {
            if(Session["UserSession"] != null)
            {
                UserSession userss = SessionHelper.GetUserSession();
                string MaKH = userss.getUserName();
                var user = db.KhachHangs.SingleOrDefault(s => s.MaKH == MaKH);
                ViewBag.Ten = user.Ten;
            }
            
            return View();
        }
    }
}