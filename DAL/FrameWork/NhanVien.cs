﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL.FrameWork
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class NhanVien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhanVien()
        {
            this.HoaDons = new HashSet<HoaDon>();
        }
    
        [DisplayName("Id")]
        public string MaNV { get; set; }
        [DisplayName("Họ và tên")]
        public string Ten { get; set; }
        [DisplayName("Địa chỉ hộ khẩu")]
        public string Email { get; set; }
        [DisplayName("Địa chỉ email")]
        public string DiaChi { get; set; }
        [DisplayName("Số điện thoại")]
        public string DienThoai { get; set; }
        [DisplayName("Quyền")]
        public string Quyen { get; set; }
        [DisplayName("Mật khẩu")]
        public string MatKhau { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}
