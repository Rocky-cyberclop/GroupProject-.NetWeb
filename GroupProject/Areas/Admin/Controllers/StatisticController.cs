using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork;

namespace GroupProject.Areas.Admin.Controllers
{
    public class StatisticController : Controller
    {
        // GET: Admin/Statistic
        public ActionResult Index()
        {
            string label = "['January', 'February', 'March', 'April', 'May', 'June']";
            ViewBag.label = label;
            return View();
        }
    }
}