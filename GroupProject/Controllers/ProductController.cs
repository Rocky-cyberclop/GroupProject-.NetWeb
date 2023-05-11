using DAL.FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using GroupProject.Models;

namespace GroupProject.Controllers
{
    public class ProductController : Controller
    {
        //HaoDatabase db = new HaoDatabase();
        NganDatabase db = new NganDatabase();
        //ThanhDatabase db = new ThanhDatabase();
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
                listProduct = db.SanPhams.Where(p => p.MaSP.Substring(0, 1) == cate).ToList();
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

        public ActionResult Detail(string id)
        {
            UserSession user = SessionHelper.GetUserSession();
            if (Session["UserSession"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            string MaKH = user.getUserName();
            SingleProductModel spmProduct = new SingleProductModel();
            spmProduct.Product = db.SanPhams.Where(ps => ps.MaSP == id).FirstOrDefault();
            spmProduct.CateId = id.Substring(0, 1);
            spmProduct.CateName = db.Loais.Find(spmProduct.CateId).TenLoai;
            spmProduct.RelativeProducts = db.SanPhams.Where(s => s.MaSP.Substring(0, 1) == spmProduct.CateId).OrderBy(p => Guid.NewGuid()).Take(6).ToList();

            GioHang gh = db.GioHangs.Where(ps => ps.MaKH == MaKH).FirstOrDefault();
            GioHang cart = db.GioHangs.Where(cs => cs.MaKH == MaKH && cs.MaSP == id).SingleOrDefault();

            ViewBag.slHienTai = spmProduct.Product.SoLuong;
            if (cart != null )
                ViewBag.slHienTai = spmProduct.Product.SoLuong - cart.SoLuong;
            ViewBag.slCoTheThem = 1;
            if (ViewBag.slHienTai == 0 )
                ViewBag.slCoTheThem = 0;

            return View(spmProduct);
        }

        [HttpGet, ActionName("FindProduct")]
        public JsonResult FindProduct(string search)
        {
            List<SanPham> ls = db.SanPhams.Where(p => p.Ten.Contains(search)).Take(5).ToList();
            List<FindingModel> fd = new List<FindingModel>();
            foreach(var item in ls)
            {
                FindingModel findingModel = new FindingModel();
                findingModel.Id = item.MaSP;
                findingModel.Value = item.Ten;
                fd.Add(findingModel);
            }
            return Json(new { data = fd }, JsonRequestBehavior.AllowGet);
        }

    }
}