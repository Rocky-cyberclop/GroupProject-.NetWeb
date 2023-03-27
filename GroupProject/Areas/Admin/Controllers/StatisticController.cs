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
        HaoDatabase db = new HaoDatabase();
        // GET: Admin/Statistic
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, ActionName("GetData")]
        public JsonResult GetData(string start, string end)
        {
            DateTime startDate = Convert.ToDateTime(start);
            DateTime endDate = Convert.ToDateTime(end);
            var totalDays = (endDate - startDate).TotalDays+1;
            string[] label = new string[((int)totalDays)];
            double[] earnings = new double[((int)totalDays)];

            for(var i=0; i<((int)totalDays); i++)
            {
                long sum = 0;
                DateTime daysAfter = startDate.AddDays(i);
                var bills = db.HoaDons.Where(bs => bs.NgayDat.Equals(daysAfter));
                if (bills == null) sum = 0;
                else
                {
                    foreach (var bill in bills)
                    {
                        sum += bill.TongTien;
                    }
                }
                label[i] = startDate.AddDays(i).ToString();
                earnings[i] = sum;
            }
            return Json(new { label, earnings }, JsonRequestBehavior.AllowGet);



        }
    }
}