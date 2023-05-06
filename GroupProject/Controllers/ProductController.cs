using DAL.FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace GroupProject.Controllers
{
    public class ProductController : Controller
    {
        NganDatabase db = new NganDatabase();
        // GET: Product
        public ActionResult Index(string cate = "all", string sort = "no", int page = 1)
        {
            var listProduct = new List<SanPham>();
            ViewBag.Type = cate;
            ViewBag.Sort = sort;
            listProduct = db.SanPhams.ToList();
            if (cate == "all")
            {
                listProduct = db.SanPhams.ToList();
            }
            else
            {
                listProduct = db.SanPhams.Where(p => p.MaSP.Substring(0, 1) == cate.Substring(0, 1)).ToList();
            }
            if (sort == "asc")
            {
                listProduct = listProduct.OrderBy(p => p.Gia).ToList();
            }
            else if (sort == "desc")
            {
                listProduct = listProduct.OrderByDescending(p => p.Gia).ToList();
            }
            return View(listProduct.ToPagedList(page, 8));
        }

    }
}