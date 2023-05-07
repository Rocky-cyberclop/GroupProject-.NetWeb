using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.FrameWork;

namespace GroupProject.Models
{
    public class SingleProductModel
    {
        public SanPham Product { get; set; }
        public IEnumerable<SanPham> RelativeProducts { get; set; }
        public string CateId { get; set; }
        public string CateName { get; set; }

    }
}