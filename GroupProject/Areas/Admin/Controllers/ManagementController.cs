using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.FrameWork;
using GroupProject.Controllers;

namespace GroupProject.Areas.Admin.Controllers
{
    public class ManagementController : Controller
    {
        NhatDatabase db = new NhatDatabase();
        // GET: Admin/Managament
        public ActionResult Index()
        {
            return View();
        }

        //Employees management
        [HttpGet, ActionName("Staffs")]
        public ActionResult StaffsList()
        {
            var listStaff = from ts in db.NhanViens where ts.Nghi==false select ts;
            ViewBag.right = SessionHelper.GetStaffSession().GetRight();
            return View(listStaff);
        }

        [HttpGet, ActionName("AddStaff")]
        public ActionResult AddStaff() {
            var listStaff = from ts in db.NhanViens select ts;
            ViewBag.count = listStaff.Count()+1;

            return View();
        }

        [HttpPost, ActionName("AddStaff")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddStaff(NhanVien model)
        {
            var listStaff = from ts in db.NhanViens select ts;
            model.MaNV = "NV0" + (listStaff.Count() + 1).ToString();
            model.MatKhau = "NV0" + (listStaff.Count() + 1).ToString();

            db.NhanViens.Add(model);
            db.SaveChanges();

            return RedirectToAction("Staffs");
        }

        [HttpGet, ActionName("EditStaff")]
        public ActionResult EditStaff(string id)
        {
            var staff = from ss in db.NhanViens where ss.MaNV==id select ss;
            return View(staff.FirstOrDefault());
        }

        [HttpPost, ActionName("EditStaff")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditStaffSave(string id)
        {
            var staff = db.NhanViens.Find(id);
            TryUpdateModel(staff, "", new string[] { "Ten", "Email", "DiaChi", "DienThoai", "Quyen" });
            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Staffs");
        }

        [HttpPost, ActionName("DeleteStaff")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteStaff(string id)
        {
            NhanVien staff = db.NhanViens.Where(x => x.MaNV == id).SingleOrDefault();
            staff.Nghi = true;
            db.SaveChanges();
            return RedirectToAction("Staffs");
        }

        //Products management
        [HttpGet, ActionName("AddType")]
        public ActionResult AddType()
        {
            return View();
        }

        [HttpPost, ActionName("AddType")]
        public ActionResult AddType(Loai model)
        {
            db.Loais.Add(model);
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        [HttpGet, ActionName("Products")]
        public ActionResult ProductsList()
        {
            var listProduct = from ps in db.SanPhams where ps.KhongBan==false select ps;
            return View(listProduct);
        }

        [HttpGet, ActionName("CreateProduct")]
        public ActionResult CreateProduct()
        {
            var listType = from ls in db.Loais select ls;
            List<Loai> listTypeList = listType.ToList();
            SelectList lsType = new SelectList(listTypeList, "MaLoai", "TenLoai");
            ViewBag.lsType = lsType;
            return View();
        }

        [HttpPost, ActionName("CreateProduct")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(SanPham model)
        {
            var type = Request["type"].ToString();
            var product = db.SanPhams.Where(ps=>ps.MaSP.StartsWith(type));
            model.MaSP = type + (product.Count() + 1).ToString();
            model.KhongBan = false;
            db.SanPhams.Add(model);
            db.SaveChanges();
            var randomName = DateTime.Now.ToBinary().ToString();
            var name = "";
            for (var i = 0; i < Request.Files.Count; i++)
            {
                if (Request.Files[i].FileName.Contains(".png") || Request.Files[i].FileName.Contains(".jpg"))
                {
                    //Making radom name and save to server
                    name = randomName + "_" + (i+1).ToString() + Request.Files[i].FileName.ToString().Substring(Request.Files[i].FileName.IndexOf('.'));
                    var pathFile = Server.MapPath("~/Assets/Products/Images/" + name);
                    Request.Files[i].SaveAs(pathFile);

                    //Save to database
                    HinhAnh pic = new HinhAnh();
                    pic.MaSP = model.MaSP;
                    pic.Hinh = name;
                    db.HinhAnhs.Add(pic);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Products");
        }

        [HttpGet, ActionName("EditProduct")]
        public ActionResult EditProduct(string id)
        {
            var product = from ps in db.SanPhams where ps.MaSP == id select ps;
            return View(product.FirstOrDefault());
        }

        [HttpPost, ActionName("EditProduct")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductSave(string id)
        {
            var product = db.SanPhams.Find(id);
            TryUpdateModel(product, "", new string[] { "Ten", "XuatXu", "DonVi", "SoLuong", "Gia", "MoTa" });
            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Products");
        }


        [HttpPost, ActionName("DeleteProduct")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(string id)
        {
            var path = Server.MapPath("~/Assets/Products/Images/");
            SanPham product = db.SanPhams.Where(ps => ps.MaSP == id).SingleOrDefault();
            product.KhongBan = true;

            while(product.HinhAnhs.Count > 0)
            {
                path = Server.MapPath("~/Assets/Products/Images/"+product.HinhAnhs.First().Hinh.ToString());
                System.IO.File.Delete(path);
                HinhAnh pic = product.HinhAnhs.First();
                db.HinhAnhs.Remove(pic);
            }
            db.SaveChanges();
            return RedirectToAction("Products");
        }

        //Bills Management
        [HttpGet, ActionName("Bills")]
        public ActionResult Bills()
        {
            var bills = db.HoaDons.Where(bs => bs.MaNV == null);
            return View(bills);
        }

        [HttpGet, ActionName("DetailsBill")]
        public ActionResult DetailsBill(string id)
        {
            var billDetails = db.ChiTietHoaDons.Where(bds => bds.MaHD == id);
            return View(billDetails);
        }

        [HttpGet, ActionName("AcceptBill")]
        public ActionResult AcceptBill(string id)
        {
            StaffSession staff = SessionHelper.GetStaffSession();
            var maNV = staff.GetID();
            var bill = db.HoaDons.Where(bs => bs.MaHD == id).FirstOrDefault();
            bill.MaNV = maNV;
            bill.TrangThai = "SUCCESS";
            db.SaveChanges();
            return RedirectToAction("Bills");
        }

    }
}