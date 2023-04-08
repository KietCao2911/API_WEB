using System;
using System.Collections.Generic;

namespace API_DSCS2_WEBBANGIAY.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            PhieuNhapXuats = new HashSet<PhieuNhapXuat>();
            ChiTietCoupons = new HashSet<ChiTietCoupon>();
        }

        public string MaCoupon { get; set; }
        public string TenCoupon { get; set; } = "";
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int SoLanDung { get; set; } = 0;
        public decimal GiaTri { get; set; } = 0;
        public bool KieuGiaTri { get; set; } = false;
        public string MoTa { get; set; } = "";
        public virtual ICollection<PhieuNhapXuat> PhieuNhapXuats { get; set; }
        public virtual ICollection<ChiTietCoupon> ChiTietCoupons { get; set; }

    }
}
